using System.Collections.Generic;
using eShop.Services.Catalog.API.Model;

namespace eShop.Services.Catalog.API.ViewModel {
    public class PaginatedItemsViewModel {
        private int pageIndex;
        private int pageSize;
        private long count;
        private List<CatalogItem> data;

        public int PageIndex {
            get { return this.pageIndex; }
        }

        public int PageSize {
            get { return this.pageSize; }
        }

        public long Count {
            get { return this.count; }
        }

        public List<CatalogItem> Data {
            get { return this.data; }
        }

        public PaginatedItemsViewModel(int pageIndex, int pageSize, long count,
            List<CatalogItem> data) {
            this.pageIndex = pageIndex;
            this.pageSize = pageSize;
            this.count = count;
            this.data = data;
        }
    }
}