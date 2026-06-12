using System.ComponentModel;
using System.Diagnostics;

namespace LinkSoft.WordTemplateToPdf;

internal sealed class LibreOfficePdfConverter
{
    private readonly string _executable;
    private readonly TimeSpan _timeout;

    public LibreOfficePdfConverter(string executable, TimeSpan timeout)
    {
        _executable = executable;
        _timeout = timeout;
    }

    public async Task<string> ConvertAsync(
        string docxPath,
        string outputDirectory,
        CancellationToken cancellationToken)
    {
        var profileDirectory = Path.Combine(outputDirectory, "libreoffice-profile");
        var conversionFailed = false;

        try
        {
            Directory.CreateDirectory(profileDirectory);

            var result = await RunLibreOfficeAsync(docxPath, outputDirectory, profileDirectory, cancellationToken).ConfigureAwait(false);

            if (result.ExitCode != 0)
            {
                throw new InvalidOperationException(
                    $"LibreOffice conversion failed with exit code {result.ExitCode}. " +
                    $"Standard error: {result.StandardError.Trim()} Standard output: {result.StandardOutput.Trim()}");
            }

            var expectedPdfPath = Path.Combine(outputDirectory, $"{Path.GetFileNameWithoutExtension(docxPath)}.pdf");
            if (!File.Exists(expectedPdfPath))
            {
                throw new InvalidOperationException($"LibreOffice did not create the expected PDF file: {expectedPdfPath}");
            }

            return expectedPdfPath;
        }
        catch
        {
            conversionFailed = true;
            throw;
        }
        finally
        {
            DeleteProfileDirectory(profileDirectory, suppressExceptions: conversionFailed);
        }
    }

    private async Task<ProcessResult> RunLibreOfficeAsync(
        string docxPath,
        string outputDirectory,
        string profileDirectory,
        CancellationToken cancellationToken)
    {
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _executable,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.StartInfo.ArgumentList.Add($"-env:UserInstallation={new Uri(EnsureDirectorySeparator(profileDirectory)).AbsoluteUri}");
        process.StartInfo.ArgumentList.Add("--headless");
        process.StartInfo.ArgumentList.Add("--nologo");
        process.StartInfo.ArgumentList.Add("--nofirststartwizard");
        process.StartInfo.ArgumentList.Add("--convert-to");
        process.StartInfo.ArgumentList.Add("pdf");
        process.StartInfo.ArgumentList.Add("--outdir");
        process.StartInfo.ArgumentList.Add(outputDirectory);
        process.StartInfo.ArgumentList.Add(docxPath);

        try
        {
            if (!process.Start())
            {
                throw new InvalidOperationException($"Failed to start LibreOffice executable '{_executable}'.");
            }
        }
        catch (Win32Exception exception)
        {
            throw new InvalidOperationException(
                $"LibreOffice executable '{_executable}' was not found. Configure LibreOfficePath or make sure 'libreoffice' is available in PATH.",
                exception);
        }

        var standardOutputTask = process.StandardOutput.ReadToEndAsync();
        var standardErrorTask = process.StandardError.ReadToEndAsync();

        using var timeoutTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutTokenSource.CancelAfter(_timeout);

        try
        {
            await process.WaitForExitAsync(timeoutTokenSource.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            KillProcess(process);
            throw new TimeoutException($"LibreOffice conversion timed out after {_timeout}.");
        }
        catch (OperationCanceledException)
        {
            KillProcess(process);
            throw;
        }

        var standardOutput = await standardOutputTask.ConfigureAwait(false);
        var standardError = await standardErrorTask.ConfigureAwait(false);

        return new ProcessResult(process.ExitCode, standardOutput, standardError);
    }

    private static string EnsureDirectorySeparator(string path)
    {
        return Path.EndsInDirectorySeparator(path)
            ? path
            : path + Path.DirectorySeparatorChar;
    }

    private static void KillProcess(Process process)
    {
        try
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
            }
        }
        catch (InvalidOperationException)
        {
        }
    }

    private static void DeleteProfileDirectory(string profileDirectory, bool suppressExceptions)
    {
        try
        {
            if (Directory.Exists(profileDirectory))
            {
                Directory.Delete(profileDirectory, recursive: true);
            }
        }
        catch (Exception exception) when (suppressExceptions && IsProfileCleanupException(exception))
        {
        }
        catch (Exception exception) when (IsProfileCleanupException(exception))
        {
            throw new IOException($"Could not delete LibreOffice profile directory '{profileDirectory}'.", exception);
        }
    }

    private static bool IsProfileCleanupException(Exception exception)
    {
        return exception is IOException or UnauthorizedAccessException or NotSupportedException;
    }

    private sealed record ProcessResult(int ExitCode, string StandardOutput, string StandardError);
}
