using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Diceus_test_task.State
{
    public class WaitingForVehicleCertificatePhotoState : IState
    {
        public async Task HandleAsync(Update update, BotContext context)
        {
            if (update.Message.Photo != null)
            {
                try
                {
                    var fileId = update.Message.Photo.Last().FileId;
                    var file = await context.Bot.GetFileAsync(fileId);
                    using (var fileStream = new MemoryStream())
                    {
                        await context.Bot.DownloadFileAsync(file.FilePath, fileStream);
                        fileStream.Seek(0, SeekOrigin.Begin);
                        var extractedData = await context.ExtractVehicleIdentificationNumberFromDocument(fileStream);

                        context.VehicleData = extractedData;
                        context.State = new WaitingForConfirmationState();
                        await context.Bot.SendTextMessageAsync(update.Message.Chat.Id, $"Vehicle identification document received. \n Driver License data: {context.DriverLicenseData}. \n Vehicle data: {extractedData}. \n Is this correct? (yes/no)");
                    }
                }
                catch (System.Exception ex)
                {

                    await context.Bot.SendTextMessageAsync(update.Message.Chat.Id, "Something went wrong, contact the administrator. \n" + ex.Message);
                }

            }
            else
            {
                await context.Bot.SendTextMessageAsync(update.Message.Chat.Id, "Please send the document with the vehicle identification number as a photo.");
            }
        }
    }
}