
using EShop.Services.Ordering.Domain.Shared;
using MediatR;

namespace EShop.Services.Ordering.API.Application.Commands {
    internal interface ICommand : IRequest<Result> {

    }

    internal interface ICommand<TResponse> : IRequest<Result<TResponse>> {

    }
}