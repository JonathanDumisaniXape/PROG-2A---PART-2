using System;
using System.Speech.Synthesis;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MercuryAI.Services
{
    public class AudioService : IDisposable
    {
        private SpeechSynthesizer _synth;
        private bool _enabled = true;

        public AudioService()
        {
            try
            {
                _synth = new SpeechSynthesizer();
                try { _synth.SelectVoice("Microsoft David Desktop"); }
                catch { /* fall back to default voice */ }
                _synth.Rate = 2;
                _synth.Volume = 90;
            }
            catch
            {
                _enabled = false;
            }
        }

        // Preserved from original: voice greeting on startup
        public async Task PlayVoiceGreetingAsync()
        {
            await SpeakAsync(
                "Hi there! Welcome to the Cybersecurity Awareness Bot. " +
                "I'm Mercury, here to help you stay safe online. " +
                "Can you please tell me your name?");
        }

        public async Task SpeakAsync(string text)
        {
            if (!_enabled || _synth == null || string.IsNullOrWhiteSpace(text))
                return;
            try
            {
                // Strip emoji / non-ASCII for speech engine
                var clean = Regex.Replace(text, @"[^\u0000-\u007F\s]", "");
                var idx = clean.IndexOfAny(new[] { '.', '!', '?' });
                var spoken = idx > 0 ? clean.Substring(0, idx + 1).Trim()
                           : clean.Length > 120 ? clean.Substring(0, 120)
                           : clean;
                await Task.Run(() => _synth.SpeakAsync(spoken));
            }
            catch { /* non-critical */ }
        }

        public void Dispose() => _synth?.Dispose();
    }
}