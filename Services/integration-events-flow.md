# Integration Events Flow

Catalog::ProductPriceChangedIntegrationEvent
    -> Basket::ProductPriceChangedIntegrationEventHandler

Basket::BasketController::CheckoutBasketAsync()
    Basket::UserCheckoutAcceptedIntegrationEvent
        -> Ordering::UserCheckoutAcceptedIntegrationEventHandler:Handle()
            -> Ordering::CreateOrderCommand
                -> Ordering::CreateOrderCommandHandler::Handle()
                    -> Order::Order()
                        -> Ordering::OrderStartedDomainEvent
                            -> Ordering::SendEmailToCustomerWhenOrderStartedDomainEventHandler::Handle()
                            -> Ordering::ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler::Handle()
                                -> Buyer::VerifyOrAddPaymentMethod()
                                    -> Ordering::BuyerAndPaymentMethodVerifiedDomainEvent
                                        -> Ordering::UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler::Handle()
                                -> Ordering::OrderStatusChangedToSubmittedIntegrationEvent **
                                    -> Ordering.SignalrHub::OrderStatusChangedToSubmittedIntegrationEventHandler::Handle()
                                        -> Ordering.SignalrHub::UpdatedOrderState
                                            -> UIClients::UpdatedOrderStateHandler::Handle()
                    -> Ordering::OrderStartedIntegrationEvent
                        -> Basket::OrderStartedIntegrationEventHandler::Handle()

Ordering::GracePeriodManagerService
    -> Ordering::GracePeriodConfirmedIntegrationEvent