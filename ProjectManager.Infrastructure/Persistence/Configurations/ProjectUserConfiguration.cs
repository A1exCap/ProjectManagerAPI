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
    public class ProjectUserConfiguration : IEntityTypeConfiguration<ProjectUser>
    {
        public void Configure(EntityTypeBuilder<ProjectUser> builder)
        {
            builder.HasKey(pu => pu.Id);

            builder.Property(pu => pu.Id)
                .ValueGeneratedOnAdd();

            builder.HasOne(pu => pu.Project)
              .WithMany(p => p.ProjectUsers)
              .HasForeignKey(pu => pu.ProjectId)
              .OnDelete(DeleteBehavior.Cascade);

            builder.Property(pu => pu.Role)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(pu => pu.JoinedAt)
                .IsRequired();

            builder.Property(pu => pu.HourlyRate)
                .HasPrecision(18, 2);

            builder.HasOne(pu => pu.User)
              .WithMany(u => u.ProjectUsers)
              .HasForeignKey(pu => pu.UserId)
              .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(pu => new { pu.UserId, pu.ProjectId })
                .IsUnique();
        }
    }
}
