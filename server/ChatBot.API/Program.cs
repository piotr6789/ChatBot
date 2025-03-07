using ChatBot.Persistence.Context;
using ChatBot.Persistence.Repositories;
using ChatBot.API.Hubs;
using Microsoft.EntityFrameworkCore;
using ChatBot.Infrastructure.Services;
using ChatBot.Application.Messages.Queries;

namespace ChatBot.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            var connectionString = builder.Configuration.GetConnectionString("ChatBotDb");

            builder.Services.AddDbContext<ChatBotDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddSignalR();

            builder.Services.AddScoped<ISessionRepository, SessionRepository>();
            builder.Services.AddScoped<IMessageRepository, MessageRepository>();
            builder.Services.AddScoped<IResponseRepository, ResponseRepository>();

            builder.Services.AddScoped<IChatService, ChatService>();

            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetChatHistoryQuery).Assembly));

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddOpenApiDocument(config =>
            {
                config.DocumentName = "v1";
                config.Title = "ChatBot API";
                config.Version = "v1";
            });

            builder.Services.AddAuthorization();
            builder.Services.AddControllers();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ChatBotDbContext>();
                dbContext.Database.Migrate();
                dbContext.EnsureDatabaseSeeded();
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();

                app.UseOpenApi();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowAngular");

            app.UseAuthorization();

            app.MapControllers();

            app.MapHub<ChatHub>("/chathub");

            app.Run();
        }
    }
}
