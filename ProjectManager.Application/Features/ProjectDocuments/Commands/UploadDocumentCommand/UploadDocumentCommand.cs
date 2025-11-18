using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.ProjectDocuments.Commands.UploadDocumentCommand
{
    public record UploadDocumentCommand(IFormFile File, string UserId, int ProjectId) : IRequest<int>;
}
