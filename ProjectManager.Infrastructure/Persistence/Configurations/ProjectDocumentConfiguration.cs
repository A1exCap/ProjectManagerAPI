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
    public class ProjectDocumentConfiguration : IEntityTypeConfiguration<ProjectDocument>
    {
        public void Configure(EntityTypeBuilder<ProjectDocument> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Id)
                .ValueGeneratedOnAdd();

            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(d => d.FilePath)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(d => d.FileSize)
                .IsRequired();

            builder.Property(d => d.ContentType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.UploadedAt)
                .IsRequired();

            builder.HasOne(d => d.UploadedBy)
                .WithMany(u => u.UploadedDocuments)  
                .HasForeignKey(d => d.UploadedById)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(d => d.ProjectId);
            builder.HasIndex(d => d.UploadedById);
        }
    }
}
