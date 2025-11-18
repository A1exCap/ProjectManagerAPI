using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.ProjectDocuments.Queries.GetAllDocumentsByProjectIdQuery
{
    public class DocumentQueryParams
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
