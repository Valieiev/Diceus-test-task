using Diceus_test_task.State;
using Mindee;
using Mindee.Input;
using Mindee.Parsing.Common;
using Mindee.Product.Us.DriverLicense;
using Mindee.Product.Generated;
using OpenAI_API;
using Telegram.Bot;
using Telegram.Bot.Types;
using iText.Kernel.Pdf;
using iText.Layout.Element;

namespace Diceus_test_task
{
    public class BotContext
    {
        public IState State { get; set; }
        public ITelegramBotClient Bot { get; }
        private readonly MindeeClient _mindeeClient;
        private readonly OpenAIAPI _openAIClient;

        public string DriverLicenseData { get; set; }
        public string VehicleData { get; set; }

        public BotContext(ITelegramBotClient bot, MindeeClient mindeeClient, OpenAIAPI openAIClient)
        {
            Bot = bot;
            _mindeeClient = mindeeClient;
            _openAIClient = openAIClient;
            State = new WaitingForDriverLicenseState();
        }

        public async Task HandleUpdateAsync(Update update)
        {
            await State.HandleAsync(update, this);
        }

        public async Task<string> ExtractDataFromDocument(MemoryStream documentStream)
        {
            var inputSource = new LocalInputSource(documentStream.ToArray(), "document.png");
            var prediction = await _mindeeClient.ParseAsync<DriverLicenseV1>(inputSource);
            var extractedData = ExtractRelevantDocumentData(prediction.Document);

            return extractedData;
        }

        public async Task<string> ExtractVehicleIdentificationNumberFromDocument(MemoryStream documentStream)
        {
            var inputSource = new LocalInputSource(documentStream.ToArray(), "document.png");
            var prediction = await _mindeeClient.EnqueueAndParseAsync<GeneratedV1>(inputSource, endpoint: new Mindee.Http.CustomEndpoint("vehicle_identification_document", accountName: "AValieiev", version: "1"));
            var extractedData = ExtractRelevantVINData(prediction.Document);

            return extractedData;
        }

        public async Task<string> GeneratePolicyDocument(string extractedData)
        {
            var policyData = $" \nStart Date: {DateTime.Now} \n End Date: {DateTime.Now.AddYears(1)} \n  Coverage Amount: 1000$";
            var prompt = $"Generate a car insurance policy document with the following details:\n{extractedData + policyData}";
            var response = await _openAIClient.Completions.CreateCompletionAsync(new OpenAI_API.Completions.CompletionRequest(
                prompt,
                model: OpenAI_API.Models.Model.Davinci, // or use a more specific model
                max_tokens: 150
                )
            );

            return response.Completions.FirstOrDefault()?.Text.Trim();
        }

        public Task Reset()
        {
            State = new WaitingForDriverLicenseState();
            return Task.CompletedTask;
        }


        private string ExtractRelevantDocumentData(Document<DriverLicenseV1> document)
        {
            if (document.Inference == null)
            {
                return "Data could not be extracted.";
            }

            var license = document.Inference.Prediction;
            return $"\n First Name: {license.FirstName} \n Last Name: {license.LastName} \n DOB: {license.DateOfBirth} \n Driver License Number: {license.DriverLicenseId}";
        }

        private string ExtractRelevantVINData(Document<GeneratedV1> document)
        {
            if (document.Inference == null)
            {
                return "Data could not be extracted.";
            }

            var license = document.Inference.Prediction.Fields;
            string VIN = license["vin"].ToString().Split(':')[2].Trim();
            return $"\n Vehicle Identification Number: {VIN}";
        }

        public MemoryStream GeneratePDF(string content)
        {
            var memoryStream = new MemoryStream();

            using (var writer = new PdfWriter(memoryStream))
            {
                using (var pdf = new PdfDocument(writer))
                {
                    var document = new iText.Layout.Document(pdf);
                    document.Add(new Paragraph(content));
                }
            }

            memoryStream.Position = 0;
            return memoryStream;
        }

    }
}