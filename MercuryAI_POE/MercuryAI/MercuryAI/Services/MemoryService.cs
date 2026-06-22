using MercuryAI.Models;

namespace MercuryAI.Services
{
    public class MemoryService
    {
        public UserSession Session { get; private set; } = new UserSession();

        public void SetUser(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return;
            Session.UserName = char.ToUpper(name[0]) + name.Substring(1).ToLower();
        }

        public void RecordTopic(string topic) => Session.RecordTopic(topic);
        public void SetSentiment(string mood) => Session.LastSentiment = mood;
        public string GetMemorySummary() => Session.GetMemorySummary();
    }
}