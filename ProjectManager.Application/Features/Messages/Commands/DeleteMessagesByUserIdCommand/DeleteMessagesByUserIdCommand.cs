using MediatR;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.Messages.Commands.DeleteMessagesByUserIdCommand
{
    public record DeleteMessagesByUserIdCommand(string UserId) : IRequest<Unit>;
}
