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
using System.Text;
using System.Text.RegularExpressions;

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

            if (Question == null || Keywords == null)
            {
                return NotFound();
            }

            QString = HttpUtility.HtmlEncode(Question.Text);

            var wrongKeywords = Keywords.Where(k => !(k.RightOrWrong ?? false)).ToList();

            if (wrongKeywords.Count == 0)
            {
                return Redirect("./Index");
            }

            var renderText = new StringBuilder();
            var rand = new Random();
            var split = QString.Split('�B');

            foreach (var sentence in split)
            {
                if (sentence.Length == 0)
                {
                    renderText.Append(sentence);
                    continue;
                }
                var mc = Regex.Matches(sentence, string.Join("|", wrongKeywords.Select(k => Regex.Escape(k.Word))));
                if (mc.Count == 0)
                {
                    renderText.Append($"{sentence}�B");
                }
                else
                {
                    var m = mc[rand.Next(0, mc.Count)];
                    var keyword = Keywords.FirstOrDefault(k => k.Word == m.Value);
                    if (keyword == null)
                    {
                        renderText.Append($"{sentence}�B");
                    }
                    else
                    {
                        var appendText = $"<span class='redsheet-parent' keyword-id='{keyword.KeywordsID}'>"
                        + $"<div class='redsheet-option' role='group'>"
                        + $"<input type='radio' class='btn-check' name='right-or-wrong-{keyword.KeywordsID}' id='right-{keyword.KeywordsID}' autocomplete='off' value='1'>"
                        + $"<label class='redsheet-right' for='right-{keyword.KeywordsID}'> �� </label>"
                        + $"<input type='radio' class='btn-check' name='right-or-wrong-{keyword.KeywordsID}' id='wrong-{keyword.KeywordsID}' autocomplete='off' value='0'>"
                        + $"<label class='redsheet-wrong' for='wrong-{keyword.KeywordsID}'> �~ </label>"
                        + "</div>"
                        + $"<a class='redsheet'>{keyword.Word}</a></span>";
                        renderText.Append($"{sentence[..m.Index]}{appendText}{sentence[(m.Index + m.Length)..]}�B");
                    }
                }
            }

            QString = renderText.ToString();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string msg)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var sets = msg.Split("|");
            foreach (var set in sets.Take(sets.Length - 1))
            {
                var split = set.Split(",");
                var Keyword = await _context.Keywords.FindAsync(int.Parse(split[0]));
                Keyword.RightOrWrong = split[1] == "1";
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
