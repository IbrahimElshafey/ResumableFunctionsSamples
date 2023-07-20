using RequestApproval.Controllers;
using ResumableFunctions.AspNetService;
using ResumableFunctions.Handler.InOuts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddControllers()
    .AddResumableFunctions(
    new SqlServerResumableFunctionsSettings(null, "RequestApprovalWaitsTest")
    .SetCurrentServiceUrl("https://localhost:7003"));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IRequestApprovalService, RequestApprovalService>();
builder.Services.AddScoped<RequestApprovalWorkflow>();

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
