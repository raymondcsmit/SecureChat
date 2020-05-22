using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Session.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Session.API.EntityConfigurations
{
    public class ChatUserEventEntityTypeConfiguration : IEntityTypeConfiguration<ChatUserEvent>
    {
        public void Configure(EntityTypeBuilder<ChatUserEvent> builder)
        {
            builder.ToTable("ChatUserEvent");

            builder.HasKey(m => new { m.UserId, m.EventId });

            builder.Property(m => m.UserId);

            builder.Property(m => m.EventId);

            builder.HasOne(m => m.User)
                   .WithMany(m => m.EventsLink)
                   .HasForeignKey(m => m.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(m => m.Event)
                   .WithOne(m => m.UserLink)
                   .HasForeignKey<ChatUserEvent>(m => m.EventId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
