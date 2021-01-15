using System;
using System.Collections.Generic;
using System.Text;
using Chats.Domain.AggregateModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chats.Infrastructure.EntityConfigurations
{
    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(m => m.Id);

            builder.Ignore(m => m.DomainEvents)
                .Ignore(m => m.IsTransient);

            builder.Property(u => u.UserName)
                .IsRequired();
        }
    }
}
