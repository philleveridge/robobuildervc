
;(def readdistance () 50) ; debug only!
;(def setServoPos (a b c) ())
;(def psdoff () ())


(def scan (f)
  (= p1 (new "Pen" (Color.FromName "Black")))
  (.set_DashStyle p1 (System.Drawing.Drawing2D.DashStyle.DashDot))
  (createwindow "Plot Distance Demo" 250 250)
  (.show form1) 
  
  (= centre 142)
  (= left (+ centre 40))
  (= right (- centre 40))

  (while (not (console.keyavailable))
    (scanw right   left  2 f)
    (scanw left   right -2 f)
  )
  (setServoPos 20 centre 4) 
  (.close form1)
  (psdoff)
)


(def scanw (a b s f)
  (prn "Scanw " a " to " b)
  (with (d () history () n 0 i a)
   (while (or (and (> s 0)(< (= i (+ i s)) b)) (and (< s 0) (> (= i (+ i s)) b)))
   (do 
    (= d (readdistance) )
    (if (> s 0)
       (= n (+ -100 (* (- i a) (/ 200 (- b a)))))      ; n = -100 + (i-a)*(200/(b-a))
       (= n (+ -100 (* (- i b) (/ 200 (- a b)))))      ; n = -100 + (i-b)*(200/(a-b))
    )
    
    (if f 
      (do
        (scope n d)   
        (= history (cons (list n d) history))
        (prn i " " d " " n) 
        (drawlist g history "Blue")  
       )
      (do  
        (= q (conv d i a b))
        (scope (car q) (cadr q))  
        (if (> d 49)      
          (= history (cons (+ q (list "White")) history))
          (= history (cons (+ q (list "Red"))   history))
        )
        (drawlistc g history "Blue")  
       ))

 
    (setServoPos 20 i 4)
    (sleep 0.1)
   )
   )
 )
)

(def conv (d p ml mr)
  (with 
     (ppd (/ 270.0 255)
      cn (+ ml (/ (- mr ml) 2.0))
      a 0 x 0 y 0 r 0)

    (= a (- p cn) )
    (= a (* a ppd))
    (= r (/ (* a (Math.PI)) 90)) ; 2r

    (= x (* d (Math.Sin r)))
    (= y (* d (Math.Cos r)))

    (prn "d=" d ", p=" p " -> a=" a " -> (x,y) =" x "," y)
    (list x y)
  )
)

(def scant ()
 (for i 100 200 (conv 50 i 100 200)))

