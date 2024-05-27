
using EShop.Services.Ordering.Domain.Shared;
using MediatR;

namespace EShop.Services.Ordering.API.Application.Queries {
    internal interface IQueryHandler<TQuery, TResponse>
        : IRequestHandler<TQuery, Result<TResponse>> where TQuery : IQuery<TResponse> {

    }
}