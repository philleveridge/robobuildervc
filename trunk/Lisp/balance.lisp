
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

(def pwin (coord n rt)
   (with (x (coerce (- (mod (* n 10) 280) 140) "Int32") y (coerce (* 4 coord) "Int32") h nil txt "")
   
     (= text (+ "(Acc=" (str y) " Rate=" (String.Format "{0:#.#}" rt) " ms)" ))
     (plot text x y)

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
 (= xyz (readAcc))
 (= x (car xyz)) (= y (cadr xyz)) (= z (car (cddr xyz)))
 ;(prn x "," y "," z)
 (list (- x gx) (- y gy) (- z gz))
)

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


(def bt4 ()
"dcm plus mode - high speed"
  (if (not (bound 'DCMODEPLUS)) (err "load DCMP.lisp"))
  (standup)
  (calibrateXYZ)
  (= nc 0)
  (= base (getallServos 15))
  (pause)

  (swin)
  (= st (.ticks (DateTime.Now)))
  (= rt 0)
  
  (try    
  (while (not (Console.keyavailable))

   (if (is (mod nc 10) 0) 
   (do
     (= ft (- (.ticks (DateTime.Now)) st))
     (= st (.ticks (DateTime.Now)))
     (= rt (/ ft (* 10 (TimeSpan.tickspermillisecond))))
   ))

   (= c1 (getZ))
   
   (pwin c1 (= nc (+ nc 1)) rt)
     
   (= dz (rmatch c1 Z2))
   (if (or dz) 
    (do
      (= nxt (mapcar + base dz))
 
      (try (.SyncPosSend wck 15 4 (toarray nxt) 0) (do (pr "overflow") (prl nxt)) null)

      (= base nxt)
     )
     ;(prn "loop " c1)
    )
 )
 (prn "Exception caught")
 null
 )
 (.close form1)
 (standup)
)





