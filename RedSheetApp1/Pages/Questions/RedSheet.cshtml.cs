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
            var appendString = EditorUtil.AppendHTML(QString, wrongKeywords, keyword =>
                $"<span keyword-id='{keyword.KeywordsID}'>{keyword.Word}</span>");
            var split = appendString.Split('。');

            foreach (var appendSentence in split)
            {
                Console.WriteLine(appendSentence);
                var sentence = EditorUtil.OmitTag(appendSentence);
                if (string.IsNullOrEmpty(sentence)) continue;

                var mc = Regex.Matches(appendSentence, "<span keyword-id='\\d+'>.+?</span>");
                if (mc.Count == 0)
                {
                    renderText.Append($"{sentence}。");
                }
                else
                {
                    var mIndex = rand.Next(0, mc.Count);
                    var m = mc[mIndex];
                    var keywordID = int.Parse(Regex.Match(m.Value, "(?<=<span keyword-id=')\\d+?(?='>)").Value);
                    var keyword = Keywords.Find(k => k.KeywordsID == keywordID);
                    if (keyword == null)
                    {
                        renderText.Append($"{sentence}。");
                    }
                    else
                    {
                        var appendText = $"<span class='redsheet-parent' keyword-id='{keyword.KeywordsID}'>"
                        + $"<div class='redsheet-option' role='group'>"
                        + $"<input type='radio' class='btn-check' name='right-or-wrong-{keyword.KeywordsID}' id='right-{keyword.KeywordsID}' autocomplete='off' value='1'>"
                        + $"<label class='redsheet-right' for='right-{keyword.KeywordsID}'> ○ </label>"
                        + $"<input type='radio' class='btn-check' name='right-or-wrong-{keyword.KeywordsID}' id='wrong-{keyword.KeywordsID}' autocomplete='off' value='0'>"
                        + $"<label class='redsheet-wrong' for='wrong-{keyword.KeywordsID}'> × </label>"
                        + "</div>"
                        + $"<a class='redsheet'>{keyword.Word}</a></span>";
                        renderText.Append($"{appendSentence[..m.Index]}{appendText}{appendSentence[(m.Index + m.Length)..]}。");
                    }
                }
            }

            QString = renderText.ToString();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string send, string msg, int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (string.IsNullOrEmpty(msg))
            {
                return Route(send, id);
            }

            var sets = msg.Split("|");
            foreach (var set in sets.Take(sets.Length - 1))
            {
                var split = set.Split(",");
                var Keyword = await _context.Keywords.FindAsync(int.Parse(split[0]));
                Keyword.RightOrWrong = split[1] == "1";
            }

            await _context.SaveChangesAsync();

            return Route(send, id);
        }
        public IActionResult Route(string send, int id)
        {
            if (send == "replay")
            {
                return Redirect($"./RedSheet?id={id}");
            }

            return Redirect($"./Details?id={id}");
        }
    }
}
