using MediatR;
using Microsoft.AspNetCore.Identity;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.Tasks.Commands.CreateTask
{
    public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, int>
    {
        private readonly IProjectTaskRepository _taskRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public CreateTaskCommandHandler(IProjectTaskRepository taskRepository, IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _taskRepository = taskRepository;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<int> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        { 
            User? assignee = null;
            if (!string.IsNullOrEmpty(request.AssigneeEmail))
            {
                assignee = await _userManager.FindByEmailAsync(request.AssigneeEmail);
                if (assignee == null)
                    throw new Exception($"User with email '{request.AssigneeEmail}' not found.");
            }

            var task = new ProjectTask
            {
                Title = request.Title,
                Description = request.Description,
                Priority = request.Priority,
                DueDate = request.DueDate,
                EstimatedHours = request.EstimatedHours,
                ActualHours = request.ActualHours,
                Tags = request.Tags,
                ProjectId = request.ProjectId,
                AssigneeId = assignee?.Id
            };

            await _taskRepository.AddTaskAsync(task);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return task.Id;
        }
    }
}
