﻿using System;
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
    public class DetailsModel : PageModel
    {
        private readonly RedSheetApp1Context _context;

        public DetailsModel(RedSheetApp1Context context)
        {
            _context = context;
        }

        public Question Question { get; set; }
        public QuestionSet QuestionSet { get; set; }
        public IList<Keywords> Keywords { get; set; }
        public string QString { get; set; }

        [BindProperty]
        public Keywords NewWord { get; set; }

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

            foreach (var keyword in Keywords)
            {
                var appendText = $"<span class='keyword-parent keyword-{(keyword.RightOrWrong ?? false ? "right" : "wrong")}' keyword-id='{keyword.KeywordsID}'>"
                    + $"<a class='keyword'>{keyword.Word}</a>"
                    + $"<div class='redsheet-option' role='group'>"
                    + $"<a class='keyword-delete' href='./DeleteKeyword?id={keyword.KeywordsID}&qid={id}'>削除</a>"
                    + "</div></span>";
                QString = QString.Replace(keyword.Word, appendText);
            }

            QuestionSet = new QuestionSet(Question, Keywords);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? qid)
        {
            Question = await _context.Question.FirstOrDefaultAsync(m => m.QuestionID == qid);

            Keywords = await _context.Keywords.Where(k => k.QuestionID == qid).ToListAsync();

            if (Question == null || Keywords == null)
            {
                return NotFound();
            }

            if (!Question.Text.Contains(NewWord.Word) || Keywords.Select(k => k.Word).Contains(NewWord.Word))
            {
                return Redirect($"./Details?id={qid}");
            }

            NewWord.QuestionID = qid ?? 0;
            NewWord.RightOrWrong = false;
            NewWord.CreateDate = DateTime.Now;
            _context.Keywords.Add(NewWord);

            await _context.SaveChangesAsync();

            return Redirect($"./Details?id={qid}");
        }
    }
}
