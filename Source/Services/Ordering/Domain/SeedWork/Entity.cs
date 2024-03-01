using System;
using System.Collections.Generic;
using MediatR;

namespace EShop.Services.Ordering.Domain.SeedWork {
    public abstract class Entity {
        private int id;
        private int? requestedHashCode;
        private List<INotification> domainEvents;

        public virtual int ID {
            get {
                return this.id;
            }
            protected set {
                this.id = value;
            }
        }

        public IReadOnlyCollection<INotification> DomainEvents {
            get {
                return this.domainEvents?.AsReadOnly();
            }
        }

        public void AddDomainEvent(INotification domainEvent) {
            this.domainEvents = this.domainEvents ?? new List<INotification>();
            this.domainEvents.Add(domainEvent);
        }

        public void RemoveDomainEvent(INotification domainEvent) {
            this.domainEvents?.Remove(domainEvent);
        }

        public void ClearDomainEvents() {
            this.domainEvents?.Clear();
        }

        public bool IsTransient() {
            return this.id == default(Int32);
        }

        public override bool Equals(object obj) {
            if (obj == null || !(obj is Entity)) {
                return false;
            }

            if (Object.ReferenceEquals(this, obj)) {
                return true;
            }

            if (this.GetType() != obj.GetType()) {
                return false;
            }

            Entity item = (Entity)obj;

            if (item.IsTransient() || this.IsTransient()) {
                return false;
            }

            return item.ID == this.ID;
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
            if (Object.Equals(left, null)) {
                return Object.Equals(right, null);
            }

            return left.Equals(right);
        }

        public static bool operator !=(Entity left, Entity right) {
            return !(left == right);
        }
    }
}