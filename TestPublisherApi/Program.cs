using ResumableFunctions.Publisher.Helpers;
using ResumableFunctions.Publisher.Implementation;

namespace TestPublisherApi;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddResumableFunctionsPublisher(
            new PublisherSettings(new() {
                { "TestApi1", "https://localhost:7140/" },
                { "TestApi2", "https://localhost:7099/" },
            },
            checkFailedRequestEvery: TimeSpan.FromSeconds(10)));

        var app = builder.Build();
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
        app.UseResumableFunctionsPublisher();
        app.Run();
    }
}