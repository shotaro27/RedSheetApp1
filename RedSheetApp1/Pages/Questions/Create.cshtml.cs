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

        public CreateModel(RedSheetApp1Context context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Question Question { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Question.CreateDate = DateTime.Now;
            Question.UpdateDate = DateTime.Now;


            _context.Question.Add(Question);
            await _context.SaveChangesAsync();
            var id = Question.QuestionID;

            foreach (var word in Question.ExtractKeywords())
            {
                var Keyword = new Keywords
                {
                    QuestionID = id,
                    Word = word,
                    CreateDate = DateTime.Now
                };
                _context.Keywords.Add(Keyword);
            }

            await _context.SaveChangesAsync();
            
            return RedirectToPage("./Index");
        }
    }
}
