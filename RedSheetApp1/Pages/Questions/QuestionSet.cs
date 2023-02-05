using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RedSheetApp1.Models;

namespace RedSheetApp1.Pages.Questions
{
    public class QuestionSet
    {
        public Question Question { get; set; }
        public double Progress { get; set; }
        public QuestionSet(Question question, IEnumerable<Keywords> keywords)
        {
            Question = question;
            Progress = GetQuestionProgress(question, keywords);
        }
        double GetQuestionProgress(Question Question, IEnumerable<Keywords> Keywords)
        {
            var qKeywords = Keywords.Where(k => k.QuestionID == Question.QuestionID).ToList();
            if (qKeywords.Count == 0)
            {
                return 0;
            }
            else
            {
                return qKeywords.Select(k => k.RightOrWrong ?? false ? 1 : 0).Average();
            }
        }
    }
}
