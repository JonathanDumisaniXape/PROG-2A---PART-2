using System;
using System.Collections.Generic;
using System.Text;
using MercuryAI.Models;

namespace MercuryAI.Services
{
    public class ActivityLogService
    {
        private readonly List<LogEntry> _entries = new List<LogEntry>();

        public IList<LogEntry> AllEntries => _entries.AsReadOnly();

        public void LogTaskAdded(string taskName)              => Add("TASK",    "Task added",         taskName);
        public void LogTaskDeleted(int id, string taskName)    => Add("TASK",    "Task deleted",       $"ID {id}: {taskName}");
        public void LogTaskUpdated(int id, string taskName)    => Add("TASK",    "Task updated",       $"ID {id}: {taskName}");
        public void LogTaskViewed()                            => Add("TASK",    "Task list viewed",   string.Empty);
        public void LogReminderCreated(string name, DateTime t)=> Add("REMINDER","Reminder created",  $"\"{name}\" at {t:dd MMM yyyy HH:mm}");
        public void LogQuizStarted()                           => Add("QUIZ",    "Quiz started",       string.Empty);
        public void LogQuizAnswered(bool ok, int s, int t)     => Add("QUIZ",    ok ? "Answer correct" : "Answer incorrect", $"Score: {s}/{t}");
        public void LogQuizCompleted(int s, int t)             => Add("QUIZ",    "Quiz completed",     $"Final score: {s}/{t}");
        public void LogTopicExplored(string topic)             => Add("CHAT",    "Topic explored",     topic);
        public void LogSessionStarted()                        => Add("SYSTEM",  "Session started",    string.Empty);
        public void LogActivityLogViewed()                     => Add("SYSTEM",  "Activity log viewed",string.Empty);

        private void Add(string category, string action, string detail)
        {
            _entries.Add(new LogEntry { Category = category, Action = action, Detail = detail });
        }

        public string GetFormattedLog(int maxEntries = 20)
        {
            if (_entries.Count == 0)
                return "📭 No activity recorded yet — start chatting or run the quiz to get things going! 🐵";

            var sb = new StringBuilder();
            sb.AppendLine($"📜 MERCURY'S ACTIVITY LOG  ({_entries.Count} total event(s))");
            sb.AppendLine();
            sb.AppendLine("──────────────────────────────────────");

            int start = Math.Max(0, _entries.Count - maxEntries);
            for (int i = start; i < _entries.Count; i++)
                sb.AppendLine(_entries[i].ToString());

            if (_entries.Count > maxEntries)
                sb.AppendLine($"\n… and {_entries.Count - maxEntries} earlier event(s) not shown.");

            sb.AppendLine("──────────────────────────────────────");
            sb.Append($"Session duration: {GetSessionDuration()}");
            return sb.ToString();
        }

        public string GetSummary()
        {
            int tasks     = _entries.FindAll(e => e.Category == "TASK").Count;
            int quiz      = _entries.FindAll(e => e.Category == "QUIZ").Count;
            int chat      = _entries.FindAll(e => e.Category == "CHAT").Count;
            int reminders = _entries.FindAll(e => e.Category == "REMINDER").Count;

            return "📊 Session Summary\n\n" +
                   $"📋 Task events:     {tasks}\n" +
                   $"🎯 Quiz events:     {quiz}\n" +
                   $"💬 Chat events:     {chat}\n" +
                   $"⏰ Reminders set:   {reminders}\n" +
                   $"🕒 Duration:        {GetSessionDuration()}";
        }

        private string GetSessionDuration()
        {
            if (_entries.Count == 0) return "—";
            var elapsed = DateTime.Now - _entries[0].Timestamp;
            if (elapsed.TotalMinutes < 1) return "< 1 minute";
            if (elapsed.TotalHours   < 1) return $"{(int)elapsed.TotalMinutes} min";
            return $"{(int)elapsed.TotalHours}h {elapsed.Minutes}m";
        }
    }
}
