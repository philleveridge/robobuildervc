;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
; L# software using RobobuilderLib to control
;  Humanoid robot - Robobuilder remotely via serial port
;
;  utilities library 
;
;  l3v3rz - Dec 2009
;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;



(reference "System.Windows.Forms"
           "System.Drawing")

(using     "System.Drawing"
           "System.Windows.Forms")

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
; Graphic utilities
;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;

(def createwindow (title x y)
 (do 
   (if (bound 'form1) (.close form1))
   (= form1 (new "Form"))
   (.set_text form1 title)   
   (.set_autosize form1 true)
   (.set_topmost form1 false)
   ( = pb (new "System.Windows.Forms.Panel"))
   (.set_size     pb (new "Size" x y))
   (.set_Location pb (new "Point" 24 16))
   (.add (.controls form1) pb)

   (= g (.CreateGraphics pb))
 )
)

(def plot ( txt x y )
  (with (h 0 w 0 h2 0 w2 0 )
  (= w (.width pb))
  (= h (.height pb))
  (= h2 (/ h 2))
  (= w2 (/ w 2))
  (= x (+ x w2))
  (= y (- h2 y))
  
  (.show form1)
  
  (.clear g (Color.FromName "White"))
  (= axis (new "Pen" (Color.FromName "Black")))
  (= pen  (new "Pen" (Color.FromName "Red")))
  (= font (new "Font" "Arial" (coerce 8.25 "Single" )))
  (.drawline    g axis 0 h2 w h2)
  (.drawline    g axis w2 0 w2 h)
  (.drawellipse g pen (- x 6)  (- y 6) 14 14)
  (.drawstring  g txt font (.Brush pen) (new "PointF" 10 10))
  )
)

(= history ())

(def scope (x y)
   (= x (coerce x "Int32"))
   (= y (coerce y "Int32"))  
   (= text (+ "(" (str x) " " (str y) ")" ))
   (plot text x y)
)

(= Pi 3.1415)

(def anim (f t s)
  (createwindow "Demo" 250 250)
  (.show form1)
  (while true
  (do
  (= history ()) 
  (= n -125) 
  (while (< n 125) 
     (= a (* 100 (Math.Sin (/ (* n Pi) f))))
     (scope n a) 
     
     (= history (cons (list n a) history))
     (= h (list (car history) (cadr history) (car (cdr (cdr history)))))
     (drawlist g h "Blue")
     
     (sleep t) 
     (= n (+ s n)) 
     (exit?)
  )
  ))
  (.hide form1)
)

; (anim 50 0.5 10)
; (.show form1)(drawlist g history "Blue")

(def alert (x)    "Display alert box!" (MessageBox.Show x))
(def message (x)  "Display message"    (.Message form x))

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
; Real time graphic demo - using acceleromter
;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
(= r (new "Random"))
;(def readAcc () (list (- (.next r 50) 25)  (- (.next r 50) 25) (- (.next r 50) 25) ))

;(def readAcc () (.readXYZ pcr))


(def plotaccel (r)
  "Plot - sample rate rHz"
  (= r (/ 1.0 r))
  (= p1 (new "Pen" (Color.FromName "Black")))
  (.set_DashStyle p1 (System.Drawing.Drawing2D.DashStyle.DashDot))
  
  (with (acc 0)
    (createwindow "Accelerometer Demo" 250 250)
    (.show form1) 
    (= n 1) 
    (while (< n 1000) 
       (= acc (readAcc))
       (scope (car acc) (cadr acc)) 
       (status 205 8  "Serial" (serial?))
       (status 160 8  "Remote" (remote?))
       (drawlist g '((-125  40) (125   40)) p1) ; limit
       (drawlist g '((-125 -40) (125 -40))  p1) ; limit
       
       (= history (cons (list (car acc) (cadr acc)) history))
       (= h (list (car history) (cadr history) (car (cdr (cdr history)))))
       (drawlist g h "Blue")
     
       (sleep r) 
       (= n (+ 1 n))
       (exit?)
   ) 
   (.hide form1)
  )
)

;(plotaccel 5) ; 5Hz


(def status(x y txt c)
  (= pen  (new "Pen" (Color.FromName c)))
  (.fillellipse g (.Brush pen) (- x 10)  (- y 6) 14 14)
  (= pen  (new "Pen" (Color.FromName "Black")))
  (.drawellipse g pen (- x 10)  (- y 6) 14 14)
  (= font (new "Font" "Arial" (coerce 8.25 "Single" )))
  (.drawstring  g txt font (.Brush pen) (new "PointF" (+ x 10) (- y 4)))
)


(def serial? () (if (and (bound 'sport) (.isopen sport)) "Green" "Red"))
(def remote? () (if (and (bound 'pcr) (is "Green" (serial?))) "Green" "Red"))

(def demo2()
 (createwindow "status" 250 250)
 (.show form1)
 (.clear g (Color.FromName "White"))
 (status 205 8  "Serial" (serial?))
 (status 160 8  "wck"   (remote?))
)


;; (= testset '( (10 5 -4) (10 4 -2) (11 7 6) (20 7 6) (10 5 -4)))
(= dataset ())

(def readSensorData() 
  "Read 100 data set points at 5Hz sample rate"
  (= n 1) 
  (while (< n 100)  
    (= acc (readAcc))
    (= dataset (cons acc dataset))
    (sleep 0.2)
    (= n (+ 1 n))
  )  
)

;(drawlist g '((0 0) (20 20) (30 120) (140 20) (50 50)) "Red")
(def drawlist (g l c) 
      (= from (car  l))
      (= to   (cadr l))
      (if (and from to)
        (do     
        (drawline g (car from) (cadr from) (car to) (cadr to) c)
        (drawlist g (cdr l) c)
        )
      )
)

;(drawlistc g '((0 0 "Red") (20 20 "Green") (30 120 "Green") (140 20 "Green") (50 50 "Red")))
(def drawlistc (g l) 
      (= from (car  l))
      (= to   (cadr l))
      
      (if (and from to)
        (do    
              (= c    (car (cddr l))) 
        (drawline g (car from) (cadr from) (car to) (cadr to) (car (cddr to)))
        (drawlistc g (cdr l))
        )
      )
)

(def drawline (g fx fy tx ty c) 
      (if (isa c (typeof "String"))
         (= lp (new "Pen" (Color.FromName c)))  
         (= lp c)
      )        
      (= w (.width pb))
      (= h (.height pb))
      (= h2 (/ h 2))
      (= w2 (/ w 2))
      (= fx (+ (coerce fx "Int32") w2))
      (= fy (- h2 (coerce fy "Int32")))
      (= tx (+ (coerce tx "Int32") w2))
      (= ty (- h2 (coerce ty "Int32")))     
      (.drawline  g lp fx fy tx ty)
)

(def plotData ()
  (with (i 0 fx 0 fy 0) 
    (each x dataset
        (do (= i (+ i 1)) 
               (if (> fx 0) 
                   (drawline g fx fy i (car x))) 
         (= fx i) 
         (= fy (car x))  
         )
     )
  )
)

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
; Read CSV utilities utilities
;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;


(def readcsv (filename)
  (with (f "" z ())
    (= f (System.IO.File.ReadAllLines filename))
    (each line f     
      (if (not (is #\# (car line)) )
         (do (= z (cons (splitline line) z)))
       )
     )
     (reverse z)
  )
)

(def splitline (text)
  (with (l "0" z () r 0) 
     (each a (toarray text ) 
        (if (is a #\,) 
             (do (= z (cons (coerce l "Int32") z)) (= l "0") ) 
             ;else
             (= l (+ l (str a))) 
         )
     )
     (= z (cons (coerce l "Int32") z)) ;; last arg
     (= r (reverse z))
     (cons (cadr r) (cons (car r) (list (cdr (cdr r)))))
  )
)

;; example 
;(= data (readcsv "Lisp\\aim.csv"))
;(each x data (prl x))


;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
; Keyboard input
;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;

(def getch () (.keychar (Console.ReadKey true)))

(def pause () 
  (prn "Paused - press 'p' to continue")
  ( while (not (is (getch) #\p)) (pr "."))
)

(def exit? () 
   (if (Console.keyavailable) 
     (if (is (.key (Console.ReadKey true)) (ConsoleKey.Q)) (err "Quit Pressed"))))

;(prn "loop 100 times or until Q pressed")
;(for i 0 100 (do (prn i ) (exit?) (sleep 0.1)))


; example
; ( while (not (member? (getch) "abcde")) (pr "."))

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
; predicate utilities
;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;

(def number? (x) (and x (or (isa x (typeof "Int32")) (isa x (typeof "Double")))))
(def byte?   (x) (and x (number? x) (< 0 x) (< x 255)))


;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
; List /property handling utilities
;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

(def prl  (a) (do (prl1 a)  (prn "") a))
(def prl1 (a) (do (pr "(" ) (each x a (if (seq? x) (prl1 x)(Console.Write "{0:0} " x ))) (pr ")")))

; (prl '(a b c))
; (prl '(a (b d) c))


(def cadar (x) (car (cdr (car x))))

(def assoc (x y)
  (if   (is y () ) null
        (is (caar y) x) (cadar y)
        (assoc x (cdr y))
  ))

; assoc example
; (= z2 '( (temp 100) (rgb (123 234 30)) (del 50)))
; (assoc 'rgb z2)
; (assoc 'p z2)

 (def del (e l) 
   (if (not l) null (is e (car l)) (del e (cdr l)) (cons (car l) (del e (cdr l)))))
 
 ;eg (del 'H '(H T H T T H))
 
 
(def subst ( x y s )
  (if (is y s)   x 
      (atom? s ) s 
      (cons (subst x y (car s ) ) (subst x y (cdr s ) )) 
  ) 
) 

; example
;(subst 'a 'b '(a b a b c))
;> (a a a a c)


(def toStringArray (pos1)
   "Converts a list into an array of String[]"
   (with (temp (new "String[]" (len pos1)))
   (for i 0 (- (len pos1) 1) 
         (.SetValue temp (nth pos1 i)  i)
   )
   temp
   )
)

; example
;> (tolist (toStringArray '("red" "yellow" "green" "blue")))
;("red" "yellow" "green" "blue")


;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
; Vector utilities
;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
(def add   (x y) 
   "Add two vectors x and y together"
   (if (and x y) (cons (+ (car x) (car y)) (add (cdr x) (cdr y)))))
   
(def diff  (x y) 
   "Subtract two vectors x and y "
      (if (and x y) (cons (- (car x) (car y)) (diff (cdr x) (cdr y)))))

(def dot-product (a b)
  "calculate dot product of two vectors."
  (if (or (empty? a) (empty? b))
      0
      (+ (* (car a) (car b))
      (dot-product (cdr a) (cdr b)))))

(def norm (a) 
  "normalise a vecor i.e. sqrt(a.a)"
  (sqrt (dot-product a a)))

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
; Macros utilities
;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

(mac repeat (n & body) `(for x 1 ,n ,@body))

(mac For (x start finish step & body)  `(with (,x ,start) (while 
  (or (and (> ,step 0) (< ,x ,finish )) (and (< ,step 0) (< ,finish ,x))) 
   (do ,@body (= ,x (+ ,x ,step))))))
   
   


