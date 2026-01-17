using System.Reflection;
using Autofac;
using eShop.Services.Ordering.API.Application.Behaviours;
using eShop.Services.Ordering.API.Application.Commands;
using eShop.Services.Ordering.API.Application.DomainEventHandlers.OrderStartedEvent;
using eShop.Services.Ordering.API.Application.Validators;
using FluentValidation;
using MediatR;

namespace eShop.Services.Ordering.API.Infrastructure.AutofacModules {
    public class MediatorModule : Autofac.Module {
        protected override void Load(ContainerBuilder builder) {
            // Register MediatR's core services (IMediator, internal handler wrappers, etc.)
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly)
                .AsImplementedInterfaces();

            // Register all the Command classes (they implement IRequestHandler) in assembly
            // holding the Commands
            builder.RegisterAssemblyTypes(typeof(CreateOrderCommand).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IRequestHandler<,>));

            // Register the DomainEventHandler classes (they implement INotificationHandler)
            // in assembly holding the Domain events.
            builder.RegisterAssemblyTypes(typeof(ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(INotificationHandler<>));

            // Register the Command's Validators (validators based on FluentValidation library)
            builder.RegisterAssemblyTypes(typeof(CreateOrderCommandValidator).GetTypeInfo().Assembly)
                .Where(type => type.IsClosedTypeOf(typeof(IValidator<>)))
                .AsImplementedInterfaces();

            builder.Register<ServiceFactory>(context => {
                IComponentContext componentContext = context.Resolve<IComponentContext>();
                return type => {
                    object obj;
                    return componentContext.TryResolve(type, out obj) ? obj : null;
                };
            });

            builder.RegisterGeneric(typeof(LoggingBehaviour<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(ValidatorBehaviour<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(TransactionBehaviour<,>)).As(typeof(IPipelineBehavior<,>));
        }
    }
}