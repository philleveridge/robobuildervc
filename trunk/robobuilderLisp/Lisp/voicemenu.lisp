;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
;  L# software using RobobuilderLib to control
;  Humanoid robot - Robobuilder remotely via serial port
;  Using voice commands with voice response
;
;  l3v3rz - March 2010
;
;  http://en.wikiquote.org/wiki/Lisp_programming_language
;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

;
; vocab
; ===============
; exit
; stand
; open
; close
; extend
; retract
; left
; right
; swing
; out
; in
; learn
; remember
; showme
;
; <verb> left | right
;    extend 
;    open
;    close
;    retract
;    swing
;
(if (not (bound 'run_robobuilder))
     (do 
     (load "Lisp\\final.lisp")
     (run_robobuilder "COM3")
     )
)

(if (not (bound 'exit?))
     (load "Lisp\\utilities.lisp")
)

(if (not (bound 'standup))
  (load "Lisp\\wckutils18.lisp")
)

(if (not (bound 'open_gripper))
  (load "Lisp\\grip.lisp")
)

(reference "C:\\Program Files\\Reference Assemblies\\Microsoft\\Framework\\v3.0\\System.Speech.dll")

(using "System.Speech.Synthesis")
(using "System.Speech.Recognition")

(= sp (new "SpeechSynthesizer"))

(.currentUIculture (Threading.Thread.CurrentThread))

;(.SelectVoice sp "Microsoft Anna")

;; Display a greeting depending on the hour
(if (< (.Hour (DateTime.Now)) 12) (= greet "Good Morning") 
	  (< (.Hour (DateTime.Now)) 18) (= greet "Good Afternoon") 
	  (= greet  "Good Evening"))

(.set_currentUIculture (Threading.Thread.CurrentThread) (new "System.Globalization.CultureInfo" "en-GB"))

(def voicemenu (greet)
  (.Speak sp greet)
  (= menu "stand:open:close:exit")
  (= c (.split menu (.tochararray ":")))
  
  (prn "Options:") (each i c (prn "> " i))

  (if (not (bound 'rec)) 
      (do 
         (= rec (new "SpeechRecognitionEngine"))
         (= g (new "Grammar" (new "GrammarBuilder" (new "Choices" c))))
         (.loadgrammar rec g)
         (.SetInputToDefaultAudioDevice rec)
       )
   )

  (with (loop true)
    (while loop 
      (do
        (.Speak sp "Ready!") (pr "Ready> ")
        (= t (.Recognize rec))
        (if  (is t nil)             (do  (prn "timeout") (= loop false))
             (is (.text t) "exit")  (do  (prn (.text t)) (.speak sp "Goodbye") (= loop false))
             (is (.text t) "stand") (do  (prn (.text t)) (standup))
             (is (.text t) "open")  (do  (prn (.text t)) (opengripper  5))
             (is (.text t) "close") (do  (prn (.text t)) (closegripper 8))
             (prn (.text t))
        )
      )
    )
  )
)

(prn "(voicemenu greet) to start")


