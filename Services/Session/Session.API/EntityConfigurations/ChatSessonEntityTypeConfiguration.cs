using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Session.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Session.API.EntityConfigurations
{
    public class ChatSessonEntityTypeConfiguration : IEntityTypeConfiguration<ChatSession>
    {
        public void Configure(EntityTypeBuilder<ChatSession> builder)
        {
            builder.ToTable("ChatSession");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .IsRequired();

            builder.Property(m => m.UserId)
                .IsRequired();

            builder.Property(m => m.CreatedAt)
                .IsRequired();

            builder.Property(m => m.ModifiedAt)
                .HasColumnName("LastActivity")
                .IsRequired();
        }
    }
}
