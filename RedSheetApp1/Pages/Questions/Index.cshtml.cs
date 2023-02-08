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
    public class IndexModel : PageModel
    {
        private readonly RedSheetApp1Context _context;

        public IndexModel(RedSheetApp1Context context)
        {
            _context = context;
        }

        public IList<Question> Question { get;set; }
        public IList<QuestionSet> QuestionSets { get; set; }
        public IList<Keywords> Keywords { get; set; }

        public async Task OnGetAsync()
        {
            Question = await _context.Question.ToListAsync();
            Keywords = await _context.Keywords.ToListAsync();
            QuestionSets = Question.Select(q => new QuestionSet(q, Keywords)).OrderByDescending(s => s.Question.UpdateDate).ToList();
        }
    }
}
