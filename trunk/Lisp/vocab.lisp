;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
; L# software using RobobuilderLib to control
;  Humanoid robot - Robobuilder remotely via serial port
;
;
;  l3v3rz - March 2010
;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;


(reference "C:\\Program Files\\Reference Assemblies\\Microsoft\\Framework\\v3.0\\System.Speech.dll")
(using "System.Speech.Recognition")
(using "System.Speech.Synthesis")

(= sp (new "SpeechSynthesizer"))

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
;; Display a greeting depending on the hour
;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

(if (< (.Hour (DateTime.Now)) 12) (= greet "Good Morning") 
	(< (.Hour (DateTime.Now)) 18) (= greet  "Good Afternoon") 
	(= greet  "Good Evening"))
	

(.Speak sp greet)


;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
;matching
;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;

(def evalstring (s)  (eval (LSharp.Runtime.ReadFromString s)))

(def atomcar (z) (str (car (str z))))
(def atomcdr (z) (str (cdr (str z))))

(def setstr  (x y) (LSharp.Runtime.VarSet (LSharp.Symbol.FromName x) y environment))

(def evalstr (x) (eval (LSharp.Symbol.FromName x)))


(def match ( p d)
  ;(prn "debug: " p " & " d) 
  (if (and (empty? p) (empty? d)) t
      (or  (empty? p) (empty? d)) nil
      (or  (is (car p) '>)  (is (car p) (car d))) 
         (match (cdr p) (cdr d))
      (and (is (atomcar (car p)) ">") (match (cdr p) (cdr d)))
         (setstr (atomcdr (car p)) (car d))
      (is  (car p) '+)
        (if (match (cdr p) (cdr d)) true (match p (cdr d)))
      (is (atomcar (car p)) "+")
         (if (match (cdr p) (cdr d)) (setstr (atomcdr (car p)) (list (car d))) 
            (match p (cdr d))        (setstr (atomcdr (car p)) (cons (car d) (evalstr (atomcdr (car p))))
         )
     )
  )
)

(def say (t) 
(= txt "")
(each x t 
   (if (atom? x) (= txt (str txt (eval x))) (= txt (str txt x)) )
)
(prn txt)
(.Speak sp txt)
)

(= syntax '(
(open +X hand)   ("Opening " X "hand")
(close +X hand)  ("Closing " X "hand")
))

(def tmatch (x) 
  (= mf false)
  (each p (pair syntax) 
    (do 
    ;(prn "s=" (car p) "  r=" (cdr p))
    (if (match (car p) inpt) (do (= mf true)(say (cadr p))))
    )
  )
  mf
)

;(tmatch "(I AM WORRIED ABOUT FRED)")

(def initrecog (x)
      (do 
         (= rec (new "SpeechRecognitionEngine"))
         (= g (new "Grammar" x))
         (.loadgrammar rec g)
         (.SetInputToDefaultAudioDevice rec)
       )
)


(def vocab ()
  "Demo"
  (if (not (bound 'rec))  (initrecog "Lisp\\vocab.xml"))
  (with (item 1) 
     (say "Hello")
     (while (not (is item 0)) 
         (do 
			(= t (.Recognize rec))
			(prn (.text t)) (= inpt (+ "'(" (.text t) ")"))
	 
            ;(while (is (= inp (Console.ReadLine)) "" ) (pr ": "))) 
            ;(= inpt (evalstring (+ "'" (.ToUpper inp))))
	          (if 
              (iso inpt 'BYE) 
                  (do (say "GOODBYE" )(err "Done"))

              (if (not (tmatch inpt))
                    (say "Pardon?"))
              )
	         (pr ": ") 
         )
     )
  )
)

(initrecog "Lisp\\vocab.xml")
(vocab)

;scratch
;
; (.RecognizeAsync rec (RecognizeMode.Single))
; (.SpeechRecognized rec  += new EventHandler<SpeechRecognizedEventArgs>(SpeechRecognized);
; private void SpeechRecognized(object sender, RecognitionEventArgs e);
