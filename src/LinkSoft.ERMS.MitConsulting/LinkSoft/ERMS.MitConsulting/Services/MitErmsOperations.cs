using LinkSoft.ERMS.Services;
using LinkSoft.ERMS.MITConsulting.ExtensionTypes;
using LinkSoft.ERMS.MITConsulting.Models;
using LinkSoft.ERMS.Options;
using Microsoft.Extensions.Options;

namespace LinkSoft.ERMS.MITConsulting.Services;

public class MitErmsOperations(IErmsService ermsService, IOptions<ErmsOperationsOptions> options) : ErmsOperations(ermsService, options)
{
    public Task<Result> SubmitToSignatureBook(SubmitToSignatureBookDto dto)
    {
        var builder = new EventBatchBuilder()
            .AddSubmitToSugnatureBookEvent(dto);

        return SendEventsSyn(builder);
    }

    public Task<Result> SubmitToExternalSignatureBook(SubmitToSignatureBookDto dto)
    {
        var builder = new EventBatchBuilder()
            .AddSubmitToSignatureExternalBookEvent(dto);

        return SendEventsSyn(builder);
    }

    public Task<Result> RemoveFromSignatureBook(string componentId, string? reason = null)
    {
        var builder = new EventBatchBuilder()
            .AddRemoveFromSignatureBookEvent(componentId, reason);

        return SendEventsSyn(builder);
    }
    public Task<Result> ApproveSignatureExternal(string componentId)
    {
        var builder = new EventBatchBuilder()
            .AddSignatureApproveEvent(componentId);

        return SendEventsSyn(builder);
    }
    public Task<Result> RejectSignatureExternal(string componentId, string? reason = null)
    {
        var builder = new EventBatchBuilder()
            .AddSignatureRejectEvent(componentId, reason);

        return SendEventsSyn(builder);
    }

    public Task<Result> CancelSignatureRequest(string componentId, string? reason = null)
    {
        var builder = new EventBatchBuilder()
            .AddSignatureCancelEvent(componentId, reason);

        return SendEventsSyn(builder);
    }
}
