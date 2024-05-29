using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Diceus_test_task.Commands
{
    public class StartCommand : ICommand
    {
        private readonly ITelegramBotClient _botClient;

        public StartCommand(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task ExecuteAsync(Update update, string message)
        {
            await _botClient.SendTextMessageAsync(update.Message?.Chat.Id ?? 222309675, message);
        }
    }
}