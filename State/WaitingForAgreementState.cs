using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Diceus_test_task.State
{
    public class WaitingForAgreementState : IState
    {

        public async Task HandleAsync(Update update, BotContext context)
        {
            if (update.Message.Text.ToLower() == "yes")
            {
                try
                {
                    var policyDocument = await context.GeneratePolicyDocument($"{context.PassportData}\n{context.VehicleData}");
                    var pdfStream = context.GeneratePDF(policyDocument);
                    await context.Bot.SendDocumentAsync(update.Message.Chat.Id, InputFile.FromStream(pdfStream, "InsurancePolicy.pdf"));
                    await context.Bot.SendTextMessageAsync(update.Message.Chat.Id, "Thank you for your purchase! Here is your insurance policy.");
                    context.State = new WaitingForPassportState();
                }
                catch (System.Exception ex)
                {
                    await context.Bot.SendTextMessageAsync(update.Message.Chat.Id, "Something went wrong, contact the administrator. \n" + ex.Message);
                }

            }
            else if (update.Message.Text.ToLower() == "no")
            {
                await context.Bot.SendTextMessageAsync(update.Message.Chat.Id, "Thank you. If you have any questions, feel free to ask.");
                context.State = new WaitingForPassportState();
            }
            else
            {
                await context.Bot.SendTextMessageAsync(update.Message.Chat.Id, await context.GenerateAIResponse(update.Message.Text));
                await context.Bot.SendTextMessageAsync(update.Message.Chat.Id, "Expected answer “yes” or “no”");
            }
        }
    }
}