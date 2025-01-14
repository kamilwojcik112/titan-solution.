using Fronius.Onboarding.Filtering;
using Fronius.Onboarding.Logging;
using Fronius.Onboarding.Policies;
using Fronius.Onboarding.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;

namespace Fronius.Onboarding;

public static class DependencyInjection
{
    public static IServiceCollection BuildDependencyInjectionContainer()
    {
        var services = new ServiceCollection();

        return services
            .AddTransient<IControlRepository, InMemoryControlRepository>()
            .AddTransient<IControlFilter, ControlFilter>()
            .AddSingleton<IControlAddingPolicy, ControlAddingPolicy>()
            .AddSingleton<ILogger, Logger>();
    }
}