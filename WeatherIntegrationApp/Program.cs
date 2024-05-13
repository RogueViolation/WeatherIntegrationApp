using Utility;
using DataAccess;
using Utilities;

namespace WeatherIntegrationApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            //Dependency injection for all the valid services
            builder.Services.AddSingleton<IConfigurationReader, ConfigurationReader>(); 
            builder.Services.AddSingleton<IProcessWrapper, ProcessWrapper>();
            builder.Services.AddSingleton<IWeatherAccess, WeatherAccess>();
            builder.Services.AddSingleton<IFileWriter, FileWriter>();

            //Add a named httpClient that we will use for calling Weather API that has the API key already set.
            builder.Services.AddHttpClient("WeatherHttpClient", client =>
            {
                client.BaseAddress = new Uri("http://api.weatherapi.com/v1/");
                client.DefaultRequestHeaders.Add("key", builder.Configuration["Weather:APIKey"]);
            });

            var app = builder.Build();

            //This exposes Swagger in Production. Although it would be considered unsafe, Swagger in itself only documents APIs and does not protect them per se.
            //Disabling Swagger in production is a good practice, however other forms of protection should be used for securing an API
            //if (app.Environment.IsDevelopment())
            //{
            app.UseSwagger();
            app.UseSwaggerUI();
            //}

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
