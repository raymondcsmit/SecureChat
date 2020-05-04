using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Session.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Session.API.EntityConfigurations
{
    public class ChatEventEntityTypeConfiguration : IEntityTypeConfiguration<ChatEvent>
    {
        public void Configure(EntityTypeBuilder<ChatEvent> builder)
        {
            builder.ToTable("Event");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.EventType)
                .HasColumnType("int")
                .IsRequired();

            builder.Property(m => m.CreatedAt)
                .IsRequired();

            builder.Property(m => m.ModifiedAt)
                .IsRequired();
        }
    }
}
