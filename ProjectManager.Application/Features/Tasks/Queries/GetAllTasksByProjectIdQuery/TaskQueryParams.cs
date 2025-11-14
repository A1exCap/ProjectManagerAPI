using ProjectManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.Tasks.Queries.GetAllTasksByProjectIdQuery
{
    public class TaskQueryParams
    {
        public string? Title { get; set; }
        public string? AssigneeEmail { get; set; }
        public ProjectTaskStatus? Status { get; set; }
        public ProjectTaskPriority? Priority { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string? SortBy { get; set; } 
        public bool SortDescending { get; set; } = false;
    }
}
