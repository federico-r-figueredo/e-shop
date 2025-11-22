using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eShop.Services.Basket.API.Model {
    public class BasketItem : IValidatableObject {
        public string ID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal OldUnitPrice { get; set; }
        public int Quantity { get; set; }
        public string PictureURL { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            List<ValidationResult> results = new List<ValidationResult>();

            if (this.Quantity < 1) {
                results.Add(new ValidationResult("Invalid number of units", new[] { "Quantity" }));
            }

            return results;
        }
    }
}