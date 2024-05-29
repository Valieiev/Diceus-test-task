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
                    var policyDocument = await context.GeneratePolicyDocument($"{context.DriverLicenseData}\n{context.VehicleData}");
                    await context.Bot.SendDocumentAsync(update.Message.Chat.Id, InputFile.FromStream(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(policyDocument)), "InsurancePolicy.txt"));
                    await context.Bot.SendTextMessageAsync(update.Message.Chat.Id, "Thank you for your purchase! Here is your insurance policy.");
                    context.State = new WaitingForDriverLicenseState();
                }
                catch (System.Exception ex)
                {

                    await context.Bot.SendTextMessageAsync(update.Message.Chat.Id, "Something went wrong, contact the administrator. \n" + ex.Message);
                }

            }
            else if (update.Message.Text.ToLower() == "no")
            {
                await context.Bot.SendTextMessageAsync(update.Message.Chat.Id, "Thank you. If you have any questions, feel free to ask.");
                context.State = new WaitingForDriverLicenseState();
            }
            else
            {
                await context.Bot.SendTextMessageAsync(update.Message.Chat.Id, "Expected answer “yes” or “no”");
            }
        }
    }
}