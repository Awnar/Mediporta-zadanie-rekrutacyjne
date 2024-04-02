
using Mediporta.StackOverflowWebAPI.Repositories;
using Mediporta.StackOverflowWebAPI.Repositories.Interfaces;
using Mediporta.StackOverflowWebAPI.Services;
using Mediporta.StackOverflowWebAPI.Services.Interface;
using System.Text.Json.Serialization;

namespace Mediporta.StackOverflowWebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddScoped<ITagsService, TagsService>();
            builder.Services.AddScoped<ICacheRepository, CacheRepository>();
            builder.Services.AddStackExchangeRedisCache(options => { options.Configuration = builder.Configuration.GetConnectionString("RedisConnection"); });
            builder.Services.AddAutoMapper(typeof(Program));

            builder.Services.AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
