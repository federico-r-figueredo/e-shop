
using EShop.Services.Ordering.Domain.Shared;
using MediatR;

namespace EShop.Services.Ordering.API.Application.Commands {
    internal interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Result>
        where TCommand : ICommand {

    }

    internal interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
        where TCommand : ICommand<TResponse> {

    }
}