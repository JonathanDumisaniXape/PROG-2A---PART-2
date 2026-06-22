using System.Collections.Generic;

namespace MercuryAI.Services
{
    public enum Sentiment { Neutral, Happy, Sad, Angry, Stressed, Tired, Scared, Confused }

    public class SentimentAnalyzer
    {
        private static readonly Dictionary<Sentiment, List<string>> _keywords =
            new Dictionary<Sentiment, List<string>>
            {
                [Sentiment.Happy] = new List<string> { "happy", "great", "awesome", "good", "wonderful", "fantastic", "excited", "amazing", "love", "excellent", "glad" },
                [Sentiment.Sad] = new List<string> { "sad", "unhappy", "depressed", "miserable", "terrible", "bad", "awful", "upset", "down", "heartbroken" },
                [Sentiment.Angry] = new List<string> { "angry", "mad", "furious", "annoyed", "frustrated", "rage", "hate", "livid", "irritated" },
                [Sentiment.Stressed] = new List<string> { "stressed", "stress", "overwhelmed", "anxious", "worried", "nervous", "panic", "pressure", "tense" },
                [Sentiment.Tired] = new List<string> { "tired", "exhausted", "sleepy", "fatigue", "drained", "bored" },
                [Sentiment.Scared] = new List<string> { "scared", "afraid", "terrified", "hacked", "compromised", "attacked", "fear", "concerned" },
                [Sentiment.Confused] = new List<string> { "confused", "lost", "unsure", "not sure", "don't understand", "explain", "how does" }
            };

        private static readonly Dictionary<Sentiment, string> _responses =
            new Dictionary<Sentiment, string>
            {
                [Sentiment.Happy] = "😊 That's great to hear! Let's keep that energy going while staying cyber-safe.",
                [Sentiment.Sad] = "💙 Sorry you're feeling down. I'll keep things clear and simple — you've got this!",
                [Sentiment.Angry] = "😤 I understand the frustration. Cyber issues can be infuriating. Let's sort it out together.",
                [Sentiment.Stressed] = "🧘 Take a breath — I'll break everything down step by step. No jargon, I promise.",
                [Sentiment.Tired] = "😴 No worries, I'll keep it short and to the point!",
                [Sentiment.Scared] = "🛡️ It's okay to feel worried — awareness is your best defence. I'm here to help.",
                [Sentiment.Confused] = "🤔 Great question! Let me explain that as clearly as possible.",
                [Sentiment.Neutral] = ""
            };

        public Sentiment Analyze(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return Sentiment.Neutral;
            var lower = input.ToLower();
            foreach (var kvp in _keywords)
                foreach (var kw in kvp.Value)
                    if (lower.Contains(kw))
                        return kvp.Key;
            return Sentiment.Neutral;
        }

        public string GetResponse(Sentiment s) =>
            _responses.TryGetValue(s, out var r) ? r : string.Empty;

        public string SentimentToString(Sentiment s) => s.ToString().ToLower();
    }
}