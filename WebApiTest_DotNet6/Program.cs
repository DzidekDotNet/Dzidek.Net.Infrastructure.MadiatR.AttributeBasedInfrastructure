using Dzidek.Net.Infrastructure.MadiatR.AttributeBasedInfrastructure;
using WebApiTest_DotNet6;
using WebApiTest_DotNet6.RetryPolicy;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services
  .AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>())
  .AddMediatRAttributeBasedInfrastructure()
  .AddSingleton<TestService>()
  .AddTransient<IRetryPolicyService, RetryPolicyService>();

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

app.Run();
