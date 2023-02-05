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
    public class DeleteKeywordModel : PageModel
    {
        private readonly RedSheetApp1Context _context;

        public DeleteKeywordModel(RedSheetApp1Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Question Question { get; set; }

        [BindProperty]
        public Keywords Keyword { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, int? qid)
        {
            if (id == null || qid == null)
            {
                return NotFound();
            }

            Question = await _context.Question.FirstOrDefaultAsync(m => m.QuestionID == qid);

            if (Question == null)
            {
                return NotFound();
            }

            Keyword = await _context.Keywords.FirstOrDefaultAsync(m => m.KeywordsID == id);

            if (Keyword == null)
            {
                return Redirect($"./Details?id={qid}");
            }

            _context.Keywords.Remove(Keyword);
            await _context.SaveChangesAsync();

            return Redirect($"./Details?id={qid}");
        }
    }
}
