using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RedSheetApp1.Pages.Questions
{
    public class TextEditModel : PageModel
    {
        public string Text { get; set; }
        public IList<Line> Lines { get; set; }
        public string LineText { get; set; }
        public IActionResult OnGet()
        {
            Lines = CropImageModel.TextData;
            LineText = JsonConvert.SerializeObject(Lines);
            if (Lines == null)
            {
                return NotFound();
            }
            var size = Lines.Select(GetSize).Average();
            Text = string.Join("", Lines.Where(line => size - GetSize(line) <= 12).Select(line => line.Text));
            return Page();
        }
        public IActionResult OnPost(string text)
        {
            TempData["Text"] = CreateModel.CurrentQuestion.Text + text;

            return RedirectToPage("./Create");
        }

        public static double GetSize(Line line) => Math.Sqrt(Math.Pow((line.BoundingBox[6] ?? 0) - (line.BoundingBox[0] ?? 0), 2)
            + Math.Pow((line.BoundingBox[7] ?? 0) - (line.BoundingBox[1] ?? 0), 2));
    }
}
