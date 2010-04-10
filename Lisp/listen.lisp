(reference "C:\\Program Files\\Reference Assemblies\\Microsoft\\Framework\\v3.0\\System.Speech.dll")

(using "System.Speech.Recognition")

(.currentUIculture (Threading.Thread.CurrentThread))

(.set_currentUIculture (Threading.Thread.CurrentThread) (new "System.Globalization.CultureInfo" "en-GB"))

; convert a list to a string array
; is also defined in utilities.lisp
(def toStringArray (pos1)
   "Converts a list into an array of String[]"
   (with (temp (new "String[]" (len pos1)))
   (for i 0 (- (len pos1) 1)
         (.SetValue temp (nth pos1 i)  i)
   )
   temp
   )
)

;(= c (toStringArray '("stand" "open" "close" "exit")))

;Another way would be this ...
(= menu "stand:open:close:exit")
(= c (.split menu (.tochararray ":")))

(= g (new "Grammar" (new "GrammarBuilder" (new "Choices" c)))) 

(= rec (new "SpeechRecognitionEngine"))

(.loadgrammar rec g)

(.SetInputToDefaultAudioDevice rec)

(.Text (.Recognize rec))