using System;

namespace MercuryAI.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public DateTime ReminderDate { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public override string ToString() =>
            $"[{Id}] {TaskName} — Due: {ReminderDate:dd MMM yyyy HH:mm} | {Status}";
    }
}
