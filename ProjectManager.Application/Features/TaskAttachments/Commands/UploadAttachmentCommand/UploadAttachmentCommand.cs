using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.TaskAttachments.Commands.UploadTaskAttachmentCommand
{
    public record UploadAttachmentCommand(int TaskId, IFormFile File, string UserId, int ProjectId) : IRequest<int>;
}
