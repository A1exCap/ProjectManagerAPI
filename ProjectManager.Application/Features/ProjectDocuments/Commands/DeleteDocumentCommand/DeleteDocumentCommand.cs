using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.ProjectDocuments.Commands.DeleteDocumentCommand
{
    public record DeleteDocumentCommand(int ProjectId, string UserId, int DocumentId) : IRequest<Unit>;
}
