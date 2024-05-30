using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Diceus_test_task.State
{
    public class WaitingForPassportState : IState
    {
        public async Task HandleAsync(Update update, BotContext context)
        {
            if (update.Message?.Photo != null)
            {
                try
                {
                    var fileId = update.Message.Photo.Last().FileId;
                    var file = await context.Bot.GetFileAsync(fileId);
                    using (var fileStream = new MemoryStream())
                    {
                        await context.Bot.DownloadFileAsync(file.FilePath, fileStream);
                        fileStream.Seek(0, SeekOrigin.Begin);
                        var extractedData = await context.ExtractDataFromDocument(fileStream);

                        context.PassportData = extractedData;
                        context.State = new WaitingForVehicleCertificatePhotoState();
                        await context.Bot.SendTextMessageAsync(update.Message.Chat.Id, $"Passport received. Now, please send a photo of your vehicle identification document.");
                    }
                }
                catch (System.Exception ex)
                {

                    await context.Bot.SendTextMessageAsync(update.Message.Chat.Id, "Something went wrong, contact the administrator. \n" + ex.Message);
                }

            }
            else
            if (update.Message.Text != null)
            {
                await context.Bot.SendTextMessageAsync(update.Message.Chat.Id, await context.GenerateAIResponse(update.Message.Text));
                await context.Bot.SendTextMessageAsync(update.Message.Chat.Id, "Please send your passport as a photo.");
            }
            else
            {
                await context.Bot.SendTextMessageAsync(update.Message.Chat.Id, "Please send your passport as a photo.");
            }
        }
    }
}