using Chats.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Chats.Domain.AggregateModel
{
    public interface IChatRepository : IRepository<Chat>
    {
    }
}
