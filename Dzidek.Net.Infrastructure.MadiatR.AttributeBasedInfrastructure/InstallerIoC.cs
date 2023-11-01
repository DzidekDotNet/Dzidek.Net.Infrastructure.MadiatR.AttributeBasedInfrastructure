using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Dzidek.Net.Infrastructure.MadiatR.AttributeBasedInfrastructure;

public static class InstallerIoC
{
  public static IServiceCollection AddMediatRAttributeBasedInfrastructure(this IServiceCollection services)
  {
    return services
      .AddScoped(typeof(IPipelineBehavior<,>), typeof(AttributeBasedInfrastructureBehaviour<,>));
  }
}
