using System.Collections.Generic;

namespace eShop.Services.Basket.API.Infrastructure.Options {
    internal class FailingOptions {
        public FailingOptions() {
            this.EndpointPaths = new List<string>();
            this.NotFilteredPaths = new List<string>();
        }

        public string ConfigPath { get { return "/Failing"; } }
        public List<string> EndpointPaths { get; set; }
        public List<string> NotFilteredPaths { get; set; }
    }
}