﻿using Microsoft.Extensions.DependencyInjection;
using NavigateCore.Commands;
using NavigateCore.Parsers;

namespace NavigateCore
{
    public static class ServicesConfigure
    {
        public static IServiceCollection AddNavigateServices(this IServiceCollection services)
        {
            services.AddTransient<INavigationBarParser, NavigationBarParser>();
            services.AddTransient<IToDorfCommand, ToDorfCommand>();
            return services;
        }
    }
}