using System;
using System.Collections.Generic;
using MediatR;

namespace eShop.Services.Ordering.Domain.SeedWork {
    public abstract class Entity {
        private int? requestedHashCode;
        protected int id;
        private List<INotification> domainEvents;

        public Entity() {
            this.domainEvents = new List<INotification>();
        }

        public virtual int ID {
            get { return this.id; }
            // This private setter is required so EF Core design time tools won't fail with
            // "No backing field could be found for property '<EntityChild>.ID' and 
            // the property does not have a setter".
            private set { this.id = value; }
        }

        public IReadOnlyCollection<INotification> DomainEvents {
            get { return this.domainEvents?.AsReadOnly(); }
        }

        public void AddDomainEvent(INotification domainEvent) {
            this.domainEvents.Add(domainEvent);
        }

        public void RemoveDomainEvent(INotification domainEvent) {
            this.domainEvents.Remove(domainEvent);
        }

        public void ClearDomainEvents() {
            this.domainEvents.Clear();
        }

        public bool IsTransient() {
            return this.id == default(Int32);
        }

        public override bool Equals(object obj) {
            if (obj == null || this.GetType() != obj.GetType()) {
                return false;
            }

            if (Object.ReferenceEquals(this, obj)) {
                return true;
            }

            Entity item = (Entity)obj;

            if (this.IsTransient() || item.IsTransient()) {
                return false;
            }

            return this.id == item.id;
        }

        public override int GetHashCode() {
            if (!this.IsTransient()) {
                if (!this.requestedHashCode.HasValue) {
                    this.requestedHashCode = this.id.GetHashCode() ^ 31; // XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)
                }

                return this.requestedHashCode.Value;
            }

            return base.GetHashCode();
        }

        public static bool operator ==(Entity left, Entity right) {
            return Object.Equals(left, null)
                ? Object.Equals(right, null)
                : left.Equals(right);
        }

        public static bool operator !=(Entity left, Entity right) {
            return !(left == right);
        }
    }
}