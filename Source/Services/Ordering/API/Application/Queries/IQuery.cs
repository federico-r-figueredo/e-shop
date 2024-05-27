using EShop.Services.Ordering.Domain.Shared;
using MediatR;

namespace EShop.Services.Ordering.API.Application.Queries {
    internal interface IQuery<TResponse> : IRequest<Result<TResponse>> {

    }
}