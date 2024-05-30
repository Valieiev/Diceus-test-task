using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diceus_test_task.State;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mindee;
using OpenAI_API;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Diceus_test_task
{
    public class BotHostedService : IHostedService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IServiceProvider _serviceProvider;
        private CancellationTokenSource _cts;
        private static ConcurrentDictionary<long, BotContext> _userContexts = new ConcurrentDictionary<long, BotContext>();

        

        public BotHostedService(ITelegramBotClient botClient, IServiceProvider serviceProvider)
        {
            _botClient = botClient;
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { },
            };

            _botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                _cts.Token);

            Console.WriteLine("Bot is running...");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cts.Cancel();
            return Task.CompletedTask;
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<BotContext>();

            if (update.Message != null)
            {
                var userId = update.Message?.From?.Id ?? 222309675;

                if (update.Message.From.IsBot) return;

                var botContext = _userContexts.GetOrAdd(userId, id =>
                {
                    using var scope = _serviceProvider.CreateScope();
                    var mindeeClient = scope.ServiceProvider.GetRequiredService<MindeeClient>();
                    var openAiApi = scope.ServiceProvider.GetRequiredService<OpenAIAPI>();
                    return new BotContext(_botClient, mindeeClient, openAiApi);
                });
                
                switch (update.Message.Text)
                {
                    case "/start":
                        await botContext.Bot.SendTextMessageAsync(update.Message.Chat.Id, $"Welcome, {update.Message?.From?.FirstName} {update.Message?.From?.LastName}! Please send a photo of your driver license.");
                        botContext.State = new WaitingForDriverLicenseState();
                        break;

                    case "/reset":
                        _userContexts.TryRemove(userId, out _);
                        await botContext.Bot.SendTextMessageAsync(update.Message.Chat.Id, "Your state has been reset. Please send a photo of your driver license.");
                        break;

                    default:
                        await botContext.HandleUpdateAsync(update);
                        break;
                }

            }
        }

        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Error occurred: {exception.Message}");
            return Task.CompletedTask;
        }
    }
}