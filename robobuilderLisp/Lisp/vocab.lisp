;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
; L# software using RobobuilderLib to control
;  Humanoid robot - Robobuilder remotely via serial port
;
;
;  l3v3rz - March 2010
;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;


(= verb '(wave open close extend retract swing in out learn remember showme exit))
(= noun '(left right arm foot))

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
(I AM WORRIED ABOUT +L)  ("WHY ARE YOU WORRIED ABOUT " L "?")
(+ MOTHER +)             ("TELL ME MORE ABOUT YOUR FAMILY")  
) 
)

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



(def vocab ()
  "Demo"
  (with (item 1) 
     (while (not (is item 0)) 
         (do 
            (while (is (= inp (Console.ReadLine)) "" ) (pr ": "))) 
            (= inpt (evalstring (+ "'" (.ToUpper inp))))
	          (if 
              (iso inpt 'BYE) 
                  (do (say "GOODBYE" )(err "Done"))

              (if (not (tmatch inpt))
                    (say "TELL ME MORE")
              )
            )
	         (pr ": ") 
         )
     )
)
