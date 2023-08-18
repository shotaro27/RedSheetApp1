using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Text.RegularExpressions;
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

        [BindProperty]
        public string QString { get; set; }

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

            QString = HttpUtility.HtmlEncode(Question.Text);

            foreach (var keyword in Keywords.Select(k => k.Word))
            {
                var appendText = $"<span class=\"keyword-edit\">{keyword}</span>";
                QString = QString.Replace(keyword, appendText);
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
            var id = Question.QuestionID;

            Console.WriteLine(QString);

            List<Keywords> newKeywords = new List<Keywords>();

            foreach (Match m in Regex.Matches(QString, "(?<=<span class=\"keyword-edit\">).+?(?=</span>)"))
            {
                Predicate<Keywords> matchKey = k => k.Word == m.Value;
                if (Keywords.Exists(matchKey))
                {
                    var keyword = Keywords.Find(matchKey);
                    newKeywords.Add(keyword);
                }
                else if (newKeywords.Exists(matchKey) || m.Length == 0)
                {
                    Console.WriteLine(m.Value);
                }
                else
                {
                    newKeywords.Add(new Keywords()
                    {
                        QuestionID = id,
                        Word = m.Value,
                        RightOrWrong = false,
                        CreateDate = DateTime.Now
                    });
                }
            }

            Question.Text = Regex.Replace(QString, "<(\"[^\"]*\"|'[^']*'|[^'\">])*>", "");


            try
            {
                Keywords = newKeywords;
                var currentKeywords = _context.Keywords.Where(k => k.QuestionID == id).ToList();
                var removeKeywords = currentKeywords.Where(k => !newKeywords.Select(w => w.Word).Contains(k.Word)).ToList();
                _context.RemoveRange(removeKeywords);
                var addKeywords = newKeywords.Where(k => !currentKeywords.Select(w => w.Word).Contains(k.Word)).ToList();
                _context.AddRange(addKeywords);
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

            return Redirect($"./Details?id={id}");
        }

        private bool QuestionExists(int id)
        {
            return _context.Question.Any(e => e.QuestionID == id);
        }
    }
}
