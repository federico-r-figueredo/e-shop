using System;

namespace eShop.Services.Ordering.Infrastructure.Idempotency {
    public class ClientRequest {
        public Guid GUID { get; set; }
        public string Name { get; set; }
        public DateTime DateTime { get; set; }
    }
}