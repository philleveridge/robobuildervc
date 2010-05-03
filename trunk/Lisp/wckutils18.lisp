;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
; L# software using RobobuilderLib to control
;  Humanoid robot - Robobuilder remotely via serial port
;
;   wck based commands library 
;   requires pcremote
;
;  l3v3rz - Dec 2009
;  1/4/2010 Added PlayMition and PlayMotionFile
;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;


;(load "Lisp\\final.lisp")(run_robobuilder)


(def dcmodeOn ()
  "Enter DC mode (amber light on)"
  (if (not (.isopen sport)) (.open sport))
  (if (bound 'wck) 
    (.setDCmode pcr true)
    (= wck (new "RobobuilderLib.wckMotion" pcr))
  )
  (sleep 0.05)
)

(def wckwriteIO (n c0 c1)
   "turn servo IO on/off"
   (if (not (bound 'wck)) (dcmodeOn))
   (.wckWriteIO wck n c0 c1)
)

(def dcmodeOff ()
  "exit DC mode (amber light off)"
  (.setDCmode pcr false)
  (sleep 0.05)
)

(def getServoPos (n)
  "get position of servo id n"
  (.wckReadPos wck n)
  (car (cdr (.respnse wck)))
)

(def setPassive (n)
  "set servo id n to read mode"
  (.wckPassive wck n)
)

(def setServoPos (id pos torq)
  "set servo position (id: 0-31) (pos: 0-254) (torq: 0-3)"
  (.wckMovePos wck id pos torq)
  (cadr (.respnse wck))
)

(def setSyncMove (lastid torq position)
  "Synchronous mode"
  ;(prn "SetSyncMove " lastid " - " torq "- " )
  ;(prl position) 
  (.SyncPosSend wck lastid torq (toarray position) 0)
)

(def playmotionfile (f)
   (dcmodeOn)
   (.PlayFile wck f)
)

(def toByteArray (pos1)
   "Converts a list into an array of Bytes[]"
   (with (temp (new "Byte[]" (len pos1)))
   (for i 0 (- (len pos1) 1) 
     (do 
         (.SetValue temp (coerce (nth pos1 i) "Byte") i)
         ;(prn i ", " (nth pos1 i))
     )
   )
   temp
   )
)

(def playmotion (spos ff)
  "Play a synchronous motion with inbetweens"
  (with (d (car spos) fr (cadr spos) sp (cddr spos))
    (.PlayPose wck d fr  (toByteArray sp) ff) 
    ;(prn "d=" d "fr=" fr "sp=" sp)
  )
)

(def getallServos(n)
   "Get the current positon of attached servos" 
   (with (current () )
      (for i 0 n 
         (= current (cons (getServoPos (- n i)) current))
      )
   )
)

;;eg (= cur (getallServos 15))

(= data ())

(def capture (id n t) 
  "capture n points every t secs"
  (for i 1 n 
     (do (= data (cons (prn (getServoPos id)) data) )
         (sleep t)
     )
  )
  "Captured"
)

(def play (id n t)
  (for i 0 (- n 1)
     (do (pr ".")
         (setServoPos id (nth data i) 2)
         (sleep t)
     )
  )
  "Complete"
)
   
; (setPassive 12)
; (capture 12 10 0.5)
; (= data (reverse data))
; (play 12 10 0.5)


(def md (xs ys step)
     "make distance / difference list"
     (= intval ())
     (for i 0 (- (len xs) 1)
           (= t  (/ (- (nth xs i)  (nth ys i)) step)) 
           (= intval (cons t intval))
     )
     (reverse intval)
)

;;eg (md '(6 3 2) '(3 1 1) 5.0)


(def prl  (a) (do (prl1 a)  (prn "") a))
(def prl1 (a) (do (pr "(" ) (each x a (if (seq? x) (prl1 x)(Console.Write "{0:0} " x))) (pr ")")))


(def smove (a b n tm)
     "smooth move position a to b in n steps"
     (prn "Smoothmove " n " steps in " tm "s")  
     (pr "from: ")(prl a) 
     (pr "to:   ")(prl b) 

     (= interval (md a b (* n 1.0))) ;; calculate distance

     (= l (len a))
     
     (for i 0 n
           (= nl ())
           (for j 0 (- l 1)
             (= t  (- (nth a j)  (* (nth interval j) i)) )
             (= nl (cons (coerce t "Int32") nl))
           )
           (setSyncMove (- l 1) 2 (reverse  nl))
           (sleep tm)
     ) 
     "done"
)   

;eg (smove '(6 3 2) '(3 1 1) 5 0.1)


(def playsMotion (c x)
  "Play a motion list"
  (= ip (car x))

  ;; move from current pos to ip
  (prn "Do initial move")
  (prl ip) 

  ; move to initial position smoothly ..
  (smove c ip 10 0.5) 

  ;;now loop
  (play1 ip (cdr x))
)

(def play1 (c x)
  "used by play move c -> x"
  (if (or x)
    ( do ;move through list
      (prn "Do next move")
      (= cur (car x))
      (= nof (car cur))
      (= trt (/ (cadr cur) nof))
      (= gt  (car (cdr (cdr cur))))

      (smove c gt nof (/ trt 1000.0) )       ;smooth move from c to gt
    
      ;recurse
      (play1 gt (cdr x))
    )
    ;else weve finished
    (prn "Done") 
  )
) 


(= basic   '( 125 179 199  88 108 126  72  49 163 141  51  47  49 199 205 205 ))
(= initpos '( 125 179 199  88 108 126  72  49 163 141  51  47  49 199 205 205 ))
(= Data0   '( 125 179 199  88 108 126  72  49 163 141  51  47  49 199 205 205 ))	
(= Scene1  '( 107 164 233 106  95 108  80  29 155 129  56  62  40 166 206 208 ))
(= Scene2  '( 107 164 233 106  95 145  74  40 163 154 117 124 114 166 206 208 ))
(= Scene4  '( 126 164 222 100 107 125  80  29 155 142  79  44  40 166 206 208 ))

(= punchleft (list        initpos 
             (list  1  70 Data0) 
             (list  8 310 Scene1) 
             (list  1 420 Scene2) 
             (list  5 200 Scene4) 
             (list 15 300 Data0)))

(def demo ()
  (dcmodeOn) 
  (= cur (getallServos 15))
  (smove cur basic 10 0.1)
  (playsMotion cur punchleft)
)

;(demo)

; -- 18 DOF example ----------------------------------------------------------------

(def cv18 (x) 
  (do 
    (= t (toarray x)) 
    (.SetValue t (+ (.GetValue t 0) 18) 0)
    (.SetValue t (- (.GetValue t 5) 20) 5)
    (+ (tolist t) '(122 125 127))
  )
)

(= basic18 '( 143 179 198  83 106 106  69  48 167 141  47  47  49 199 204 204 122 125 127 ))	

(= punchl18  (list        (cv18 initpos) 
             (list  1  70 (cv18 Data0)  ) 
             (list  8 310 (cv18 Scene1) ) 
             (list  1 420 (cv18 Scene2) ) 
             (list  5 200 (cv18 Scene4) )
             (list 15 300 (cv18 Data0)  )
))

(def standup () (dcmodeOn)(sleep 0.5)  (smove (getallServos 17) basic18 10 0.1) )

(def demo18 ()
  (standup)
  (playmotion basic18 punchl18)
  (standup)
)

; -- 18 DOF example ----------------------------------------------------------------

(def ldiff (t)
 (with (next 0 now 0)
   (dcmodeOn)
   (= now (getallServos 18))
   
   (dcmodeOff)
   (= nacc (readAcc))
      
     (while true 
     (do
       (sleep t)
       (dcmodeOn)
       (= next (getallServos 18))
       (dcmodeOff)
       (= nxa (readAcc))

       (exit?)
       (prn "x" (md now next 1.0) "+" (md nacc nxa 1.0))
       (= now next)
     )
   )
  )
)
;eg (ldiff 0.1)

(def headfwd () (setServoPos 20 145 4))
(def headlft () (setServoPos 20 85  4))
(def headrgt () (setServoPos 20 205 4))


