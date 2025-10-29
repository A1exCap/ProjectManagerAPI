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
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(n => n.Id)
                   .ValueGeneratedOnAdd();

            builder.Property(c => c.Content)
                   .IsRequired()
                   .HasMaxLength(4000);

            builder.Property(c => c.CreatedAt)
                     .IsRequired();

            builder.Property(c => c.UpdatedAt)
                     .IsRequired(false);

            builder.HasOne(c => c.ProjectTask)
                     .WithMany(pt => pt.Comments)
                     .HasForeignKey(c => c.ProjectTaskId)
                     .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.Author)
                        .WithMany(u => u.Comments)
                        .HasForeignKey(c => c.AuthorId)
                        .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(c => c.ProjectTaskId);
        }
    }
}
