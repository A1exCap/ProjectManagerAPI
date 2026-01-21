using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common;
using ProjectManager.Application.DTOs.Comment;
using ProjectManager.Application.DTOs.Message;
using ProjectManager.Application.Features.Comments.Queries.GetAllCommentsByTaskIdQuery;
using ProjectManager.Application.Mappers;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.Messages.Queries.GetAllMessagesByUserQuery
{
    public class GetAllMessagesByUserQueryHandler : IRequestHandler<GetAllMessagesByUserQuery, PagedResult<MessageDto>>
    {
        private readonly ILogger<GetAllMessagesByUserQueryHandler> _logger;
        private readonly IMessageRepository _messageRepository;
        public GetAllMessagesByUserQueryHandler(ILogger<GetAllMessagesByUserQueryHandler> logger, IMessageRepository messageRepository)
        {
            _logger = logger;
            _messageRepository = messageRepository;
        }
        public async Task<PagedResult<MessageDto>> Handle(GetAllMessagesByUserQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetAllMessagesByUserQuery for userId: {UserId}", request.UserId);

            var query = _messageRepository.GetMessagesByUserId(request.UserId);

            var totalCount = await query.CountAsync(cancellationToken);

            var messages = await query.OrderByDescending(m => m.CreatedAt)
                .Skip((request.QueryParams.PageNumber - 1) * request.QueryParams.PageSize)
                .Take(request.QueryParams.PageSize)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Retrieved {TotalCount} messages by userId: {UserId}", totalCount, request.UserId);
            return new PagedResult<MessageDto>
            {
                Items = messages.Select(MessageMapper.ToDto),
                TotalCount = totalCount,
                PageNumber = request.QueryParams.PageNumber,
                PageSize = request.QueryParams.PageSize
            };
        }
    }
}
