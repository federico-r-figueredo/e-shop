
namespace eShop.Services.Basket.API.Settings {
    public class BasketSettings {
        public string EventBusConnection { get; set; }
        public string EventBusUserName { get; set; }
        public string EventBusPassword { get; set; }
        public string EventBusRetryCount { get; set; }
        public string SubscriptionClientName { get; set; }
    }
}