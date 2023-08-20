using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Text.RegularExpressions;
using RedSheetApp1.Models;

namespace RedSheetApp1.Pages.Questions
{
    public static class EditorUtil
    {
        public static List<Keywords> Replace(string QString, int id)
        {
            if (string.IsNullOrEmpty(QString))
            {
                return new List<Keywords>();
            }

            List<Keywords> newKeywords = new List<Keywords>();

            newKeywords.AddRange(RegisterKeyword(QString, id, "wrong", false));
            newKeywords.AddRange(RegisterKeyword(QString, id, "right", true));

            return newKeywords;
        }

        static List<Keywords> RegisterKeyword(string QString, int id, string t, bool rightorwrong)
        {
            List<Keywords> newKeywords = new List<Keywords>();

            foreach (Match m in Regex.Matches(QString, $"(?<=<span class=\"keyword-{t}\">).+?(?=</span>)"))
            {
                if (m.Length != 0)
                {
                    var position = CountOf(QString.Substring(0, m.Index), m.Value);
                    newKeywords.Add(new Keywords()
                    {
                        QuestionID = id,
                        Word = m.Value,
                        RightOrWrong = rightorwrong,
                        Position = position,
                        CreateDate = DateTime.Now
                    });
                }
            }

            return newKeywords;
        }

        public static string OmitTag(string QString) => string.IsNullOrEmpty(QString) ? "" : Regex.Replace(QString, "<(\"[^\"]*\"|'[^']*'|[^'\">])*>", "");

        public static string AppendTag(string text, List<Keywords> keywords)
            => AppendHTML(text, keywords, keyword => $"<span class=\"keyword-{(keyword.RightOrWrong ?? false ? "right" : "wrong")}\">{keyword.Word}</span>");

        public static string AppendHTML(string text, List<Keywords> keywords, Func<Keywords, string> sentence)
        {
            var QString = HttpUtility.HtmlEncode(text);

            foreach (var keyword in keywords)
            {
                var appendText = sentence(keyword);
                QString = ReplaceNthString(QString, keyword.Position, keyword.Word, appendText);
            }

            return QString;
        }

        public static int CountOf(string input, string src)
        {
            int count = 0;

            int index = input.IndexOf(src, 0);
            while (index != -1)
            {
                count++;
                index = input.IndexOf(src, index + src.Length);
            }

            return count;
        }

        public static string ReplaceNthString(string input, int index, string src, string dst)
        {
            var regex = new Regex(src);
            var input2 = regex.Replace(input, dst, index + 1);
            regex = new Regex(dst);
            return regex.Replace(input2, src, index);
        }
    }
}
