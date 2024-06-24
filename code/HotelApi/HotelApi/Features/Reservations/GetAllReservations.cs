using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Hotel.Api.Database;
using Hotel.Api.Shared;
using Hotel.Api.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Api.Features.Reservations;

public class GetAllReservationsEndpoint : ICarterModule
{
    public class Request
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/reservations", async (ISender sender, [AsParameters]Request request) =>
        {
            var query = new GetAllReservations.Query
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

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

public static class GetAllReservations
{
    public class Query : IRequest<Result<Response>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    internal sealed class Handler : IRequestHandler<Query, Result<Response>>
    {
        private readonly ApplicationDbContext _dbContext;

        public Handler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<Response>> Handle(Query query, CancellationToken cancellationToken)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            int pageNumber = query.PageNumber > 0 ? query.PageNumber : 1;
            int pageSize = query.PageSize > 0 ? query.PageSize : 10;

            var q = _dbContext.Reservations.AsQueryable();

            int totalCount = await q.CountAsync();
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var pagedEntities = await q.Skip((pageNumber - 1) * pageSize)
                                           .Take(pageSize)
                                           .ToListAsync();

            return new Response(pagedEntities, pageNumber, pageSize, totalCount, totalPages);
        }
    }

    public class Response
    {
        public IList<Reservation> Items { get; private set; }
        public int PageNumber { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages { get; private set; }

        public Response(IList<Reservation> items, int pageNumber, int pageSize, int totalCount, int totalPages)
        {
            Items = items;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
            TotalPages = totalPages;
        }
    }
}