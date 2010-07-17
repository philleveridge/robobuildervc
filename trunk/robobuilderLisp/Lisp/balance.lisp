
(def mapcar  (x y z)   (if (or y) (cons (x (car y) (car z))         (mapcar  x (cdr y) (cdr z)        ))) )
(def mapcar2 (x a b c) (if (or a) (cons (x (car a) (car b) (car c)) (mapcar2 x (cdr a) (cdr b) (cdr c)))) )


;            0    1  2   3   4   5   6   7   8   9   10  11  12  13  14  15
(= ub_Huno '(174 228 254 130 185 254 180 126 208 208 254 224 198 254 200 254))
(= lb_Huno '(  1  70 124  40  41  73  22   1 120  57   1  46   1   1  25  40))


(def dptest ()
   (setServoPos 13 200 2) (setServoPos 10 50 2)   
   (while (not (console.keyavailable))
       (do (.wckReadPos wck 30 5)
         (= pos (nth (.respnse wck) 0))
          (prn (.PadLeft "*" pos #\-)) 
         (setServoPos 13 (coerce (- 250 pos) "Byte") 2)
         (setServoPos 10 (coerce (+ 0 pos) "Byte") 2)

      )

   )
) 



(def bcheck ( a b c) (if (> b a) b (< c a) c a))



;( (min1 max1) (list 1 of servo postions increments)
;  (min2 max2) (list 2 of servo postions increments) 
;  etc .... )

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

(= Zx    '(( ( 6  15)  (0 0  0  0  0 0  0 0 0 0 0 -2 0 0 -2 0 ))
           ( ( 3   7)  (0 0  0  0  0 0  0 0 0 0 0 -1 0 0 -1 0 ))
           ( (-7  -3)  (0 0  0  0  0 0  0 0 0 0 0  1 0 0  1 0 ))
           ( (-15 -6)  (0 0  0  0  0 0  0 0 0 0 0  2 0 0  2 0 ))       
))

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

(def getZX ()
   "DCMP - quicker if we just want Z"
   (.wckReadPos wck 30 2) ; get y & Z
   (with (x (nth (.respnse wck) 0) z (nth (.respnse wck) 1))
   (= z (if (> z 127) (- z 256) z))
   (= x (if (> x 127) (- x 256) x))
   ;(prn "x=" (- x gx) ", z=" (- z gz))
   (list (- x gx) (- z gz))
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

   (if (is (mod (= nc (+ nc 1)) 10) 0)        ; time each 10 iterations
   (do
     (= ft (- (.ticks (DateTime.Now)) st))
     (= st (.ticks (DateTime.Now)))
     (= rt (/ ft (* 10 (TimeSpan.tickspermillisecond))))
     (if (not DISPLAY) (prn (String.Format "Rate = {0:#.#}" rt)))
   ))

   (= c1 (- (getZ) gz))           ; get change in Z 
   
   (if DISPLAY (pwin c1 nc rt))   ; display value and rate in window
     
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



;
;
;
(def spos (p)
  (= p (mapcar2 bcheck p lb_Huno ub_Huno))
  (try (.SyncPosSend wck 15 4 (toarray p) 0) 
      (do (pr "overflow") (prl p))          ;catch
      ()                                    ;finally
  )
  p
)

;
; balance function
; 
(def btfn (base xp zp)
  (with 
   ( dz  (rmatch zp Z2)       ; check if range match and return servo position update array 
     dx  (rmatch xp Zx)
     nxt () 
   )                    
     
   (if (or dz)  (= nxt dz))  
   (if (or dx)  (= nxt (mapcar + dx nxt )))
   
   (if (or nxt)
    (do
      ;(pr "d(zx)=" )(prl nxt)
      (= nxt   (mapcar + base nxt))       ; add array to base    
     )
     base
   )
  )
)

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
; bt5 - (balance test mk 5!)
;   Now looks at both X & Z parameters
;   new bound checking routines
;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

(def bt5 ()
"dcm plus mode - high speed"
  (if (not (bound 'DCMODEPLUS)) (err "load DCMP.lisp"))
  (standup)
  (calibrateXYZ)
  (= nc 0)
  (= base (getallServos 15))
  (pause)

  (= st (.ticks (DateTime.Now)))
  (= rt 0)
  
  (try    
  (while (not (Console.keyavailable))

   (if (is (mod (= nc (+ nc 1)) 10) 0)             ; time each 10 iterations
   (do
     (= rt (/ (- (.ticks (DateTime.Now)) st) (* 10 (TimeSpan.tickspermillisecond))))
     (if (not DISPLAY) (prn (String.Format "Rate = {0:#.#}" rt)))
     (= st (.ticks (DateTime.Now)))
   ))

   (= c1 (getZX) )                    ; get change (X z)      
   (= base (spos (btfn base (car c1) (cadr c1))))
  )
  (prn "Exception caught")     
  null
 )
 (standup)
)
