using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Chats.Domain.AggregateModel;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chats.Infrastructure.EntityConfigurations
{
    public class ChatEntityTypeConfiguration : IEntityTypeConfiguration<Chat>
    {
        public void Configure(EntityTypeBuilder<Chat> builder)
        {
            builder.ToTable("Chats");

            builder.HasKey(c => c.Id);

            builder.Ignore(c => c.DomainEvents)
                .Ignore(c => c.IsTransient);

            builder.HasOne(c => c.Owner)
                .WithMany()
                .HasForeignKey(c => c.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Messages)
                .WithOne(m => m.Chat)
                .HasForeignKey(m => m.ChatId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Metadata.FindNavigation(nameof(Chat.ChatMemberships))
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.Metadata.FindNavigation(nameof(Chat.ChatModerators))
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.Metadata.FindNavigation(nameof(Chat.Messages))
                .SetPropertyAccessMode(PropertyAccessMode.Field);
                
            builder.Property(c => c.Id)
                .ValueGeneratedOnAdd();

            builder.Property(c => c.Name)
                .IsRequired();

            builder.Property(c => c.OwnerId)
                .IsRequired();

            builder.Property(c => c.Capacity)
                .IsRequired();
        }
    }
}
