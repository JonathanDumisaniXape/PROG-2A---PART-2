using System;
using System.Collections.Generic;
using MercuryAI.Models;

namespace MercuryAI.Services
{
    public class ChatService
    {
        private static readonly Random _rng = new Random();
        private readonly List<ChatTopic> _topics;

        // ── Random response lists ────────────────────────────────────
        public static readonly List<string> UnknownResponses = new List<string>
        {
            "🤔 I'm not sure I understand. Could you try rephrasing that?",
            "Hmm, I didn't quite catch that. Try a topic name or type 'help'.",
            "I'm still learning! Could you rephrase, or type 'help' to see topics?",
            "That one has me stumped. Try keywords like 'phishing', 'password', or 'malware'."
        };

        public static readonly List<string> EmptyInputResponses = new List<string>
        {
            "Looks like you didn't type anything. Give it another go! 😊",
            "Don't be shy — type a question or topic and I'll help!",
            "I need a little input from you. What would you like to know?"
        };

        public static readonly List<string> Acknowledgements = new List<string>
        {
            "Great question! Here's what you need to know:",
            "Absolutely! Let me break that down for you:",
            "Sure thing! Here's some important information:",
            "Good thinking — cybersecurity awareness starts here:"
        };

        public ChatService()
        {
            _topics = BuildTopics();
        }

        public string GetResponse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return GetRandom(EmptyInputResponses);

            var lower = input.ToLower().Trim();

            if (lower.Contains("about") || lower.Contains("who are you") || lower.Contains("what are you"))
                return AboutMercury();

            foreach (var topic in _topics)
                foreach (var kw in topic.Keywords)
                    if (lower.Contains(kw))
                        return $"{GetRandom(Acknowledgements)}\n\n{GetRandom(new List<string>(topic.Responses))}";

            return GetRandom(UnknownResponses);
        }

        public List<ChatTopic> GetAllTopics() => _topics;

        private static string GetRandom(List<string> list)
        {
            if (list == null || list.Count == 0) return string.Empty;
            return list[_rng.Next(list.Count)];
        }

        private string AboutMercury() =>
            "🤖 ABOUT MERCURY AI\n\n" +
            "I'm Mercury — your Cybersecurity Awareness Bot built with C# and WPF.\n\n" +
            "I can help with:\n" +
            "• Phishing & Scams\n• Password Security\n• Two-Factor Authentication\n" +
            "• Ransomware\n• Social Engineering\n• Safe Browsing\n" +
            "• Malware Protection\n• Cyber Hygiene\n• Privacy & More\n\n" +
            "Type 'help' to see all topics!";

        // ── Topic knowledge base (all original topics + expanded) ────
        private List<ChatTopic> BuildTopics() => new List<ChatTopic>
        {
            new ChatTopic
            {
                Name = "Phishing", Emoji = "🎣",
                Keywords = new[] { "phishing", "phish", "fake email", "email scam", "spoofed" },
                Responses = new[]
                {
                    // Preserved from original, expanded
                    "🎣 PHISHING\n\nPhishing is a cyber scam where attackers trick people into giving away private information like passwords, usernames, or banking details by pretending to be trusted sources. This usually happens through fake emails, messages, or websites that look real. These messages often create urgency to pressure you into acting quickly.\n\n✅ Stay Safe:\n• Avoid clicking on suspicious links or downloading unknown attachments\n• Always double-check the sender's email address\n• Confirm through official channels before sharing any personal information\n• Look for HTTPS and correct spelling in URLs\n\n🔴 Types of Phishing:\n• Email Phishing — mass fake emails\n• Spear Phishing — targeted attacks using your real name\n• Smishing — phishing via SMS\n• Vishing — phishing via phone call",
                    "🎣 PHISHING — Did You Know?\n\nOver 3.4 billion phishing emails are sent every single day. It's the #1 cause of data breaches worldwide.\n\n🛡️ Golden Rule: Legitimate organisations NEVER ask for your password, OTP, or PIN via email or phone.\n\nWhen in doubt — go directly to the company's website by typing the address yourself, and call their official number."
                }
            },
            new ChatTopic
            {
                Name = "Password Security", Emoji = "🔑",
                Keywords = new[] { "password", "passwords", "pass word", "credentials", "passphrase" },
                Responses = new[]
                {
                    "🔑 PASSWORD SECURITY\n\nKeeping your passwords secure is essential for protecting your accounts. A strong password should be long and include a mix of uppercase letters, lowercase letters, numbers, and symbols. Avoid using the same password for multiple accounts.\n\n✅ Tips:\n• Use at least 12–16 characters\n• Never reuse passwords across different sites\n• Use a password manager like Bitwarden (free) to store and generate passwords\n• Update passwords regularly and enable 2FA\n\n❌ Avoid: 'password123', your name, birthdays, or pet names.\n\n💡 Check haveibeenpwned.com to see if your email was in a known breach.",
                    "🔑 PASSWORD SECURITY — Pro Tips\n\n81% of data breaches involve weak or stolen passwords.\n\n🧪 Password Manager Benefits:\n• Generates unique strong passwords for every site\n• You only remember ONE master password\n• Alerts you if your credentials appear in a data breach\n\nFree options: Bitwarden, KeePass\n⚠️ Never share your password with anyone — ever."
                }
            },
            new ChatTopic
            {
                Name = "Two-Factor Authentication", Emoji = "🔐",
                Keywords = new[] { "two-factor", "2fa", "mfa", "multi-factor", "authentication", "otp", "one-time" },
                Responses = new[]
                {
                    "🔐 TWO-FACTOR AUTHENTICATION (2FA)\n\nTwo-factor authentication adds an extra level of security to your accounts. It requires both your password and a second verification step, like a code sent to your phone or generated by an app. Even if someone gets your password, they won't be able to access your account without the second step.\n\n✅ 2FA Types (Best to Weakest):\n1. Hardware Key (YubiKey)\n2. Authenticator App (Google Authenticator, Authy)\n3. Email code\n4. SMS code — still better than nothing\n\n🔒 Enable 2FA on: Email, Banking, Social Media, Work accounts\n💡 Never share your OTP with anyone — ever."
                }
            },
            new ChatTopic
            {
                Name = "Ransomware", Emoji = "🔒",
                Keywords = new[] { "ransomware", "ransom", "encrypted files", "locked files", "held hostage" },
                Responses = new[]
                {
                    "🔒 RANSOMWARE\n\nRansomware is harmful software that locks or encrypts your files and demands payment to restore access. It usually spreads through unsafe downloads, emails, or infected websites.\n\n✅ Prevention:\n• Regularly back up your data (3-2-1 rule: 3 copies, 2 media types, 1 offsite)\n• Avoid suspicious links and unexpected attachments\n• Keep your system and antivirus software updated\n\n⚠️ If Hit:\n• Do NOT pay the ransom — it's not guaranteed to work and funds criminals\n• Disconnect from the network immediately\n• Contact a cybersecurity professional"
                }
            },
            new ChatTopic
            {
                Name = "Social Engineering", Emoji = "🎭",
                Keywords = new[] { "social engineering", "social", "manipulation", "pretexting", "baiting" },
                Responses = new[]
                {
                    "🎭 SOCIAL ENGINEERING\n\nSocial engineering is when attackers manipulate people into sharing confidential information. They often take advantage of emotions like fear or urgency to trick victims.\n\n🔴 Common Tactics:\n• Pretexting — 'Hi, I'm from IT. I need your password.'\n• Baiting — infected USB drives left in public hoping you'll plug one in\n• Tailgating — following someone through a secure door\n• Urgency — 'Act now or your account is deleted!'\n\n✅ Defence:\n• Always verify requests for sensitive information\n• Be cautious of unexpected messages\n• When unsure, confirm directly with the source via official channels"
                }
            },
            new ChatTopic
            {
                Name = "Safe Browsing", Emoji = "🌐",
                Keywords = new[] { "safe browsing", "browsing", "browser", "website", "https", "vpn", "web", "internet" },
                Responses = new[]
                {
                    "🌐 SAFE INTERNET BROWSING\n\nSafe browsing means using the internet carefully to avoid threats. Always visit secure websites (HTTPS), avoid suspicious downloads, and keep your browser updated.\n\n✅ Key Practices:\n• Look for HTTPS 🔒 in the URL — never submit info on HTTP sites\n• Use an ad blocker (uBlock Origin — free)\n• Keep your browser updated\n• Only download software from official sources\n\n🛡️ Privacy Tools:\n• VPN — encrypts your connection, especially on public Wi-Fi\n• Privacy browser: Firefox or Brave\n• Search engine: DuckDuckGo"
                }
            },
            new ChatTopic
            {
                Name = "Malware Protection", Emoji = "🦠",
                Keywords = new[] { "malware", "virus", "trojan", "spyware", "adware", "worm", "keylogger", "antivirus" },
                Responses = new[]
                {
                    "🦠 MALWARE PROTECTION\n\nMalware refers to harmful software designed to damage or exploit systems. This includes viruses, spyware, Trojans, worms, and keyloggers.\n\n✅ Protection:\n• Use updated antivirus software (Windows Defender is solid for home use)\n• Avoid untrusted downloads\n• Keep your operating system and all apps updated\n• Never plug in unknown USB drives\n\n🔴 If Infected:\n• Run a full system scan with antivirus\n• Disconnect from the internet\n• Consider using dedicated malware removal tools like Malwarebytes"
                }
            },
            new ChatTopic
            {
                Name = "Cyber Hygiene", Emoji = "🧹",
                Keywords = new[] { "cyber hygiene", "hygiene", "digital habits", "security habits", "daily security" },
                Responses = new[]
                {
                    "🧹 CYBER HYGIENE\n\nCyber hygiene involves daily habits that help keep your digital life secure, like updating software, using strong passwords, and enabling 2FA.\n\n✅ Daily Habits:\n• Lock your screen when stepping away\n• Think before clicking any link\n• Log out of accounts on shared devices\n\n📅 Weekly/Monthly:\n• Check for OS and app updates\n• Review app permissions on your phone\n• Back up important files\n• Check bank statements for unusual activity\n\n📅 Annually:\n• Review and update all passwords\n• Check haveibeenpwned.com for breaches\n• Review social media privacy settings"
                }
            },
            new ChatTopic
            {
                Name = "Privacy Protection", Emoji = "🔏",
                Keywords = new[] { "privacy", "private", "personal data", "tracking", "data collection" },
                Responses = new[]
                {
                    "🔏 PRIVACY PROTECTION\n\nYour personal data is extremely valuable. Companies and criminals both want it.\n\n✅ Protect Yourself:\n• Review privacy settings on ALL social media accounts\n• Audit app permissions — Location, Camera, Microphone\n• Use a VPN on public Wi-Fi\n• Use Signal for private messaging\n• Use a private email alias for sign-ups\n\n💡 Golden Rule: If a service is free, YOUR data is the product."
                }
            },
            new ChatTopic
            {
                Name = "Scams", Emoji = "⚠️",
                Keywords = new[] { "scam", "scams", "fraud", "fake", "lottery", "prize", "romance scam" },
                Responses = new[]
                {
                    "⚠️ SCAM AWARENESS\n\nScams exploit urgency, trust, and emotion to trick victims.\n\n🔴 Common Types:\n• Lottery Scams — 'You've won! Pay a fee to claim.'\n• Romance Scams — fake relationships to extract money\n• Tech Support Scams — 'Your PC has a virus, call us!'\n• Investment Scams — guaranteed huge returns on crypto\n• Impersonation Scams — criminals pretending to be your bank or SARS\n\n✅ Red Flags:\n• Requests for gift cards or cryptocurrency\n• Extreme urgency or fear tactics\n• Requests for your OTP, PIN, or banking details\n\n💡 If Scammed: Contact your bank immediately and report to SAPS Cybercrime."
                }
            },
            new ChatTopic
            {
                Name = "Wi-Fi Security", Emoji = "📶",
                Keywords = new[] { "wifi", "wi-fi", "wireless", "public wifi", "hotspot", "router" },
                Responses = new[]
                {
                    "📶 WI-FI SECURITY\n\n🏠 Home Router:\n• Change the default admin username and password\n• Use WPA3 or WPA2 encryption (never WEP)\n• Keep router firmware updated\n• Create a separate guest network for visitors\n\n☕ Public Wi-Fi:\n• Never access banking on public Wi-Fi\n• Always use a VPN on public networks\n• Disable auto-connect to open networks"
                }
            },
            new ChatTopic
            {
                Name = "Identity Theft", Emoji = "🪪",
                Keywords = new[] { "identity theft", "identity", "stolen identity", "id theft" },
                Responses = new[]
                {
                    "🪪 IDENTITY THEFT PREVENTION\n\nIdentity theft occurs when a criminal uses your personal info to commit fraud in your name.\n\n✅ Protect Yourself:\n• Shred sensitive documents before discarding\n• Monitor bank statements regularly\n• Check your credit report for suspicious activity\n• Never carry your ID unless necessary\n• Place a fraud alert if you suspect theft"
                }
            }
        };
    }
}