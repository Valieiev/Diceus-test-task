using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Diceus_test_task.State
{
    public interface IState
    {
        Task HandleAsync(Update update, BotContext context);
    }
}