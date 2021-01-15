using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Chats.Domain.AggregateModel;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chats.Infrastructure.EntityConfigurations
{
    public class MessageEntityTypeConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("Messages");

            builder.HasKey(m => m.Id);

            builder.Ignore(m => m.DomainEvents)
                .Ignore(m => m.IsTransient);

            builder.Property(m => m.Id)
                .ValueGeneratedOnAdd();

            builder.Property(m => m.ChatId)
                .IsRequired();

            builder.Property(m => m.Content)
                .IsRequired();

            builder.Property(m => m.UserId)
                .IsRequired();

            builder.Property(m => m.CreatedAt)
                .IsRequired();

            builder.Property(m => m.ModifiedAt)
                .IsRequired();
        }
    }
}
