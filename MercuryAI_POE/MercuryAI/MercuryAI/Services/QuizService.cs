using System;
using System.Collections.Generic;
using System.Linq;
using MercuryAI.Models;

namespace MercuryAI.Services
{
    public class QuizService
    {
        private static readonly Random _rand = new Random();

        private List<QuizQuestion> _questions = new List<QuizQuestion>();
        private int _currentIndex;
        private int _score;
        private bool _isActive;
        private bool _awaitingAnswer;

        public bool IsActive          => _isActive;
        public bool IsAwaitingAnswer  => _awaitingAnswer;
        public int  Score             => _score;
        public int  TotalQuestions    => _questions.Count;
        public QuizQuestion CurrentQuestion =>
            _currentIndex < _questions.Count ? _questions[_currentIndex] : null;

        private static readonly List<QuizQuestion> QuestionBank = new List<QuizQuestion>
        {
            new QuizQuestion
            {
                Question     = "What should you do if an email asks for your password?",
                Options      = new[] { "A. Reply with your password", "B. Delete the email", "C. Report it as phishing ✅", "D. Ignore it" },
                CorrectIndex = 2,
                Explanation  = "Reporting phishing emails helps your provider block similar attacks. Legitimate services will never ask for your password by email — that's a definitive red flag! 🚩",
                Topic        = "Phishing"
            },
            new QuizQuestion
            {
                Question     = "Which of the following is the STRONGEST password?",
                Options      = new[] { "A. password123", "B. John1990!", "C. P@ssw0rd", "D. Xk#9!mQ2$vL7@pR ✅" },
                CorrectIndex = 3,
                Explanation  = "Strong passwords are long (16+ chars), random, and mix upper/lower/numbers/symbols. Avoid anything tied to real life — attackers try those first! 🔑",
                Topic        = "Password Safety"
            },
            new QuizQuestion
            {
                Question     = "What does 2FA stand for?",
                Options      = new[] { "A. Two-File Access", "B. Two-Factor Authentication ✅", "C. Twice-Fast Application", "D. Two-Form Authorisation" },
                CorrectIndex = 1,
                Explanation  = "2FA adds a second verification step on top of your password — like a code to your phone. Even if someone steals your password, they still can't get in without that second factor! 🔐",
                Topic        = "Two-Factor Authentication"
            },
            new QuizQuestion
            {
                Question     = "What is ransomware?",
                Options      = new[] { "A. Software that speeds up your PC", "B. A type of firewall", "C. Malware that encrypts your files and demands payment ✅", "D. An online backup service" },
                CorrectIndex = 2,
                Explanation  = "Ransomware locks your files until you pay up. The best defence is regular offline backups — then you can restore and ignore the ransom demand entirely! 💾",
                Topic        = "Ransomware"
            },
            new QuizQuestion
            {
                Question     = "A stranger calls claiming to be from Microsoft and says your PC has a virus. What do you do?",
                Options      = new[] { "A. Give them remote access", "B. Provide your credit card to fix it", "C. Hang up immediately ✅", "D. Follow their instructions" },
                CorrectIndex = 2,
                Explanation  = "Tech-support scam! Microsoft never cold-calls you. Hang up straight away — no real company will ever call unsolicited to warn about your PC 🐵",
                Topic        = "Social Engineering"
            },
            new QuizQuestion
            {
                Question     = "Which URL is SAFER to visit for online banking?",
                Options      = new[] { "A. http://mybank.com", "B. https://mybank.com ✅", "C. http://mybank-secure.net", "D. They are all equally safe" },
                CorrectIndex = 1,
                Explanation  = "HTTPS encrypts your connection — always check for the padlock. But also verify the domain carefully, because HTTPS alone doesn't guarantee the site is legitimate! 🌐",
                Topic        = "Safe Browsing"
            },
            new QuizQuestion
            {
                Question     = "What is a Trojan horse in cybersecurity?",
                Options      = new[] { "A. A strong firewall", "B. Malware disguised as legitimate software ✅", "C. A type of VPN", "D. An encrypted file" },
                CorrectIndex = 1,
                Explanation  = "Just like the legend — it looks harmless on the outside, but once installed it opens the gates for attackers. Only install software from trusted sources! 🦠",
                Topic        = "Malware"
            },
            new QuizQuestion
            {
                Question     = "How often should you update your software and OS?",
                Options      = new[] { "A. Once a year", "B. Never — updates break things", "C. Only when something breaks", "D. As soon as updates are available ✅" },
                CorrectIndex = 3,
                Explanation  = "Updates patch known vulnerabilities that attackers exploit. Delaying a patch even a few days is one of the most common causes of successful breaches! 🔄",
                Topic        = "Cyber Hygiene"
            },
            new QuizQuestion
            {
                Question     = "You receive an email saying you've won a large cash prize. What do you do?",
                Options      = new[] { "A. Claim it by providing bank details", "B. Forward it to friends", "C. Delete it — almost certainly a scam ✅", "D. Click the link to verify" },
                CorrectIndex = 2,
                Explanation  = "If it sounds too good to be true, it is! Prize scams are designed to steal your personal and financial info. Delete and move on! ⚠️",
                Topic        = "Scams"
            },
            new QuizQuestion
            {
                Question     = "What is the purpose of a VPN?",
                Options      = new[] { "A. Speed up your internet", "B. Encrypt traffic and hide your IP ✅", "C. Block all ads", "D. Scan for viruses" },
                CorrectIndex = 1,
                Explanation  = "A VPN encrypts all your traffic so snoopers — whether on public Wi-Fi, your ISP, or elsewhere — can't read what you're doing online! 📶",
                Topic        = "Safe Browsing"
            },
            new QuizQuestion
            {
                Question     = "Which is the BEST way to protect your email account?",
                Options      = new[] { "A. Same password as other accounts", "B. Share credentials with a trusted friend", "C. Enable 2FA and use a unique strong password ✅", "D. Only log in from home" },
                CorrectIndex = 2,
                Explanation  = "Your email is the master key to everything — password resets all go there. Lock it down with 2FA and a unique password you don't use anywhere else! 🔒",
                Topic        = "Password Safety"
            },
            new QuizQuestion
            {
                Question     = "What is the 3-2-1 backup rule?",
                Options      = new[] { "A. Back up once a month", "B. 3 copies, 2 media types, 1 offsite ✅", "C. 3 passwords for every account", "D. Check backups once a year" },
                CorrectIndex = 1,
                Explanation  = "3 copies of your data, on 2 different media types, with 1 stored offsite. This protects you against hardware failure, ransomware, fire, and theft all at once! 💾",
                Topic        = "Cyber Hygiene"
            }
        };

        public string StartQuiz(int questionCount = 5)
        {
            _questions      = QuestionBank.OrderBy(_ => _rand.Next()).Take(questionCount).ToList();
            _currentIndex   = 0;
            _score          = 0;
            _isActive       = true;
            _awaitingAnswer = true;
            return BuildQuestionText(_questions[0], 1);
        }

        public QuizResult SubmitAnswer(string answer)
        {
            if (!_isActive || CurrentQuestion == null)
                return new QuizResult { Feedback = "No active quiz right now. Type 'start quiz' to begin! 🐵", IsFinished = true };

            var q = CurrentQuestion;
            int answerIndex = ParseAnswer(answer);

            if (answerIndex < 0)
                return new QuizResult
                {
                    Feedback    = "⚠ Please answer with A, B, C, or D.",
                    Score       = _score,
                    Total       = _questions.Count
                };

            bool correct = answerIndex == q.CorrectIndex;
            if (correct) _score++;

            _currentIndex++;
            bool finished   = _currentIndex >= _questions.Count;
            _awaitingAnswer = !finished;

            string nextQ   = finished ? string.Empty : "\n\n" + BuildQuestionText(_questions[_currentIndex], _currentIndex + 1);
            string summary = string.Empty;
            if (finished) { _isActive = false; summary = BuildFinalSummary(); }

            return new QuizResult
            {
                IsCorrect    = correct,
                Feedback     = correct ? "✅ Correct! Great job!" : $"❌ Not quite — the right answer was {OptionLetter(q.CorrectIndex)}.",
                Explanation  = q.Explanation + nextQ,
                Score        = _score,
                Total        = _questions.Count,
                IsFinished   = finished,
                FinalSummary = summary
            };
        }

        private static string BuildQuestionText(QuizQuestion q, int num) =>
            $"❓ Question {num}: {q.Question}\n\n" +
            string.Join("\n", q.Options) +
            $"\n\n(Topic: {q.Topic}) — Type A, B, C or D to answer.";

        private string BuildFinalSummary()
        {
            string rating;
            if (_score == _questions.Count)
                rating = "🏆 Perfect score! You're a cybersecurity superstar, Mercury approves! 🐵";
            else if (_score >= _questions.Count * 0.8)
                rating = "🥇 Excellent work — you clearly know your stuff!";
            else if (_score >= _questions.Count * 0.6)
                rating = "🥈 Good effort! A little more practice and you'll be unstoppable.";
            else if (_score >= _questions.Count * 0.4)
                rating = "🥉 Fair try — keep exploring the topics to boost your knowledge!";
            else
                rating = "📚 Keep learning — I'm here to help you get up to speed! 🐵";

            return $"🎯 Quiz Complete!\n\nYour Score: {_score}/{_questions.Count}\n{rating}" +
                   "\n\nType 'start quiz' to try again with a fresh set of questions.";
        }

        private static int ParseAnswer(string answer)
        {
            switch (answer.Trim().ToUpper())
            {
                case "A": case "1": return 0;
                case "B": case "2": return 1;
                case "C": case "3": return 2;
                case "D": case "4": return 3;
                default:            return -1;
            }
        }

        private static string OptionLetter(int index)
        {
            switch (index)
            {
                case 0: return "A";
                case 1: return "B";
                case 2: return "C";
                case 3: return "D";
                default: return "?";
            }
        }
    }
}
