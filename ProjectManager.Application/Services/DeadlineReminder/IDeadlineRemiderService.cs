using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Services.DeadlineReminder
{
    public interface IDeadlineReminderService
    {
        Task CheckDeadlinesAsync();
    }
}
