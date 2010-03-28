(reference "C:\\Program Files\\Reference Assemblies\\Microsoft\\Framework\\v3.0\\System.Speech.dll")

(using "System.Speech.Recognition")

(.currentUIculture (Threading.Thread.CurrentThread))

(.set_currentUIculture (Threading.Thread.CurrentThread) (new "System.Globalization.CultureInfo" "en-GB"))

(= menu "stand:open:close:exit")

(= c (.split menu (.tochararray ":")))
(= g (new "Grammar" (new "GrammarBuilder" (new "Choices" c)))) 

(= rec (new "SpeechRecognitionEngine"))

(.loadgrammar rec g)

(.SetInputToDefaultAudioDevice rec)

(.Text (.Recognize rec))