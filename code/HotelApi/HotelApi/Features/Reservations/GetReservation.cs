using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Hotel.Api.Database;
using Hotel.Api.Shared;
using Hotel.Api.Entities;

namespace Hotel.Api.Features.Reservations;

public class GetReservationEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/reservations/{id}", async (Guid id, ISender sender) =>
        {
            var query = new GetReservation.Query { Id = id };

            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.NotFound(result.Error);
            }

            return Results.Ok(result.Value);
        })
        .WithOpenApi();
    }
}

public static class GetReservation
{
    public class Query : IRequest<Result<Response>>
    {
        public Guid Id { get; set; }
    }

    internal sealed class Handler : IRequestHandler<Query, Result<Response>>
    {
        private readonly ApplicationDbContext _dbContext;

        public Handler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var reservationResponse = await _dbContext
                .Reservations
                .AsNoTracking()
                .Where(reservation => reservation.Id == request.Id)
                .Select(reservation => new Response
                {
                    Id = reservation.Id,
                    RoomId = reservation.RoomId,
                    GuestFirstName = reservation.GuestFirstName,
                    GuestLastName = reservation.GuestLastName,
                    CheckInDate = reservation.CheckInDate,
                    CheckOutDate = reservation.CheckOutDate
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (reservationResponse is null)
            {
                return Result.Failure<Response>(new Error(
                    "GetReservation.Null",
                    "The reservation with the specified ID was not found"));
            }

            return reservationResponse;
        }
    }

    public class Response
    {
        public Guid Id { get; set; }
        public int RoomId { get; set; }
        public string GuestFirstName { get; set; }
        public string GuestLastName { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
    }
}