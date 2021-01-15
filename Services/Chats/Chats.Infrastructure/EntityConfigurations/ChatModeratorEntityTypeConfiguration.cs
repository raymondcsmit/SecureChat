using System;
using System.Collections.Generic;
using System.Text;
using Chats.Domain.AggregateModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chats.Infrastructure.EntityConfigurations
{
    public class ChatModeratorEntityTypeConfiguration : IEntityTypeConfiguration<ChatModerator>
    {
        public void Configure(EntityTypeBuilder<ChatModerator> builder)
        {
            builder.ToTable("ChatModerators");

            builder.HasKey(c => new {c.ChatId, c.UserId});

            builder.Ignore(c => c.Id)
                .Ignore(c => c.DomainEvents)
                .Ignore(c => c.IsTransient);

            builder.HasOne(c => c.Chat)
                .WithMany(c => c.ChatModerators)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.User)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
