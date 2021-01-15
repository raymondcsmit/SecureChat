using Chats.Domain.AggregateModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chats.Infrastructure.EntityConfigurations
{
    public class ChatMembershipEntityTypeConfiguration : IEntityTypeConfiguration<ChatMembership>
    {
        public void Configure(EntityTypeBuilder<ChatMembership> builder)
        {
            builder.ToTable("ChatMemberships");

            builder.HasKey(c => new {c.ChatId, c.UserId});

            builder.Ignore(c => c.Id)
                .Ignore(c => c.DomainEvents)
                .Ignore(c => c.IsTransient);

            builder.HasOne(c => c.Chat)
                .WithMany(c => c.ChatMemberships)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.User)
                .WithMany(c => c.ChatMemberships)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.Property(c => c.ModifiedAt)
                .IsRequired();
        }
    }
}
