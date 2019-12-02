using System;
using System.Collections.Generic;
using System.Text;
using Associations.Domain.Events;
using Associations.Domain.Exceptions;
using Associations.Domain.SeedWork;

namespace Associations.Domain.AggregateModel.UserAggregate
{
    public class User : Entity, IAggregateRoot
    {
        public string Username { get; protected set; }

        public int Age { get; protected set; }

        public string Sex { get; protected set; }

        public string Location { get; protected set; }

        public Session Session { get; protected set; }

        protected User()
        {
            
        }

        public void EndSession()
        {
            Session = null;
            AddDomainEvent(new SessionEndedDomainEvent(Id));
        }

        public void RefreshSession()
        {
            if (Session == null)
            {
                throw new AssociationsDomainException("Could not refresh session",
                    new[] { "No session exists" });
            }

            Session.Refresh();
        }


    }
}
