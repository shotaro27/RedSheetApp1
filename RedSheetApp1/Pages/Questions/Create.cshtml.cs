using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RedSheetApp1.Data;
using RedSheetApp1.Models;

namespace RedSheetApp1.Pages.Questions
{
    public class CreateModel : PageModel
    {
        private readonly RedSheetApp1Context _context;
        public static Question CurrentQuestion { get; set; }
        public static List<Keywords> CurrentKeywords { get; set; }

        [BindProperty]
        public Question Question { get; set; }

        [BindProperty]
        public string QString { get; set; }

        public CreateModel(RedSheetApp1Context context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            if (TempData["Text"] != null)
            {
                Question = CurrentQuestion;
                Question.Text = TempData["Text"].ToString();
                QString = EditorUtil.AppendTag(Question.Text, CurrentKeywords);
            }
            else
            {
                Question = new Question();
                CurrentKeywords = new List<Keywords>();
                QString = "";
            }
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(string send)
        {
            CurrentKeywords = EditorUtil.Replace(QString, -1);
            Question.Text = EditorUtil.OmitTag(QString);
            if (send == "cropimage")
            {
                CurrentQuestion = Question;
                return RedirectToPage("./CropImage");
            }

            if (!ModelState.IsValid || string.IsNullOrEmpty(QString))
            {
                return Page();
            }

            if (string.IsNullOrEmpty(Question.Title))
            {
                Question.Title = "Untitled";
            }

            Question.CreateDate = DateTime.Now;
            Question.UpdateDate = DateTime.Now;


            _context.Question.Add(Question);
            await _context.SaveChangesAsync();
            var id = Question.QuestionID;

            CurrentKeywords.ForEach(k => k.QuestionID = id);
            _context.Keywords.AddRange(CurrentKeywords);

            await _context.SaveChangesAsync();
            
            return RedirectToPage("./Index");
        }
    }
}
