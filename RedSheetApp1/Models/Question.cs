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

        static readonly string endpoint = "https://redsheet.cognitiveservices.azure.com/";
        static readonly string subscriptionKey = "ff767535093e42d68e8ea7b016ae52bd";

        public KeyPhraseCollection ExtractKeywords()
        {
            var textClient = AuthenticateTextAnalytics(endpoint, subscriptionKey);
            return KeyPhraseExtract(textClient, Text);
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
