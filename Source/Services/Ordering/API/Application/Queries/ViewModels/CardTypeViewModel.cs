namespace EShop.Services.Ordering.API.Application.Queries.ViewModels {
    public class CardTypeViewModel {
        private readonly int id;
        private readonly string name;

        public CardTypeViewModel(int id, string name) {
            this.id = id;
            this.name = name;
        }

        public int ID {
            get { return this.id; }
        }
        public string Name {
            get { return this.name; }
        }
    }
}
