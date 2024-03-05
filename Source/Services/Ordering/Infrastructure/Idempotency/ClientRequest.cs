
using System;

namespace Ordering.Infrastructure.Idempotency {
    public class ClientRequest {
        private readonly Guid id;
        private readonly string name;
        private readonly DateTime requestDateTime;

        public ClientRequest(Guid id, string name, DateTime requestDateTime) {
            this.id = id;
            this.name = name;
            this.requestDateTime = requestDateTime;
        }

        public Guid ID {
            get { return this.id; }
        }

        public string Name {
            get { return this.name; }
        }

        public DateTime RequestDateTime {
            get { return this.requestDateTime; }
        }
    }
}