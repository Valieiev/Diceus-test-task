using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Diceus_test_task.Commands
{
    public interface ICommand
    {
        Task ExecuteAsync(Update update, string message);
    }
}