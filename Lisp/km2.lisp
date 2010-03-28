;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
;  L# software using RobobuilderLib to control
;  Humanoid robot - Robobuilder remotely via serial port
;
;  Knowledge management library
;
;  l3v3rz - Dec 2009
;
;  http://en.wikiquote.org/wiki/Lisp_programming_language
;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
;Rules 
;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;


; example
; (remember "hello") (remember "world") (recall "hello") (recall "err")


(def remember (new) 
	(if  (member? new facts) 
		nil 
		(= facts (cons new facts) 	new)))


;eg (remember "hello")
;eg (remember "world")

(def recall (fact) (if (member? fact facts) fact nil))
 
(= rules '( 
   (rule identify1) (if (animal has hair)) (then (animal is mammal))
   (rule identify2) (if (animal gives milk)) (then (animal is mammal))
   (rule identify3) (if (animal has feathers)) (then (animal is bird))
   ))

(def testif  (rule) ((with (ifs "") (prn "dumy") )))
(def usethen (rule) (prn "dumy"))
(def tryrule (rule) (and (testif rule) (usethen rule)))



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

;> (setstr (atomcdr (car '(abc def))) 123)
;123
;> bc
;123
;>


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


(match '( a b c)  '(a b c))  ;; t
(match '( a b c)  '(a b d))  ;; f
(match '( a > c)  '(a b c))  ;; t
(match '( + b +)  '(a b c))  ;; t
(match '( + d +)  '(a b c))  ;; f
(match '(a + b)   '(a x y b))
(match '( a >L c) '(a b c))  ;; t & L=b
(match '( a +G c) '(a b d c))  ;; t & G=(b d)

(def doctor ()
  "Demo"
  (with (item 1) 
     (while (not (is item 0)) 
         (do 
            (while (is (= inp (Console.ReadLine)) "" ) (pr ": "))) 
            (= inpt (evalstring (+ "'" (.ToUpper inp))))
	    (if 
              (iso inpt 'BYE) 
                  (do (prn "GOODBYE" )(err "Done"))

              (match '(I AM WORRIED ABOUT +L) inpt) 
                  (prn "WHY ARE YOU WORRIED ABOUT " L "?")

              (match '(+ MOTHER +) inpt) 
                  (prn "TELL ME MORE ABOUT YOUR FAMILY")

              (prn "TELL ME MORE")
            )
	    (pr ": ") 
         )
     )
)
;eg (doctor)

