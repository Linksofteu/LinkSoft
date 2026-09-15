using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace LinkSoft.WordTemplateToPdf;

internal static class SquareBracketTagNormalizer
{
    public const string SquareTagPrefix = "[[";
    public const string SquareTagSuffix = "]]";
    public const string MiniWordTagPrefix = "{{";
    public const string MiniWordTagSuffix = "}}";

    private const string EscapedMiniWordTagPrefix = "{\u200B{";
    private const string EscapedMiniWordTagSuffix = "}\u200B}";
    private const int MaximumMergedTagLength = 4096;
    private const int MaximumTagsPerParagraph = 512;

    public static byte[] Normalize(string templatePath)
    {
        using var stream = new MemoryStream(File.ReadAllBytes(templatePath));

        using (var document = WordprocessingDocument.Open(stream, true))
        {
            RewriteTags(document.MainDocumentPart?.Document);

            foreach (var header in document.MainDocumentPart?.HeaderParts.Select(part => part.Header) ?? Enumerable.Empty<Header>())
            {
                RewriteTags(header);
            }

            foreach (var footer in document.MainDocumentPart?.FooterParts.Select(part => part.Footer) ?? Enumerable.Empty<Footer>())
            {
                RewriteTags(footer);
            }

            document.Save();
        }

        return stream.ToArray();
    }

    private static void RewriteTags(OpenXmlPartRootElement? rootElement)
    {
        if (rootElement is null)
        {
            return;
        }

        foreach (var paragraph in rootElement.Descendants<Paragraph>())
        {
            var textNodes = paragraph.Descendants<Text>().ToList();
            if (textNodes.Count == 0)
            {
                continue;
            }

            foreach (var textNode in textNodes)
            {
                textNode.Text = EscapeExistingMiniWordTags(textNode.Text ?? string.Empty);
            }

            RewriteSquareBracketTags(textNodes);
        }
    }

    private static string EscapeExistingMiniWordTags(string text)
    {
        return text
            .Replace(MiniWordTagPrefix, EscapedMiniWordTagPrefix, StringComparison.Ordinal)
            .Replace(MiniWordTagSuffix, EscapedMiniWordTagSuffix, StringComparison.Ordinal);
    }

    private static void RewriteSquareBracketTags(IReadOnlyList<Text> textNodes)
    {
        var paragraphText = string.Concat(textNodes.Select(textNode => textNode.Text ?? string.Empty));
        var tagRanges = FindTagRanges(paragraphText);
        if (tagRanges.Count == 0)
        {
            return;
        }

        var rewrites = tagRanges
            .Select(range => new TagRewrite(
                FindTextPosition(textNodes, range.StartOffset, preferNextNodeAtBoundary: true),
                FindTextPosition(textNodes, range.EndOffset, preferNextNodeAtBoundary: false),
                paragraphText[range.StartOffset..range.EndOffset]))
            .ToList();

        for (var rewriteIndex = rewrites.Count - 1; rewriteIndex >= 0; rewriteIndex--)
        {
            var rewrite = rewrites[rewriteIndex];
            RewriteTag(textNodes, rewrite.StartPosition, rewrite.EndPosition, rewrite.Tag);
        }
    }

    private static IReadOnlyList<TagRange> FindTagRanges(string paragraphText)
    {
        var tagRanges = new List<TagRange>();
        var searchOffset = 0;

        while (tagRanges.Count < MaximumTagsPerParagraph)
        {
            var openOffset = paragraphText.IndexOf(SquareTagPrefix, searchOffset, StringComparison.Ordinal);
            if (openOffset < 0)
            {
                break;
            }

            var closeOffset = paragraphText.IndexOf(
                SquareTagSuffix,
                openOffset + SquareTagPrefix.Length,
                StringComparison.Ordinal);

            if (closeOffset < 0)
            {
                break;
            }

            var endOffset = closeOffset + SquareTagSuffix.Length;
            if (endOffset - openOffset <= MaximumMergedTagLength)
            {
                tagRanges.Add(new TagRange(openOffset, endOffset));
            }

            searchOffset = endOffset;
        }

        return tagRanges;
    }

    private static void RewriteTag(
        IReadOnlyList<Text> textNodes,
        TextPosition startPosition,
        TextPosition endPosition,
        string tag)
    {
        var startText = textNodes[startPosition.NodeIndex].Text ?? string.Empty;
        var endText = textNodes[endPosition.NodeIndex].Text ?? string.Empty;
        var beforeTag = startText[..startPosition.Offset];
        var afterTag = endText[endPosition.Offset..];
        var rewrittenTag = tag
            .Replace(SquareTagPrefix, MiniWordTagPrefix, StringComparison.Ordinal)
            .Replace(SquareTagSuffix, MiniWordTagSuffix, StringComparison.Ordinal);

        textNodes[startPosition.NodeIndex].Text = beforeTag + rewrittenTag;

        for (var index = startPosition.NodeIndex + 1; index < endPosition.NodeIndex; index++)
        {
            textNodes[index].Text = string.Empty;
        }

        if (endPosition.NodeIndex == startPosition.NodeIndex)
        {
            textNodes[startPosition.NodeIndex].Text += afterTag;
            return;
        }

        textNodes[endPosition.NodeIndex].Text = afterTag;
    }

    private static TextPosition FindTextPosition(
        IReadOnlyList<Text> textNodes,
        int absoluteOffset,
        bool preferNextNodeAtBoundary)
    {
        var currentOffset = 0;
        for (var nodeIndex = 0; nodeIndex < textNodes.Count; nodeIndex++)
        {
            var textLength = textNodes[nodeIndex].Text?.Length ?? 0;
            var nextOffset = currentOffset + textLength;

            if (absoluteOffset < nextOffset ||
                (!preferNextNodeAtBoundary && absoluteOffset == nextOffset) ||
                nodeIndex == textNodes.Count - 1)
            {
                return new TextPosition(nodeIndex, absoluteOffset - currentOffset);
            }

            currentOffset = nextOffset;
        }

        throw new InvalidOperationException("Could not resolve text position for a template placeholder.");
    }

    private readonly record struct TextPosition(int NodeIndex, int Offset);

    private readonly record struct TagRange(int StartOffset, int EndOffset);

    private readonly record struct TagRewrite(TextPosition StartPosition, TextPosition EndPosition, string Tag);
}
