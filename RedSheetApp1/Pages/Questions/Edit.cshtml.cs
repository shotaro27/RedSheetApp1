using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RedSheetApp1.Data;
using RedSheetApp1.Models;

namespace RedSheetApp1.Pages.Questions
{
    public class EditModel : PageModel
    {
        private readonly RedSheetApp1Context _context;

        public EditModel(RedSheetApp1Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Question Question { get; set; }

        [BindProperty]
        public List<Keywords> Keywords { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Question = await _context.Question.FirstOrDefaultAsync(m => m.QuestionID == id);

            Keywords = await _context.Keywords.Where(k => k.QuestionID == id).ToListAsync();

            if (Question == null)
            {
                return NotFound();
            }
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Question).State = EntityState.Modified;
            Question.UpdateDate = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
                var removeKeywords = _context.Keywords.Where(k => k.QuestionID == Question.QuestionID).ToArray();
                _context.Keywords.RemoveRange(removeKeywords);
                var id = Question.QuestionID;

                foreach (var word in Question.ExtractKeywords())
                {
                    var Keyword = new Keywords
                    {
                        QuestionID = id,
                        Word = word,
                        RightOrWrong = false,
                        CreateDate = DateTime.Now
                    };
                    _context.Keywords.Add(Keyword);
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuestionExists(Question.QuestionID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool QuestionExists(int id)
        {
            return _context.Question.Any(e => e.QuestionID == id);
        }
    }
}
