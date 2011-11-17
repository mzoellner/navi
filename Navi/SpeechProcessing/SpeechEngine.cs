using System;
using System.Speech.Recognition;
using System.Speech.Synthesis;

namespace Navi.SpeechProcessing
{
    public class SpeechEngine
    {
        private SpeechRecognitionEngine _engine;
        private SpeechSynthesizer _synthesizer;

        public event EventHandler RepeatRecognized;

        public SpeechEngine()
        {
            _synthesizer = new SpeechSynthesizer();
            _engine = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-US"));
            _engine.SetInputToDefaultAudioDevice();

            Choices tagCommands = new Choices("again", "repeat");
            GrammarBuilder grammarBuilder = new GrammarBuilder();
            grammarBuilder.Append(tagCommands);
            
            Grammar g = new Grammar(grammarBuilder);
            _engine.LoadGrammar(g);

            //_engine.SpeechDetected +=new EventHandler<SpeechDetectedEventArgs>(OnSpeechDetected);
            //_engine.SpeechHypothesized += new EventHandler<SpeechHypothesizedEventArgs>(OnSpeechHypothesized);
            _engine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(OnSpeechRecognized);

            _synthesizer.SetOutputToDefaultAudioDevice();

            _engine.RecognizeAsync(RecognizeMode.Multiple);
        }

        void OnSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence > 0.93 && e.Result.Text == "repeat")
            {
                if (RepeatRecognized != null)
                    RepeatRecognized(this, EventArgs.Empty);

                Console.WriteLine("R " + e.Result.Text + ": " + e.Result.Confidence);
            }
            

            //var goodWords = e.Result.Words;

            //foreach (var word in goodWords)
            //{
            

            //    if (word.Text == "back")
            //    {
            //        _recorder.Start(0);
            //        _synthesizer.Speak("no woman no cry");
            //    }
            //    else if (word.Text == "forward")
            //        _recorder.Stop();
            //}

            
        }

        void OnSpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            var goodWords = e.Result.Words;

            foreach (var word in goodWords)
            {
                Console.WriteLine("H " + word.Text + ": " + word.Confidence);
            }
            
        }

        void OnSpeechDetected(object sender, SpeechDetectedEventArgs e)
        {
            
        }


        public void Speak(String text)
        {
            _synthesizer.SpeakAsync(text);
        }
    }
}
