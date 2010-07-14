;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
;  L# software using RobobuilderLib to control
;  Humanoid robot - Robobuilder remotely via serial port
;
;  Knowledge management library
;
;  l3v3rz - Dec 2009
;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;


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
  "match items in p (including wild chars) to list d"
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


;(match '( a b c)  '(a b c))  ;; t
;(match '( a b c)  '(a b d))  ;; f
;(match '( a > c)  '(a b c))  ;; t
;(match '( + b +)  '(a b c))  ;; t
;(match '( + d +)  '(a b c))  ;; f
;(match '(a + b)   '(a x y b))
;(match '( a >L c) '(a b c))  ;; t & L=b
;(match '( a +G c) '(a b d c))  ;; t & G=(b d)

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

(prn "Search and matching")
(prn "type (doctor) to start")
'Ready!


(= questions '"REALY THATS QUITE INTERESTING.
PLEASE CONTINUE.
I AM NOT SURE THAT I UNDERSTAND YOU FULLY.
PLEASE ELABORATE YOUR THOUGHTS.
I SEE.
REALLY? PLEASE CONTINUE.
I UNDERSTAND.
HELLO, WHAT IS YOUR PROBLEM?
HELLO, WHAT CAN I HELP YOU WITH TODAY?
WHERE DO YOU GET THE IDEA THAT I AM*
I DO NOT BELIEVE THAT I AM*
DOES IT PLEASE YOU TO BELIEVE THAT I AM*
DO YOU BELIEVE IT IS NORMAL TO BE*
HOW LONG HAVE YOU BEEN*
WHAT DOES IT FEEL LIKE TO BE*
DO COMPUTERS SCARE YOU?
DO YOU ENJOY USING COMPUTERS?
WHAT BRINGS UP THE SUBJECT OF COMPUTERS?
ARE COMPUTERS THE ROOT OF YOUR PROBLEM?
DO YOU THINK THAT IT IS NORMAL TO DREAM*
WHY DO YOU DREAM*
PLEASE TELL ME MORE ABOUT YOUR DREAMS
DO YOU HAVE THESE DREAMS OFTEN?
WHY DO YOU THINK*
WHAT MAKES YOU THINK THAT I AM*
IS IT NORMAL TO THINK*
WHAT MAKES YOU THINK*
WHAT CAUSES THESE THOUGHTS OF FAST WOMEN?
ARE THEY NAKED?
ARE YOU ATTRACTED TO THEM?
ARE THEY ATTRACTED TO YOU?
PLEASE TELL ME MORE ABOUT THESE THOUGHTS
THAT IS QUITE AN INTERESTING THOUGHT, PLEASE CONTINUE.
DOES THAT BOTHER YOU?
HOW MANY ARE THERE NORMALLY?
HOW BIG ARE THEY
REALLY? WHAT COLOR?
ARE THEY AMERICAN-MADE?
HOW COULD YOU IMPROVE YOUR SITUATION?
HAVE YOU EVER TRIED TO CHANGE YOUR HABITS?
DURING THIS ENTIRE INTERVIEW, YOU HAVEN'T MENTIONED YOUR MOTHER.
I SEE
ARE THEY MOVING?
HAVE YOU EVER RIDDEN IN ONE?
WHAT COLOR
I SEE
WHAT DO YOU PLAN TO DO ABOUT IT?
I UNDERSTAND
HOW DOES THAT CONCERN YOU?
DOES NOISE FRIGHTEN YOU?
DO YOU ENJOY YOUR JOB?
DO YOU LIKE THE 'STONES?
DO YOU FEEL LIKE THEY ARE EVERYWHERE?
WHAT IS IT MOST THAT BOTHERS YOU?
DOES THIS APPLY TO ALL OF THEM?
WHEN DID THIS START?
DO YOU THINK THINGS WILL CHANGE?
PLEASE CONTINUE
I THINK YOU SHOULD HAVE MORE CONFIDENCE
DO YOU USE A TELEPHONE AT WORK?
WHEN DID YOU FIRST NOTICE YOU PROBLEM WITH TELEPHONES?
DOES NOISE FRIGHTEN YOU?
THAT IS INTERESTING
DO YOU PLAY ANY INSTRUMENTS?
DO YOU WISH YOU WERE MUSICALLY INCLINED?
DO YOU LIKE THE BEATLES?
I SEE
WHAT DO YOU THINK IS BEST?
PLEASE CONTINUE
DO THEY REMIND YOU OF BUGS?
WHAT INSTRUMENT WOULD YOU LIKE TO PLAY?
ARE YOU TONE DEAF?
PLEASE BE SPECIFIC
DO YOU LISTEN TO MUSIC IN YOUR SPARE TIME?
DO I BOTHER YOU?
HAVE YOU EVER HEARD OF FIBONACCI?
DOES YOUR MOTHER REMIND YOU OF AN I.C. CHIP?
DO YOU HAVE AN AVERSION TO TOILET SEATS?
WHAT MAKES YOU THINK THAT YOU NEED*
HOW LONG HAVE YOU FELT LIKE THIS?
I SEE
DO YOU HAVE AN AVERSION TO ANY PARTICULAR PSCHOLOGIST?
HMMM
DO YOU THINK YOU CAN BE CURED?
WOULD IT HELP IF I TOLD YOU MY FEE?
WHAT IS IT THAT BOTHERS YOU MOST?
DO YOU THINK FREUD WAS AN IDIOT?
DO YOU EVER FEEL AS IF SOMEONE IS WATCHING YOU?
WHY DO YOU WANT*
SHOULD I ALSO WANT*
WHAT DRIVES YOU TO WANT*
WHAT MAKES YOU THINK I HAVE*
WHY DO YOU NEED*
COULD THIS NEED BE THE CAUSE FOR YOUR PROBLEM?
IS IT NORMAL TO NEED*
DO YOU HAVE ANY OTHER NEEDS?
HOW LONG HAVE YOU HAD*
COULD THESE DESIRES BE CAUSING YOU PROBLEMS?
TELL ME MORE ABOUT YOUR DESIRE FOR*
WHY DO YOU HAVE*
IS IT NORMAL FOR YOU TO HAVE*
WOULD IT BE NORMAL FOR ME TO HAVE*
I AM NOT SURE THAT I HAVE*
WHEN DID I ACQUIRE*
WHAT MAKES YOU THINK THAT I NEED*
WOULD YOU ALSO LIKE TO HAVE*
I AM NOT SURE THAT I NEED*
PLEASE EXPLAIN WHY I NEED*
WHAT PROMPTS YOU TO ASK?
DOES EVERYBODY NEED*
DO YOU HAVE PROBLEMS WITH WOMEN?
IS THE ANSWER NECESSARY FOR THE SOLUTION OF YOUR PROBLEM?
I AM NOT SURE THAT I CAN ANSWER THAT QUESTION FOR YOU.
PLEASE ELABORATE YOUR QUESTION WITH MORE INFORMATION.
I DONT UNDERSTAND YOUR QUESTION FULLY.
WHAT WOULD BE A GOOD ANSWER FOR YOUR QUESTION?
I NEED MORE INFORMATION TO BE ABLE TO ANSWER YOUR QUESTION.
WHY DO YOU HATE*
IS IT NORMAL TO HATE*
I THINK THAT THIS HATRED FOR* MAY BE CAUSING YOU PROBLEMS.
SHOULD I ALSO HATE*
I DO NOT UNDERSTAND WHY YOU HATE*
WHAT DOES THIS SUGGEST TO YOU?
WHY DO YOU MENTION WOMEN?
WHERE DO YOU MEET THESE WOMEN?
DO YOU LIKE WOMEN?")

(= foo '((I	@YOU)
(ME	YOU)
(IM	@YOURE)
(YOURE	IM)
(IVE	@YOUVE)
(YOUVE	IVE)
(YOU	I)))

