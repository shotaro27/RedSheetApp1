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
        private readonly RedSheetApp1Context _context;
        static readonly string endpoint = "https://redsheet.cognitiveservices.azure.com/";
        static readonly string subscriptionKey = "ff767535093e42d68e8ea7b016ae52bd";

        public CropImageModel(RedSheetApp1Context context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(string imgurl)
        {
            var visionClient = AuthenticateComputerVision(endpoint, subscriptionKey);
            var text = await ReadFileUrlAsync(visionClient, imgurl);

            TempData["Text"] = text;

            return RedirectToPage("./Create");
        }

        public static ComputerVisionClient AuthenticateComputerVision(string endpoint, string key)
        {
            return new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
            { Endpoint = endpoint };
        }

        public static async Task<string> ReadFileUrlAsync(ComputerVisionClient client, string fileUrl)
        {
            var textHeaders = await client.ReadInStreamAsync(Base64ToImage(fileUrl));
            return await ExtractReadResultAsync(client, textHeaders.OperationLocation, fileUrl);
        }

        public static async Task<string> ExtractReadResultAsync(ComputerVisionClient client, string operationLocation, string filePath)
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
            return string.Join("", textUrlFileResults[0].Lines.Select(l => l.Text));
        }

        public static Stream Base64ToImage(string imageURL)
        {
            var stream = new MemoryStream(Convert.FromBase64String(imageURL.Substring(imageURL.IndexOf("base64,") + 7)));

            return stream;
        }
    }
}
