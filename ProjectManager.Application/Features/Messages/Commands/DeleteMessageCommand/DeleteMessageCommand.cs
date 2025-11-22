using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.Messages.Commands.DeleteMessageCommand
{
    public record DeleteMessageCommand(string UserId, int MessageId) : IRequest<Unit>;
}

