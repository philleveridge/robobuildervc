
(def mapcar (x y z) (if (or y) (cons (x (car y) (car z)) (mapcar x (cdr y) (cdr z)))))

(def setl (x y q z) 
      (if (or (is x 0) (> x 0)) 
          (cons (if (is x y) z (is x q) (- 0 z) 0) (setl (- x 1) y q z))
 ))
 
  
;> (reverse (setl 10 2 7 4))
;(0 0 4 0 0 0 0 -4 0 0 0)






(= Z2    '(( ( 14  20) (0 0  0 0 0 0 0  0 0 0  4 0 0  -4 0 0 ))             
           ( ( 10  15) (0 0  0 0 0 0 0  0 0 0  2 0 0  -2 0 0 ))
           ( ( 5   11) (0 0  0 0 0 0 0  0 0 0  1 0 0  -1 0 0 ))
           ( (-11 -5)  (0 0  0 0 0 0 0  0 0 0 -1 0 0  1 0 0 ))
           ( (-14 -10) (0 0  0 0 0 0 0  0 0 0 -2 0 0  2 0 0 ))
           ( (-20 -13) (0 0  0 0 0 0 0  0 0 0 -4 0 0  4 0 0 )))
)
 
(= Z4    '(( ( 14  20)  (0 0  0  3 0  0 0  0 0 -3 ))             
           ( ( 10  15)  (0 0  0  2 0  0 0  0 0 -2 ))
           ( ( 5   11)  (0 0  0  1 0  0 0  0 0 -1 ))
           ( (-11 -5)   (0 0  0 -1 0  0 0  0 0  1 ))
           ( (-14 -10)  (0 0  0 -2 0  0 0  0 0  2 ))
           ( (-20 -13)  (0 0  0 -3 0  0 0  0 0  3 )))
)
 
 
 
(def rmatch (n l) 
 (if (or l) 
   (do 
     (= min (car (caar l))) 
     (= max (cadr (caar l)))
     (= r   (cadr (car l) ) )
     ;(prn "test " min " < " n " > " max)
     (if (and (< min n) (> max n)) r (rmatch n (cdr l)))
    )
  )
)

;> (zmatch -4 Ztest)
;(0 0 -2 0 0 0 0 2 0 0 0 0 0 0 0 0 0 0 0)



(def calibrateXYZ () 
 (= xyz (readAcc))
 (= x (car xyz)) (= y (cadr xyz)) (= z (car (cddr xyz)))
   (= gx x)
   (= gy y)
   (= gz z)
   (prn "Calibrated: " gx " " gy " " gz)
)

(def swin ()
  (= p1 (new "Pen" (Color.FromName "Black")))
  (.set_DashStyle p1 (System.Drawing.Drawing2D.DashStyle.DashDot))
  
  (= p2 (new "Pen" (Color.FromName "Red")))

    (createwindow "Balance Demo" 250 250)
    (.show form1)
)

(def pwin (coord n)
   (with (x (- (mod (* n 10) 280) 140) y (* 4 coord) h nil)
     (scope x y) 
     (drawlist g '((-125  40) (125  40)) (if (> y  40) p2 p1)) ; limit
     (drawlist g '((-125 -40) (125 -40)) (if (< y -40) p2 p1)) ; limit     
     (= history (cons (list x y ) history))     
     (= h (list (car history) (cadr history) (car (cdr (cdr history)))))
     (drawlist g h "Blue")
   )
)

(def getZ ()
   "DCMP - quicker if we just want Z"
   (.wckReadPos wck 30 1) ; get y & Z
   (with (z (nth (.respnse wck) 1))
   (= z (if (> z 127) (- z 256) z))
   )
)

(def getXYZ () 
 ;(dcmodeOff)
 (= xyz (readAcc))
 (= x (car xyz)) (= y (cadr xyz)) (= z (car (cddr xyz)))
 ;(prn x "," y "," z)
 (list (- x gx) (- y gy) (- z gz))
)

(def vmatch ( a b) 
    (with (min (norm (diff (caar a) b)) res (cadr (car a)))
    (each item (cdr a) 
       (if (> min (= t (norm (diff (car item) b))))  
          (do (= min t) (= res (cadr item) ))
       )
   ) 
  ;(prn min "," res)
  res))
  
  
(def mvpair (x a b)

  (dcmodeOn)
  (sleep 0.05)

  (with (s1 (getServoPos a) s2 (getServoPos b))
 
    (prn "x=" x ", s" a "=" s1 ", s" b "=" s2) 
    
    (prl (reverse (setl 18 a b x)))
    
    (setServoPos a (coerce (+ s1 x) "byte") 4)
    (setServoPos b (coerce (- s2 x) "byte") 4)
  )
)



(def arm (x)
  (mvpair x 10 13)
)

(def ank (x)
  (mvpair x 3 8)
)

(def knb (x)
  (mvpair x 10 13)
)

(def shl (x)
  (mvpair x 11 14)
)

(def elb (x)
  (mvpair x 12 15)
)

(def playstr (s)  
  (each c (pair s) 
    (with (n (- (coerce (cadr c) "Int32") 48) l (car c))
      (if 
        (is l #\a) (arm n) 
        (is l #\n) (ank n)
        (is l #\k) (knb n) 
        (is l #\s) (shl n)
        (is l #\e) (elb n)    
                
        (is l #\A) (arm (- 0 n))
        (is l #\N) (ank (- 0 n))            
        (is l #\K) (knb (- 0 n))
        (is l #\S) (shl (- 0 n))
        (is l #\E) (elb (- 0 n))
       )
     )
   )
)
;(playstr "a2s2")

(def create ()
  (with (c "")
    (for i 0 (.next r 4)
       (= c (+ c (Char.ToString (nth "ankseANKSE" (.next r  10))) (Char.ToString (coerce (+ 48 (.next r 10)) "char"))))
     )
   c
))
; ( =  s1 "a1k2S1")
; ( =  s2 "A1S2N1")
; (breed s1 s2)




(def checkInput () 
  "test for key input"
  (= res "")
  (if (Console.keyavailable) 
    (do 
      (= ky (.key (Console.ReadKey true)))
      (if (is ky (ConsoleKey.Q))           (err "Quit Pressed"))
      (if (is ky (ConsoleKey.LeftArrow))   (do (prn "<-") (= res "n1")))
      (if (is ky (ConsoleKey.RightArrow))  (do (prn "->") (= res "N1")))
     )
   )
   res 
)

(def bd()
  (try (balance_demo) (.hide form1) (prn "done"))
)


(def balance_demo ()
  "Balance demo"
  (swin)
  (with (r 0.2 n 1)

    (while (< n 1000) 
       (dcmodeOff)
       
       (pwin (nth (readAcc) 2) n)

       (= n (+ 1 n))
       (= i1 (checkInput))
       (if (not (is i1 "")) (playstr i1) (sleep r)  )
   ) 
   (.hide form1)
  )
)

;(= cur (getallServos 17))
;(prn "(bd) ;to start")


;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

(def bt3 ()
"dcm plus mode - high speed"
(if (not (bound 'DCMODEPLUS)) (err "load DCMP.lisp"))
(standup)
(swin)
(= nc 0)

 (= p10 (getServoPos 10))
 (= p13 (getServoPos 13))  
      
(while (not (Console.keyavailable))

   (.wckReadPos wck 30 1) ; get y & Z
   (= z (nth (.respnse wck) 1))
   (= z (if (> z 127) (- z 256) z))
   
   (pwin z (= nc (+ nc 1)))
     
   (= dz (rmatch z Z2))
   (if (or dz) 
    (do

      (= n10 (coerce (+ p10 (nth dz 10)) "Int32"))
      (= n13 (coerce (+ p13 (nth dz 13)) "Int32"))

      (prn "loop: " p13 ", " p10 "," z "," n13 ", " n10)
      ;(prl dz)
      (setServoPos 10 n10 4)
      (setServoPos 13 n13 4)
      
      (= p10 n10)
      (= p13 n13)
     )
    )
 )
 (.close form1)
 (standup)
)




