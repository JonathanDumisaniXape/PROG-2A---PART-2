using System;

namespace MercuryAI.Models
{
    public class LogEntry
    {
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Category { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;

        public string Emoji
        {
            get
            {
                switch (Category)
                {
                    case "TASK":     return "📋";
                    case "QUIZ":     return "🎯";
                    case "CHAT":     return "💬";
                    case "SYSTEM":   return "⚙";
                    case "REMINDER": return "⏰";
                    default:         return "•";
                }
            }
        }

        public override string ToString() =>
            $"{Emoji} [{Timestamp:HH:mm:ss}] {Category} — {Action}" +
            (string.IsNullOrWhiteSpace(Detail) ? string.Empty : $": {Detail}");
    }
}
