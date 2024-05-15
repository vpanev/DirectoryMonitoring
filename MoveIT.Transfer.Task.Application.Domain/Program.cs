using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Console;
using MoveIT.Transfer.Task.Application.Domain.Helpers;
using MoveIT.Transfer.Task.Application.Domain.Helpers.TokenManager;
using MoveIT.Transfer.Task.Application.Domain.Middleware;
using MoveIT.Transfer.Task.Application.Domain.Services.AuthenticationService;
using MoveIT.Transfer.Task.Application.Domain.Services.FileWatcherService;
using MoveIT.Transfer.Task.Application.Domain.Services.HttpClientService;

namespace MoveIT.Transfer.Task.Application.Domain
{
    public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			ConfigureServices(builder);

			var app = builder.Build();
			Configure(app);
		}

		private static void ConfigureServices(WebApplicationBuilder builder)
		{
			builder.Logging.AddSimpleConsole(options =>
			{
				options.SingleLine = false;
				options.ColorBehavior = LoggerColorBehavior.Enabled;
			});

			builder.Services.AddControllers();
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();
			builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

			builder.Services.AddScoped<IHttpClientService, HttpClientService>();
			builder.Services.AddScoped<IFileWatcherService, FileWatcherService>();
			builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
			builder.Services.AddSingleton<ITokenManager, TokenManager>();
			builder.Services.AddHttpClient();
			builder.Services.AddMemoryCache();
		}

		private static void Configure(WebApplication app)
		{
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseMiddleware<ExceptionHandlerMiddleware>();
			app.UseHttpsRedirection();

			app.UseAuthorization();

			app.MapControllers();

			app.Run();
		}
	}
}
