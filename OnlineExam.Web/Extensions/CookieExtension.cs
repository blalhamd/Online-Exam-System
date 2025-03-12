namespace OnlineExam.Web.Extensions
{
    public static class CookieExtension
    {
        public static IServiceCollection RegisterSessionConfig(this IServiceCollection services)
        {
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Authentication/Login"; // Redirect to login if unauthorized
                options.AccessDeniedPath = "/Authentication/AccessDenied"; // If user lacks permission
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.SlidingExpiration = true;
            });

            return services;
        }
    }
}
