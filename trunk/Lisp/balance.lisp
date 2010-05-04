
(def mapcar (x y z) (if (or y) (cons (x (car y) (car z)) (mapcar x (cdr y) (cdr z)))))

(def setl (x y q z) 
      (if (or (is x 0) (> x 0)) 
          (cons (if (is x y) z (is x q) (- 0 z) 0) (setl (- x 1) y q z))
 ))
 
  
;> (reverse (setl 10 2 7 4))
;(0 0 4 0 0 0 0 -4 0 0 0)


;( 
; (min1 max1) (list 1 of servo postions increments)
; (min2 max2) (list 2 of servo postions increments) 
;    etc .... 
;)

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
; 
; range search - look for a match for n in list l
 
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

;> (rmatch -8 Z2)
;(0 0  0 0 0 0 0  0 0 0 -1 0 0  1 0 0 )

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

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
; bt4 - (balance test mk 4!)
; DISPLAY if true will show graphical Z accelerometer value
;    .. BUT .. its slows it down by 50ms per cycle
;    so set FALSE for performance
;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

(= DISPLAY false)

(def bt4 ()
"dcm plus mode - high speed"
  (if (not (bound 'DCMODEPLUS)) (err "load DCMP.lisp"))
  (standup)
  (calibrateXYZ)
  (= nc 0)
  (= base (getallServos 15))
  (pause)

  (if DISPLAY (swin))
  (= st (.ticks (DateTime.Now)))
  (= rt 0)
  
  (try    
  (while (not (Console.keyavailable))

   (if (is (mod nc 10) 0)         ; time each 10 iterations
   (do
     (= ft (- (.ticks (DateTime.Now)) st))
     (= st (.ticks (DateTime.Now)))
     (= rt (/ ft (* 10 (TimeSpan.tickspermillisecond))))
     (if (not DISPLAY) (prn "Rate = " rt))
   ))

   (= c1 (- (getZ) gz))           ; get change in Z 
   
   (if DISPLAY (pwin c1 (= nc (+ nc 1)) rt))   ; display value and rate in window
     
   (= dz (rmatch c1 Z2))          ; check if range match and return servo position update array
   (if (or dz)                    ; if null - no change required
    (do
      (= nxt (mapcar + base dz))  ; add array to base
 
      (try (.SyncPosSend wck 15 4 (toarray nxt) 0) (do (pr "overflow") (prl nxt)) null)

      (= base nxt)                ; update base potion
     )
    )
  )
  (prn "Exception caught")     
  null
 )
 (if DISPLAY (.close form1))
 (standup)
)





