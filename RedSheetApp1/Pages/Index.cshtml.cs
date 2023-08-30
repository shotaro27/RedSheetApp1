using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using RedSheetApp1.Data;
using RedSheetApp1.Models;
using RedSheetApp1.Pages.Questions;
using Microsoft.EntityFrameworkCore;

namespace RedSheetApp1.Pages
{
    public class IndexModel : PageModel
    {
        private readonly RedSheetApp1Context _context;

        public IndexModel(RedSheetApp1Context context)
        {
            _context = context;
        }

        public IList<Question> Question { get; set; }
        public IList<QuestionSet> QuestionSets { get; set; }
        public IList<Keywords> Keywords { get; set; }

        public async Task OnGetAsync()
        {
            Question = await _context.Question.ToListAsync();
            Keywords = await _context.Keywords.ToListAsync();
            QuestionSets = Question.Select(q => new QuestionSet(q, Keywords)).OrderByDescending(s => s.Question.UpdateDate).ToList();
            string dateTime = DateTime.Now.ToString("d", new CultureInfo("ja"));
            ViewData["TimeStamp"] = dateTime;
        }
    }
}
