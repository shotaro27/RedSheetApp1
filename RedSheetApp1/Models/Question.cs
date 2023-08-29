using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RedSheetApp1.Pages.Questions;
using OpenAI_API;
using OpenAI_API.Chat;

namespace RedSheetApp1.Models
{
    public class Question
    {
        public int QuestionID { get; set; }
        [DataType(DataType.Date)]
        public DateTime CreateDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime UpdateDate { get; set; }
        public string Text { get; set; }
        public string Title { get; set; }

        static readonly string ChatGPTKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY", EnvironmentVariableTarget.User);

        internal async Task<string> GetKeywordsFromChatGPTAsync()
        {
            var api = new OpenAIAPI(ChatGPTKey);
            var chat = api.Chat.CreateConversation();

            var text = @$"# Imperative Statement:
You are a school teacher.
Based on the following constraints and input sentences, mark the key words and phrases in the text.

# Following Constraints:
- Mark possible questions in the form of fill-in-the-blanks in the reference book.
- Do not leave out important key words.
- Mark as many words as possible.
- Mark in the format ""<span class=""keyword-wrong"">keywords</span>"".
- Output in HTML format.
- Do not use p tags.

# Input Sentences:
{Text}

# Output Sentences:
";
            chat.AppendUserInput(text);

            string response = await chat.GetResponseFromChatbotAsync();
            Console.WriteLine(response);
            return response;
        }
        internal async Task<(QASet, Conversation)> GetNextQuestionFromChatGPTAsync(Conversation chat)
        {
            Console.WriteLine(chat);
            foreach (var item in chat.Messages)
            {
                Console.WriteLine(item);
            }

            var text = @$"Output:
";
            chat.AppendUserInput(text);

            string response = await chat.GetResponseFromChatbotAsync();
            Console.WriteLine(response);
            if (response.Contains("```"))
            {
                response = response.Split("```")[1];
            }
            response = response.Split("}")[0] + "}";

            var QA = JsonConvert.DeserializeObject<QASet>(response);
            return (QA, chat);
        }

        internal async Task<(QASet, Conversation)> GetQuestionFromChatGPTAsync()
        {
            var api = new OpenAIAPI(ChatGPTKey);
            var chat = api.Chat.CreateConversation(new ChatRequest() { MaxTokens = 80 });

            var text = @$"Please submit questions where the input text is the basis for the question in a question-and-answer format.
Answers should be no more than 2 words.
Outputs should be given one question at a time, and one question should be given each time ""Output:"" is entered.

Output Format:
{{""Question"": ""問題"", ""Answer"": ""解答""}}

Input:
{Text}

Output:
";
            chat.AppendUserInput(text);

            string response = await chat.GetResponseFromChatbotAsync();
            Console.WriteLine(response);
            if (response.Contains("```"))
            {
                response = response.Split("```")[1];
            }
            response = response.Split("}")[0] + "}";

            var QA = JsonConvert.DeserializeObject<QASet>(response);
            return (QA, chat);
        }
    }
}
