using MediatR;
using ProjectManager.Application.DTOs.TaskAttachment;
using ProjectManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.TaskAttachments.Queries.DownloadAttachmentByIdQuery
{
    public record DownloadAttachmentByIdQuery(int ProjectId, int TaskId, int AttachmentId, string UserId) : IRequest<DownloadTaskAttachmentDto>;
}
