using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LSharp;
using System.Speech.Synthesis;
using System.Speech.Recognition;


namespace RobobuilderLib
{
    public class SpeechLib
    {
        SpeechSynthesizer speak;
        SpeechRecognitionEngine recog;
        LSharp.Environment gE;
        LSharp.Function lf = null;


        public SpeechLib(LSharp.Environment e, string f)
        {
            Console.WriteLine("speech lib constructor");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-GB");
            
            gE = e;

            speak = new SpeechSynthesizer();
            recog = new SpeechRecognitionEngine();
            recog.SetInputToDefaultAudioDevice();

            if (f != null && f != "")
            {
                Grammar g = new Grammar(f);
                recog.LoadGrammar(g);
            }

            gE.Set(Symbol.FromName("recog"), recog);
            gE.Set(Symbol.FromName("sp"),    speak);

        }

        public void InitRecogniser(Object[] x)
        {           
            if (x != null && x.Length>0)
            {

                Choices vm = new Choices();  //"left", "right", "up", "down"

                foreach (string verb in x)
                {
                    vm.Add(verb);
                }

                Grammar g = new Grammar(new GrammarBuilder(vm));

                recog.UnloadAllGrammars();
                recog.LoadGrammar(g);
            }          
        }

        public void InitRecogniser(string f, LSharp.Function z, RecognizeMode m)
        {
            try
            {
                Console.WriteLine("load grammer - " + f);
                Grammar g = new Grammar(f);
                g.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(SpeechRecognised);
                recog.LoadGrammar(g);

                Console.WriteLine("set function - " + z.Documentation + ": " + z.Signature + ", " +z.ToString());
                lf = z;

                Console.WriteLine("start recognition - " + m.ToString());
                recog.RecognizeAsync(m);
            }
            catch (Exception e1)
            {
                Console.WriteLine("initrecogniser failed - " + e1.Message);
            }
        }

        public string RecogniserStop(bool f) //cancel flag
        {
            if (f)
                recog.RecognizeAsyncStop();
            else
                recog.RecognizeAsyncCancel();

            return "ok";
        }


        public string SpeechRecognise()
        {
            return recog.Recognize().Text;
        }

        private void SpeechRecognised(object sender, RecognitionEventArgs e)
        {
            if (lf != null)
            {
                lf.Call(new object[] { e.Result });
            }

        }
    }
}
