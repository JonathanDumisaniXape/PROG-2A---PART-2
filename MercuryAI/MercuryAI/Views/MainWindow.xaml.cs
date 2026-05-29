using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using MercuryAI.Models;
using MercuryAI.Services;

namespace MercuryAI.Views
{
    public partial class MainWindow : Window
    {
        private readonly AudioService _audio;
        private readonly ChatService _chat;
        private readonly MemoryService _memory;
        private readonly SentimentAnalyzer _sentiment;

        private enum State { AwaitingName, Active }
        private State _state = State.AwaitingName;

        private static readonly List<(string Emoji, string Key)> _chips =
            new List<(string, string)>
        {
            ("🎣", "phishing"),
            ("🔑", "password"),
            ("🔐", "2fa"),
            ("🔒", "ransomware"),
            ("🎭", "social engineering"),
            ("🌐", "safe browsing"),
            ("🦠", "malware"),
            ("🧹", "cyber hygiene"),
            ("🔏", "privacy"),
            ("⚠️",  "scams"),
            ("📶", "wifi"),
            ("🪪", "identity theft"),
            ("🤖", "about mercury"),
        };

        public MainWindow()
        {
            InitializeComponent();
            _audio = new AudioService();
            _chat = new ChatService();
            _memory = new MemoryService();
            _sentiment = new SentimentAnalyzer();

            BuildTopicChips();
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            AppendMercuryMessage(AsciiArt.GetLogo(), monospace: true);
            await Task.Delay(300);
            AppendMercuryMessage(
                "Hi! I'm MERCURY, your cybersecurity assistant. 🐵\n\n" +
                "I'm here to help you stay safe online.\n\n" +
                "What should I call you? Please type your name:");
            await _audio.PlayVoiceGreetingAsync();
            UserInput.Focus();
        }

        private void BuildTopicChips()
        {
            var ti = System.Globalization.CultureInfo.CurrentCulture.TextInfo;
            foreach (var (emoji, key) in _chips)
            {
                var btn = new Button
                {
                    Content = $"{emoji} {ti.ToTitleCase(key)}",
                    Style = (Style)FindResource("TopicChip"),
                    Tag = key
                };
                btn.Click += TopicChip_Click;
                TopicPanel.Children.Add(btn);
            }
        }

        private async void BtnSend_Click(object sender, RoutedEventArgs e) => await ProcessInput();
        private async void UserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) await ProcessInput();
        }
        private async void TopicChip_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string key)
            {
                UserInput.Text = key;
                await ProcessInput();
            }
        }

        private async Task ProcessInput()
        {
            var raw = UserInput.Text?.Trim();
            UserInput.Clear();

            try
            {
                if (string.IsNullOrWhiteSpace(raw))
                {
                    var list = ChatService.EmptyInputResponses;
                    AppendMercuryMessage(list[new Random().Next(list.Count)]);
                    return;
                }

                AppendUserMessage(raw);
                await ShowTypingDots();

                if (_state == State.AwaitingName)
                {
                    HandleName(raw);
                    return;
                }

                await HandleActive(raw);
            }
            catch (Exception ex)
            {
                AppendMercuryMessage($"⚠️ Something went wrong: {ex.Message}. Please try again.");
            }
        }

        private void HandleName(string input)
        {
            if (input.Length > 50 || HasDigits(input))
            {
                AppendMercuryMessage("That doesn't look like a name. Please enter your first name:");
                return;
            }

            _memory.SetUser(input);
            _state = State.Active;

            var s = _memory.Session;
            AppendMercuryMessage(
                $"Great to meet you, {s.UserName}! 🐵\n\n" +
                $"{s.TimeGreeting}! Let's get started.\n\n" +
                GetHelpText());

            _ = _audio.SpeakAsync($"Let's get started, {s.UserName}!");
            UpdateUI();
        }

        private async Task HandleActive(string input)
        {
            var lower = input.ToLower().Trim();
            var name = _memory.Session.UserName;

            if (lower == "stop" || lower == "exit" || lower == "quit" || lower == "bye")
            {
                AppendMercuryMessage($"Alright, goodbye {name}! Stay safe and stay secure! 🔒");
                _ = _audio.SpeakAsync($"Goodbye {name}! Stay safe!");
                await Task.Delay(1500);
                Application.Current.Shutdown();
                return;
            }

            if (lower == "menu" || lower == "help" || lower == "topics")
            {
                AppendMercuryMessage(GetHelpText());
                return;
            }

            if (lower.Contains("what do you remember") || lower == "memory")
            {
                AppendMercuryMessage(
                    $"🧠 Here's what I remember about you, {name}:\n\n" +
                    $"• {_memory.GetMemorySummary()}");
                return;
            }

            if (lower.Contains("how has your day") || lower.Contains("how are you"))
            {
                AppendMercuryMessage(
                    $"I'm doing great, {name} — I'm a bot so I'm always at 100%! 😄 " +
                    $"What cybersecurity topic can I help you with?\n\n{GetHelpText()}");
                return;
            }

            var sentiment = _sentiment.Analyze(lower);
            string prefix = string.Empty;
            if (sentiment != Sentiment.Neutral)
            {
                _memory.SetSentiment(_sentiment.SentimentToString(sentiment));
                prefix = _sentiment.GetResponse(sentiment) + "\n\n";
                UpdateUI();
            }

            var response = _chat.GetResponse(input);

            foreach (var topic in _chat.GetAllTopics())
                foreach (var kw in topic.Keywords)
                    if (lower.Contains(kw))
                    {
                        _memory.RecordTopic(topic.Name);
                        UpdateUI();
                        break;
                    }

            AppendMercuryMessage(prefix + response);
            _ = _audio.SpeakAsync(response);
        }

        private void AppendMercuryMessage(string text, bool monospace = false)
        {
            var bubble = new Border
            {
                Margin = new Thickness(0, 5, 80, 5),
                Padding = new Thickness(14, 12, 14, 12),
                Background = new SolidColorBrush(Color.FromRgb(0x0C, 0x15, 0x25)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(0x00, 0xE5, 0xFF)),
                BorderThickness = new Thickness(3, 0, 0, 0),
                CornerRadius = new CornerRadius(0, 8, 8, 8)
            };
            bubble.Effect = new DropShadowEffect
            {
                Color = Color.FromRgb(0x00, 0xE5, 0xFF),
                BlurRadius = 10,
                ShadowDepth = 0,
                Opacity = 0.12
            };

            var sp = new StackPanel();
            sp.Children.Add(new TextBlock
            {
                Text = "🐵  MERCURY AI",
                Foreground = new SolidColorBrush(Color.FromRgb(0x00, 0xE5, 0xFF)),
                FontSize = 9.5,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 6)
            });
            sp.Children.Add(new TextBlock
            {
                Text = text,
                Foreground = new SolidColorBrush(Color.FromRgb(0xDD, 0xE3, 0xF0)),
                FontSize = monospace ? 10.5 : 13,
                FontFamily = monospace ? new FontFamily("Consolas") : new FontFamily("Segoe UI"),
                TextWrapping = TextWrapping.Wrap,
                LineHeight = monospace ? 16 : 22
            });
            sp.Children.Add(new TextBlock
            {
                Text = DateTime.Now.ToString("HH:mm"),
                Foreground = new SolidColorBrush(Color.FromRgb(0x7A, 0x84, 0x98)),
                FontSize = 9.5,
                Margin = new Thickness(0, 6, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Right
            });

            bubble.Child = sp;
            FadeIn(bubble);
        }

        private void AppendUserMessage(string text)
        {
            var name = _memory.Session.UserName;
            var bubble = new Border
            {
                Margin = new Thickness(80, 5, 0, 5),
                Padding = new Thickness(14, 12, 14, 12),
                Background = new SolidColorBrush(Color.FromRgb(0x08, 0x20, 0x14)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(0x00, 0xE6, 0x76)),
                BorderThickness = new Thickness(0, 0, 3, 0),
                CornerRadius = new CornerRadius(8, 0, 8, 8),
                HorizontalAlignment = HorizontalAlignment.Right
            };

            var sp = new StackPanel();
            sp.Children.Add(new TextBlock
            {
                Text = $"👤  {name}",
                Foreground = new SolidColorBrush(Color.FromRgb(0x00, 0xE6, 0x76)),
                FontSize = 9.5,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 6),
                HorizontalAlignment = HorizontalAlignment.Right
            });
            sp.Children.Add(new TextBlock
            {
                Text = text,
                Foreground = new SolidColorBrush(Color.FromRgb(0xDD, 0xE3, 0xF0)),
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap,
                LineHeight = 22,
                HorizontalAlignment = HorizontalAlignment.Right
            });
            sp.Children.Add(new TextBlock
            {
                Text = DateTime.Now.ToString("HH:mm"),
                Foreground = new SolidColorBrush(Color.FromRgb(0x7A, 0x84, 0x98)),
                FontSize = 9.5,
                Margin = new Thickness(0, 6, 0, 0)
            });

            bubble.Child = sp;
            FadeIn(bubble);
        }

        private async Task ShowTypingDots()
        {
            var dots = new Border
            {
                Margin = new Thickness(0, 5, 80, 5),
                Padding = new Thickness(14, 10, 14, 10),
                Background = new SolidColorBrush(Color.FromRgb(0x0C, 0x15, 0x25)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(0x16, 0x20, 0x30)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8)
            };
            dots.Child = new TextBlock
            {
                Text = "Mercury is typing  ●  ●  ●",
                Foreground = new SolidColorBrush(Color.FromRgb(0x7A, 0x84, 0x98)),
                FontSize = 11
            };
            ChatPanel.Children.Add(dots);
            ScrollBottom();
            await Task.Delay(650);
            ChatPanel.Children.Remove(dots);
        }

        private void FadeIn(UIElement el)
        {
            el.Opacity = 0;
            ChatPanel.Children.Add(el);
            ScrollBottom();
            el.BeginAnimation(UIElement.OpacityProperty,
                new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(220)));
        }

        private void ScrollBottom()
        {
            ChatScroll.UpdateLayout();
            ChatScroll.ScrollToEnd();
        }

        private async void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ChatPanel.Children.Clear();
            AppendMercuryMessage(
                $"Chat cleared! What would you like to explore next, " +
                $"{_memory.Session.UserName}? Type 'help' to see all topics.");
            await Task.CompletedTask;
        }

        private async void BtnHelp_Click(object sender, RoutedEventArgs e)
        {
            UserInput.Text = "help";
            await ProcessInput();
        }

        private async void BtnMemory_Click(object sender, RoutedEventArgs e)
        {
            UserInput.Text = "what do you remember";
            await ProcessInput();
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                $"Are you sure you want to exit, {_memory.Session.UserName}?\n\nStay safe online! 🔒",
                "Mercury AI",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
                Application.Current.Shutdown();
        }

        private void UpdateUI()
        {
            MemoryLabel.Text = "• " + _memory.GetMemorySummary().Replace("\n", "\n• ");
            if (_state == State.Active)
                StatusLabel.Text = $"Chatting with {_memory.Session.UserName}";
        }

        private string GetHelpText() =>
            "Here's what I can help you with today:\n\n" +
            "  🎣  1.  Phishing\n" +
            "  🔑  2.  Password Security\n" +
            "  🔐  3.  Two-Factor Authentication\n" +
            "  🔒  4.  Ransomware\n" +
            "  🎭  5.  Social Engineering\n" +
            "  🌐  6.  Safe Internet Browsing\n" +
            "  🦠  7.  Malware Protection\n" +
            "  🧹  8.  Cyber Hygiene\n" +
            "  🔏  9.  Privacy Protection\n" +
            "  ⚠️ 10.  Scam Awareness\n" +
            "  📶 11.  Wi-Fi Security\n" +
            "  🪪 12.  Identity Theft Prevention\n" +
            "  🤖 13.  About Mercury\n\n" +
            "💡 Type a topic name, number, or just ask me anything!";

        private static bool HasDigits(string s)
        {
            foreach (char c in s)
                if (char.IsDigit(c)) return true;
            return false;
        }

        protected override void OnClosed(EventArgs e)
        {
            _audio?.Dispose();
            base.OnClosed(e);
        }
    }
}