# Integration Events Flow

Catalog::ProductPriceChangedIntegrationEvent
    -> Basket::ProductPriceChangedIntegrationEventHandler

Basket::UserCheckoutAcceptedIntegrationEvent
    -> Ordering::UserCheckoutAcceptedIntegrationEventHandler
        -> Ordering::CreateOrderCommand
            -> Ordering::CreateOrderCommandHandler
                -> Ordering::OrderStartedDomainEvent
                    -> Ordering::SendEmailToCustomerWhenOrderStartedDomainEventHandler
                    -> Ordering::ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler
                        -> Ordering::BuyerAndPaymentMethodVerifiedDomainEvent
                            -> Ordering::UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler
                        -> Ordering::OrderStatusChangedToSubmittedIntegrationEvent **
                -> Ordering::OrderStartedIntegrationEvent
                    -> Basket::OrderStartedIntegrationEventHandler

Ordering::GracePeriodManagerService
    -> Ordering::GracePeriodConfirmedIntegrationEvent