using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Azure;
using Azure.AI.TextAnalytics;

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

        static readonly string endpoint = "https://redsheet.cognitiveservices.azure.com/";
        static readonly string subscriptionKey = "ff767535093e42d68e8ea7b016ae52bd";

        static readonly string ChatGPTKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY", EnvironmentVariableTarget.User);

        public KeyPhraseCollection ExtractKeywords()
        {
            var textClient = AuthenticateTextAnalytics(endpoint, subscriptionKey);
            return KeyPhraseExtract(textClient, Text);
        }

        internal async Task<string> GetKeywordsFromChatGPTAsync()
        {
            var api = new OpenAI_API.OpenAIAPI(ChatGPTKey);
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
            // ChatGPTに質問
            chat.AppendUserInput(text);

            // ChatGPTの回答
            string response = await chat.GetResponseFromChatbotAsync();
            Console.WriteLine(response);
            return response;
        }

        public static TextAnalyticsClient AuthenticateTextAnalytics(string endpoint, string key)
        {
            return new TextAnalyticsClient(new Uri(endpoint), new AzureKeyCredential(key));
        }

        public static KeyPhraseCollection KeyPhraseExtract(TextAnalyticsClient client, string text)
        {
            var response = client.ExtractKeyPhrases(text, "ja");
            return response.Value;
        }
    }
}
