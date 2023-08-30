using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RedSheetApp1.Data;
using RedSheetApp1.Models;
using System.IO;

namespace RedSheetApp1.Pages.Questions
{
    public class CropImageModel : PageModel
    {
        static readonly string endpoint = "https://redsheet.cognitiveservices.azure.com/";
        static readonly string subscriptionKey = Environment.GetEnvironmentVariable("COGNITIVESERVICE_KEY", EnvironmentVariableTarget.User);

        public static IList<Line> TextData { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(string send, string imgurl)
        {
            if (send == "back")
            {
                TempData["Text"] = CreateModel.CurrentQuestion.Text;
                return RedirectToPage("./Create");
            }

            var visionClient = AuthenticateComputerVision(endpoint, subscriptionKey);
            TextData = await ReadFileUrlAsync(visionClient, imgurl);

            return RedirectToPage("./TextEdit");
        }

        public static ComputerVisionClient AuthenticateComputerVision(string endpoint, string key)
        {
            return new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
            { Endpoint = endpoint };
        }

        public static async Task<IList<Line>> ReadFileUrlAsync(ComputerVisionClient client, string fileUrl)
        {
            var textHeaders = await client.ReadInStreamAsync(Base64ToImage(fileUrl));
            return await ExtractReadResultAsync(client, textHeaders.OperationLocation, fileUrl);
        }

        public static async Task<IList<Line>> ExtractReadResultAsync(ComputerVisionClient client, string operationLocation, string filePath)
        {
            const int numberOfCharsInOperationId = 36;
            var operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

            ReadOperationResult results;
            Console.WriteLine($"Extracting text from URL file {Path.GetFileName(filePath)}...");
            Console.WriteLine();

            do
            {
                results = await client.GetReadResultAsync(Guid.Parse(operationId));
            }
            while ((results.Status == OperationStatusCodes.Running || results.Status == OperationStatusCodes.NotStarted));

            // Display the found text.
            Console.WriteLine();
            var textUrlFileResults = results.AnalyzeResult.ReadResults;
            return textUrlFileResults[0].Lines;
        }

        public static Stream Base64ToImage(string imageURL)
        {
            var stream = new MemoryStream(Convert.FromBase64String(imageURL.Substring(imageURL.IndexOf("base64,") + 7)));

            return stream;
        }
    }
}
