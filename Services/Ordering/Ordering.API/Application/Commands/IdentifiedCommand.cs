using System;
using MediatR;

namespace eShop.Services.Ordering.API.Application.Commands {
    public class IdentifiedCommand<TCommand, TResponse>
        : IRequest<TResponse> where TCommand : IRequest<TResponse> {
        public IdentifiedCommand(TCommand command, Guid iD) {
            Command = command;
            ID = iD;
        }

        public TCommand Command { get; }
        public Guid ID { get; }
    }
}