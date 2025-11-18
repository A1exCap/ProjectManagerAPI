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
    public class TaskAttachmentConfiguration : IEntityTypeConfiguration<TaskAttachment>
    {
        public void Configure(EntityTypeBuilder<TaskAttachment> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .ValueGeneratedOnAdd();

            builder.Property(a => a.FileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(a => a.ContentType)
               .IsRequired()
               .HasMaxLength(100);

            builder.Property(a => a.FileSize)
               .IsRequired();

            builder.Property(a => a.StoredFileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(a => a.FilePath)
                .IsRequired()
                .HasMaxLength(500);  

            builder.Property(a => a.UploadedAt)
                .IsRequired();

            builder.HasOne(a => a.UploadedBy)
                .WithMany(u => u.UploadedTaskAttachments)
                .HasForeignKey(a => a.UploadedById)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
