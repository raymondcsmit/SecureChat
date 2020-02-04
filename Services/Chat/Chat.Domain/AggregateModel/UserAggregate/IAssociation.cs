using System;
using System.Collections.Generic;
using System.Text;

namespace Chat.Domain.AggregateModel.UserAggregate
{
    public interface IAssociation
    {
        string RequesterId { get; }

        string RequesteeId { get; }
    }
}
