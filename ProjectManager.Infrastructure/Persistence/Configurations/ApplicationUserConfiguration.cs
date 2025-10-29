using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManager.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Infrastructure.Persistence.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Ignore(u => u.OwnedProjects);
            builder.Ignore(u => u.UploadedDocuments);
            builder.Ignore(u => u.Comments);
            builder.Ignore(u => u.Notifications);
            builder.Ignore(u => u.AssignedTasks);
            builder.Ignore(u => u.ProjectUsers);
            builder.Ignore(u => u.UploadedTaskAttachments);

            builder.Property(u => u.FullName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.CreatedAt)
                .IsRequired();
        }
    }
}
