using Carter;
using FluentValidation;
using Hotel.Api.Database;
using Hotel.Api.PipelineBehaviors;
using Microsoft.EntityFrameworkCore;

internal class Program
{
    private static void Main(string[] args)
    {
        // CONFIGURE SERVICE
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<ApplicationDbContext>(o =>
            o.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

        builder.Services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(Program).Assembly);
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });
        
        builder.Services.AddCarter();
        
        builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

        // CONFITURE PIPELINE
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            ApplyMigrations(app);
        }

        app.MapCarter();

        app.UseHttpsRedirection();

        app.Run();
    }

    private static void ApplyMigrations(WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Database.Migrate();
    }
}