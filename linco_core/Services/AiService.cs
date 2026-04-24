using System.Text;
using System.Text.Json;

namespace linco_core.Services
{
    public interface IAiService
    {
        Task<string> TranslateTextAsync(string text);
        Task<string> GenerateSentencesAsync(string word, string level);
        Task<string> GenerateTestAsync(string topic, string level);
    }

    public class AiService : IAiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public AiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Gemini:ApiKey"] ?? throw new ArgumentNullException("Gemini ApiKey is not set.");
        }

        private async Task<string> CallGeminiApiAsync(string prompt)
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-flash-latest:generateContent?key={_apiKey}";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, jsonContent);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            
            using var doc = JsonDocument.Parse(responseString);
            var root = doc.RootElement;
            var candidates = root.GetProperty("candidates");
            if (candidates.GetArrayLength() > 0)
            {
                var content = candidates[0].GetProperty("content");
                var parts = content.GetProperty("parts");
                if (parts.GetArrayLength() > 0)
                {
                    return parts[0].GetProperty("text").GetString() ?? "";
                }
            }

            return "";
        }

        public async Task<string> TranslateTextAsync(string text)
        {
            var prompt = $"You are a highly capable and natural translator. If the following text is in English, translate it to Turkish. If it is in Turkish, translate it to English. Only provide the translation, without any explanations or additional conversational text.\n\nText: {text}";
            var result = await CallGeminiApiAsync(prompt);
            return result.Trim();
        }

        public async Task<string> GenerateSentencesAsync(string word, string level)
        {
            var prompt = $"Generate exactly 5 sample sentences in English using the word '{word}'. The sentences MUST be suitable for an English learner at CEFR level {level}. Also provide the Turkish translation for each sentence below it. Format exactly like this, using HTML (return ONLY the HTML `<li>` tags):\n\n<li class=\"mb-3 text-start\"><strong>1. [English sentence here]</strong><br><span class=\"text-muted\">[Turkish translation here]</span></li>\n<li class=\"mb-3 text-start\"><strong>2. [English sentence here]</strong><br><span class=\"text-muted\">[Turkish translation here]</span></li>\n\nReturn only the `<li>` items list. Do not use markdown backticks ```html or any other conversational text. Ensure to include the word '{word}' in each English sentence.";
            var result = await CallGeminiApiAsync(prompt);
            // clean up possible markdown backticks if gemini still adds them
            result = result.Replace("```html", "").Replace("```", "").Trim();
            return result;
        }

        public async Task<string> GenerateTestAsync(string topic, string level)
        {
            var prompt = $"Generate exactly 20 multiple choice questions in English about the topic '{topic}'. The questions MUST be suitable for an English learner at CEFR level {level}. Ensure it is strictly formatted as a valid JSON array of objects. Do not include markdown code blocks, backticks, or any conversational text. Just the JSON array. Each object MUST have this exact structure: {{\n  \"question\": \"The English question text here?\",\n  \"options\": [\n    \"Option A\",\n    \"Option B\",\n    \"Option C\",\n    \"Option D\"\n  ],\n  \"correctAnswerIndex\": 2\n}}\nMake sure exactly 20 items are in the array.";
            var result = await CallGeminiApiAsync(prompt);
            
            // clean up possible markdown backticks
            result = result.Replace("```json", "").Replace("```", "").Trim();
            return result;
        }
    }
}
