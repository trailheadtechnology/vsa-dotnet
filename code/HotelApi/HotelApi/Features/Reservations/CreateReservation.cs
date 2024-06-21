using Carter;
using FluentValidation;
using Mapster;
using MediatR;
using Hotel.Api.Database;
using Hotel.Api.Entities;
using Hotel.Api.Shared;

namespace Hotel.Api.Features.Reservations;

public class CreateReservationEndpoint : ICarterModule
{
    public class Request
    {
        public Guid Id { get; set; }
        public int RoomId { get; set; }
        public string GuestFirstName { get; set; }
        public string GuestLastName { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/reservations", async (Request request, ISender sender) =>
        {
            var command = request.Adapt<CreateReservation.Command>();

            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Ok(result.Value);
        })
        .WithOpenApi();
    }
}

public static class CreateReservation
{
    public class Command : IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
        public int RoomId { get; set; }
        public string GuestFirstName { get; set; }
        public string GuestLastName { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
    }

public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(r => r.RoomId).NotEmpty().GreaterThanOrEqualTo(1);
        RuleFor(r => r.GuestFirstName).NotEmpty();
        RuleFor(r => r.GuestLastName).NotEmpty();
        RuleFor(r => r.CheckInDate)
            .NotEmpty()
            .Must(date => date.Date >= DateTime.Now.Date)
                .WithMessage("Check-in date must be today or a future date.");
        RuleFor(r => r.CheckOutDate)
            .NotEmpty()
            .Must((model, checkOutDate) => checkOutDate.Date > model.CheckInDate.Date)
                .WithMessage("Check-out date must be after the check-in date.");
    }
}

    internal sealed class Handler : IRequestHandler<Command, Result<Guid>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IValidator<Command> _validator;

        public Handler(ApplicationDbContext dbContext, IValidator<Command> validator)
        {
            _dbContext = dbContext;
            _validator = validator;
        }

        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return Result.Failure<Guid>(new Error(
                    "CreateReservation.Validation",
                    validationResult.ToString()));
            }

            var reservation = new Reservation
            {
                Id = Guid.NewGuid(),
                RoomId = request.RoomId,
                GuestFirstName = request.GuestFirstName,
                GuestLastName = request.GuestLastName,
                CheckInDate = request.CheckInDate.Date,
                CheckOutDate = request.CheckOutDate.Date
            };

            _dbContext.Add(reservation);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return reservation.Id;
        }
    }
}
