using Diceus_test_task;
using Diceus_test_task.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mindee;
using OpenAI_API;
using Telegram.Bot;

internal class Program
{
    static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        await host.RunAsync();
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHttpClient();
                services.AddSingleton<ITelegramBotClient>(provider => new TelegramBotClient("6620545239:AAFK1uoCJzgUjFe-wkGXUZvb5M2EVrU8Gzk"));
                services.AddSingleton<ICommand, StartCommand>();

                services.AddSingleton(provider => new MindeeClient("02fb042ad045b9d7243a6358835f138d"));
                services.AddSingleton(provider =>new OpenAIAPI("your-api-key"));

                services.AddScoped<BotContext>();
                services.AddHostedService<BotHostedService>();
            });
}