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
    public class QAModel : PageModel
    {
        private readonly RedSheetApp1Context _context;

        public QAModel(RedSheetApp1Context context)
        {
            _context = context;
        }

        static OpenAI_API.Chat.Conversation Chat { get; set; }

        [BindProperty]
        public QASet QA { get; set; }

        public Question Question { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Question = await _context.Question.FirstOrDefaultAsync(m => m.QuestionID == id);

            if (Chat == null)
            {
                (QA, Chat) = await Question.GetQuestionFromChatGPTAsync();
                foreach (var item in Chat.Messages)
                {
                    Console.WriteLine(item.Content);
                }
            }
            else
            {
                (QA, Chat) = await Question.GetNextQuestionFromChatGPTAsync(Chat);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string send, int id)
        {
            if (!ModelState.IsValid || send != "next")
            {
                return Page();
            }
            if (QA == null)
            {
                Console.WriteLine("nullnull");
            }
            else
            {
                Console.WriteLine(QA.Question);
                Console.WriteLine(QA.Answer);
                foreach (var item in Chat.Messages)
                {
                    Console.WriteLine(item.Content);
                }
            }
            return Redirect($"./QA?id={id}");
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
