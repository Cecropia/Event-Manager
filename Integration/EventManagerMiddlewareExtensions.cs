﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Integration
{
    public static class EventManagerMiddlewareExtensions
    {
        public static IApplicationBuilder UseEventManager(this IApplicationBuilder builder)
        {
            // see here for reference: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/e33dba2e416469fca6c2f4975c1c41dbac942804/src/Swashbuckle.AspNetCore.SwaggerUI/SwaggerUIBuilderExtensions.cs
                return builder.UseMiddleware<EventManagerMiddleware>();
            
        }
    }
}