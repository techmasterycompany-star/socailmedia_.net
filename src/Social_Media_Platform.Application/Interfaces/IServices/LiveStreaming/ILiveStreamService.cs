using Social_Media_Platform.Application.DTOS.LiveStream;
using Social_Media_Platform.Application.DTOS.LiveStreaming;
using Social_Media_Platform.Application.Common.DTOs;
using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Application.Interfaces.IServices
{
    public interface ILiveStreamService
    {
        Task<PagedResult<LiveStreamDto>> GetAllAsync(int page, int pageSize);
        Task<LiveStreamDto> ExecuteGetByIdAsync(long streamId);
        Task<StartLiveStreamResponse> ExecuteStartAsync(long hostId, StartLiveStreamRequest request);
        Task<StartLiveStreamResponse> ExecuteJoinAsync(long viewerId, long streamId);
        Task ExecuteEndAsync(long hostId, long streamId);
        Task<StreamChatMessageDTO> ExecuteSendMessageAsync(long senderId, long streamId, string content, StreamMessageType messageType);
        Task<PagedResult<StreamChatMessageDTO>> ExecuteGetMessagesAsync(long viewerId, long streamId, int page, int pageSize);
    }
}