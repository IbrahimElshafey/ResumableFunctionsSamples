using System.Diagnostics;
using ClientOnboarding.Services;
using ResumableFunctions.AspNetService;
using ResumableFunctions.Handler.InOuts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddControllers()
    .AddResumableFunctions(new SqlServerResumableFunctionsSettings(null, "ClientOnboardingWaits"));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IClientOnboardingService, ClientOnboardingService>();
//builder.Services.AddScoped<ClientOnboardingWorkflow>();

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
try
{
    app.Run();
}
catch (Exception ex)
{
    Debug.Write(ex);
    Console.WriteLine(ex);
    Console.WriteLine(ex.Message);
    Console.WriteLine(ex.StackTrace);
	throw;
}
