using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using TestUpload.Service;

namespace TestUpload.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class Schedule
    {
        private readonly RequestDelegate _next;

        public Schedule(RequestDelegate next,ISessionLogoutService sessionLogoutService)
        {
            RecurringJob.AddOrUpdate(() => sessionLogoutService.AutoLogout(), "0 0 17 ? * * *");
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {

            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ScheduleExtensions
    {
        public static IApplicationBuilder UseSchedule(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<Schedule>();
        }
    }
}
