using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Diceus_test_task.State
{
    public class WaitingForConfirmationState : IState
    {
        public async Task HandleAsync(Update update, BotContext context)
        {
            if (update.Message.Text.ToLower() == "yes")
            {
                await context.Bot.SendTextMessageAsync(update.Message.Chat.Id, "The fixed price for the insurance is 100 USD. Do you agree with the price? (yes/no)");
                context.State = new WaitingForAgreementState();
            }
            else if (update.Message.Text.ToLower() == "no")
            {
                context.State = new WaitingForPassportState();
                await context.Bot.SendTextMessageAsync(update.Message.Chat.Id, "Please retake and resubmit the photo of your passport.");
            }
            else
            {
                await context.Bot.SendTextMessageAsync(update.Message.Chat.Id, await context.GenerateAIResponse(update.Message.Text));
                await context.Bot.SendTextMessageAsync(update.Message.Chat.Id, "Expected answer “yes” or “no”");
            }
        }
    }
}