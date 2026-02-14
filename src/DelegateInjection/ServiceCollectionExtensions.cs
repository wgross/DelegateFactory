using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DelegateInjection;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds the services required for delegate injection to the <see cref="IServiceCollection"/>.
        /// </summary>
        public IServiceCollection AddDelegateInjection()
            => services.AddSingleton<DelegateInjector>().AddSingleton<DelegateInvoker>();
    }
}
