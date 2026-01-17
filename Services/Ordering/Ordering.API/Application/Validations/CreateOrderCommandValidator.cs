using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using FluentValidation;
using eShop.Services.Ordering.API.Application.Commands;
using eShop.Services.Ordering.API.Application.DTOs;

namespace eShop.Services.Ordering.API.Application.Validators {
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand> {
        public CreateOrderCommandValidator(ILogger<CreateOrderCommandValidator> logger) {
            base.RuleFor(command => command.Street).NotEmpty();
            base.RuleFor(command => command.City).NotEmpty();
            base.RuleFor(command => command.State).NotEmpty();
            base.RuleFor(command => command.Country).NotEmpty();
            base.RuleFor(command => command.ZipCode).NotEmpty();
            base.RuleFor(command => command.CardNumber).NotEmpty().Length(12, 19);
            base.RuleFor(command => command.CardHolderName).NotEmpty();
            base.RuleFor(command => command.CardExpiration)
                .NotEmpty()
                .Must(BeValidExpirationDate)
                .WithMessage("Please specify a valid card expiration date.");
            base.RuleFor(command => command.CardSecurityNumber).NotEmpty().Length(3);
            base.RuleFor(command => command.CardTypeID).NotEmpty();
            base.RuleFor(command => command.OrderItems)
                .Must(ContainOrderItems)
                .WithMessage("No order items found.");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }

        private bool BeValidExpirationDate(DateTime dateTime) {
            return dateTime >= DateTime.Now;
        }

        private bool ContainOrderItems(IEnumerable<OrderItemDTO> orderItems) {
            return orderItems.Any();
        }
    }
}