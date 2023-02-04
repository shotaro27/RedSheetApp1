using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RedSheetApp1.Data;
using RedSheetApp1.Models;

namespace RedSheetApp1.Pages.Questions
{
    public class ResetModel : PageModel
    {
        private readonly RedSheetApp1Context _context;

        public ResetModel(RedSheetApp1Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Question Question { get; set; }
        public QuestionSet QuestionSet { get; set; }
        public IList<Keywords> Keywords { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Question = await _context.Question.FirstOrDefaultAsync(m => m.QuestionID == id);

            Keywords = await _context.Keywords.Where(k => k.QuestionID == id).ToListAsync();

            if (Question == null || Keywords == null)
            {
                return NotFound();
            }

            QuestionSet = new QuestionSet(Question, Keywords);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Keywords = await _context.Keywords.Where(k => k.QuestionID == id).ToListAsync();
            foreach (var keyword in Keywords)
            {
                keyword.RightOrWrong = false;
            }
            
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
