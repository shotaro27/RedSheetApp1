using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RedSheetApp1.Data;
using RedSheetApp1.Models;

namespace RedSheetApp1.Pages.Questions
{
    public class RedSheetModel : PageModel
    {
        private readonly RedSheetApp1Context _context;

        public RedSheetModel(RedSheetApp1Context context)
        {
            _context = context;
        }

        [BindProperty]
        public string QString { get; set; }

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
            QString = HttpUtility.HtmlEncode(Question.Text);

            foreach (var keyword in Keywords.Select(k => k.Word))
            {
                QString = QString.Replace(keyword, $"<span class='redsheet-parent'><a class='redsheet' style='width: {keyword.Length}em;'></a>{keyword}</span>");
            }

            return Page();
        }
    }
}
