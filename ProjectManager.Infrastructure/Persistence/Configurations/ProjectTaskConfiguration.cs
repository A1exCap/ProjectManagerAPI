using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Infrastructure.Persistence.Configurations
{
    public class ProjectTaskConfiguration : IEntityTypeConfiguration<ProjectTask>
    {
        public void Configure(EntityTypeBuilder<ProjectTask> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
              .ValueGeneratedOnAdd();

            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Description)
                .IsRequired(false)
                .HasMaxLength(2000);

            builder.Property(t => t.Priority)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(20);

            builder.Property(t => t.Status)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(20);

            builder.Property(t => t.CreatedAt)
                .IsRequired();

            builder.Property(t => t.DueDate)
                .IsRequired(false);

            builder.Property(t => t.CompletedAt)
                .IsRequired(false);

            builder.Property(t => t.ReminderSentAt)
                .IsRequired(false);

            builder.Property(t => t.EstimatedHours)
                .HasDefaultValue(0);

            builder.Property(t => t.ActualHours)
              .HasDefaultValue(0);

            builder.Property(t => t.Tags)
                .IsRequired(false)
                .HasMaxLength(500);

            builder.HasOne(t=>t.Creator)
                .WithMany(u=>u.CreatedTasks)
                .HasForeignKey(t=>t.CreatorId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(t => t.Assignee)
                .WithMany(u => u.AssignedTasks)
                .HasForeignKey(t=>t.AssigneeId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            
            builder.HasMany(t => t.Comments)
                .WithOne(c => c.ProjectTask)
                .HasForeignKey(c => c.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(t => t.Attachments)
                .WithOne(a => a.ProjectTask)
                .HasForeignKey(a => a.ProjectTaskId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(t => new { t.ProjectId, t.Priority, t.Status});
        }
    }
}
