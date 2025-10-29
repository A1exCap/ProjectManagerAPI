﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Infrastructure.Persistence.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Id)
                .ValueGeneratedOnAdd();

            builder.HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(n => n.NotificationType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(n => n.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(n => n.Message)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(n => n.EntityType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(n => n.IsRead)
               .IsRequired()
               .HasDefaultValue(false);

            builder.Property(n => n.RelatedEntityId)
                .IsRequired();

            builder.Property(n => n.CreatedAt)
                .IsRequired();

            builder.HasIndex(n => n.UserId);
            builder.HasIndex(n => new { n.UserId, n.IsRead });
        }
    }
}
