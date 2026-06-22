using System;
using System.Collections.Generic;
using MercuryAI.Models;

namespace MercuryAI.Services
{
    public class TaskService
    {
        private readonly List<TaskItem> _tasks = new List<TaskItem>();
        private int _nextId = 1;

        public bool IsDatabaseAvailable => false;

        public TaskItem AddTask(string taskName, DateTime reminderDate, out bool success, out string message)
        {
            if (string.IsNullOrWhiteSpace(taskName))
            {
                success = false; message = "Task name cannot be empty."; return null;
            }

            var task = new TaskItem
            {
                Id = _nextId++, TaskName = taskName.Trim(),
                ReminderDate = reminderDate, Status = "Pending", CreatedAt = DateTime.Now
            };

            _tasks.Add(task);
            success = true;
            message = $"✅ Task added! ID: {task.Id}";
            return task;
        }

        public List<TaskItem> GetAllTasks(out bool success, out string message)
        {
            success = true;
            message = $"Found {_tasks.Count} task(s).";
            return new List<TaskItem>(_tasks);
        }

        public bool UpdateTask(int id, string newName, DateTime newDate, string newStatus, out string message)
        {
            if (string.IsNullOrWhiteSpace(newName)) { message = "Task name cannot be empty."; return false; }
            var t = _tasks.Find(x => x.Id == id);
            if (t == null) { message = $"⚠ No task found with ID {id}."; return false; }
            t.TaskName = newName.Trim(); t.ReminderDate = newDate; t.Status = newStatus;
            message = $"✅ Task {id} updated successfully.";
            return true;
        }

        public bool DeleteTask(int id, out string message)
        {
            int removed = _tasks.RemoveAll(x => x.Id == id);
            if (removed > 0) { message = $"🗑 Task {id} deleted."; return true; }
            message = $"⚠ No task found with ID {id}."; return false;
        }

        public static bool TryParseTaskFromMessage(string message, out string taskName)
        {
            var lower = message.ToLower();
            string[] triggers = {
                "remind me to ", "add task ", "create task ",
                "set reminder for ", "set reminder to ", "new task ", "schedule task "
            };
            foreach (var trigger in triggers)
            {
                int idx = lower.IndexOf(trigger, StringComparison.OrdinalIgnoreCase);
                if (idx >= 0)
                {
                    var ex = message.Substring(idx + trigger.Length).Trim();
                    if (!string.IsNullOrWhiteSpace(ex))
                    { taskName = char.ToUpper(ex[0]) + ex.Substring(1); return true; }
                }
            }
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "enable 2fa",        "Enable Two-Factor Authentication" },
                { "enable two-factor", "Enable Two-Factor Authentication" },
                { "update password",   "Update Password" },
                { "change password",   "Update Password" },
                { "review privacy",    "Review Privacy Settings" },
                { "install antivirus", "Install Antivirus Software" },
                { "backup data",       "Back Up Important Data" },
                { "update software",   "Update Software & OS" },
            };
            foreach (var kvp in map)
                if (lower.Contains(kvp.Key)) { taskName = kvp.Value; return true; }
            taskName = string.Empty; return false;
        }
    }
}
