using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace Users.API.Client.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUsersApiClient(this IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.Get<UsersApiClientConfig>();
            services.AddHttpClient<IUsersApiClient, UsersApiClient>(client =>
            {
                client.BaseAddress = new Uri(config.UsersServiceUrl);
                client.DefaultRequestHeaders.Add(config.BypassAuthenticationHeader, config.BypassAuthenticationSecret);
            })
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuitBreakerPolicy());
            return services;
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            var jitterer = new Random();
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6,    // exponential back-off plus some jitter
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                                    + TimeSpan.FromMilliseconds(jitterer.Next(0, 100))
                );
        }

        private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
        }
    }
}
