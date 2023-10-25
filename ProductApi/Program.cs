
namespace ProductApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Kestrel
            builder.WebHost.UseKestrel(options =>
            {
                options.ListenAnyIP(5264);
            });

            // Other configurations...

            // Configure services and middleware
            builder.Services.AddControllers(); // Add other services as needed

            var app = builder.Build();

            // Configure middleware
            app.MapControllers(); // Map other endpoints as needed

            app.Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}