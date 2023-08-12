using ResumableFunctions.AspNetService;
using ResumableFunctions.Handler.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddControllers()
    .AddResumableFunctions(
        new SqlServerResumableFunctionsSettings()
        .SetCurrentServiceUrl("https://localhost:7099/")
        .SetDllsToScan("ReferenceLibrary"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();
app.UseResumableFunctions();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();

