using System.Reflection;
using Autofac;
using eShop.BuildingBlocks.EventBus.Abstractions;
using eShop.Services.Ordering.API.Application.Commands;
using eShop.Services.Ordering.API.Application.Queries;
using eShop.Services.Ordering.Domain.Model.BuyerAggregate;
using eShop.Services.Ordering.Domain.Model.OrderAggregate;
using eShop.Services.Ordering.Infrastructure.Idempotency;
using eShop.Services.Ordering.Infrastructure.Repositories;

namespace eShop.Services.Ordering.API.Infrastructure.AutofacModules {
    internal class ApplicationModule : Autofac.Module {
        public string queriesConnectionString;

        public ApplicationModule(string queriesConnectionString) {
            this.queriesConnectionString = queriesConnectionString;
        }

        protected override void Load(ContainerBuilder builder) {
            builder.Register(x => new OrderQueries(this.queriesConnectionString))
                .As<IOrderQueries>()
                .InstancePerLifetimeScope();

            builder.RegisterType<BuyerRepository>()
                .As<IBuyerRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<OrderRepository>()
                .As<IOrderRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<RequestManager>()
                .As<IRequestManager>()
                .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(typeof(CreateOrderCommandHandler).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IIntegrationEventHandler<>));
        }
    }
}