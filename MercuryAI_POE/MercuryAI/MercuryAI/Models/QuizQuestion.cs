namespace MercuryAI.Models
{
    public class QuizQuestion
    {
        public string Question { get; set; } = string.Empty;
        public string[] Options { get; set; } = new string[0];
        public int CorrectIndex { get; set; }
        public string Explanation { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
    }

    public class QuizResult
    {
        public bool IsCorrect { get; set; }
        public string Feedback { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
        public int Score { get; set; }
        public int Total { get; set; }
        public bool IsFinished { get; set; }
        public string FinalSummary { get; set; } = string.Empty;
    }
}
