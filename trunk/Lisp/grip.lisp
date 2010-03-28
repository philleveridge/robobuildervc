;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
;  L# software using RobobuilderLib to control
;  Humanoid robot - Robobuilder remotely via serial port
;  dynamic control of gripper
;
;  l3v3rz - Jan 2010
;
;  http://en.wikiquote.org/wiki/Lisp_programming_language
;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
;(load "Lisp\\final.lisp") (run_robobuilder)
;(load "Lisp\\wckutils18.lisp")(standup)

(def serial? ()  "check if serial port connected" (if (and (bound 'sport) (.isopen sport)) "Green" "Red"))
(def remote? ()  "check if robot connected"       (if (and (bound 'pcr) (is "Green" (serial?))) "Green" "Red"))
(def number? (x) "check if x number" (and x (or (isa x (typeof "Int32")) (isa x (typeof "Double")))))
(def byte?   (x) "check if x byte"   (and x (number? x) (< 0 x) (< x 255)))

(def dcmodeOn ()
  "Enter DC mode (amber light on)"
  (if (not (.isopen sport)) (.open sport))
  (if (bound 'wck) 
    (.setDCmode pcr true)
    (= wck (new "RobobuilderLib.wckMotion" pcr))
  )
  (sleep 0.05)
)

(def dcmodeOff ()
  "exit DC mode (amber light off)"
  (.setDCmode pcr false)
  (sleep 0.05)
)

(def alert (x)    "Display alert box!" (MessageBox.Show x))
(def message (x)  "Display message"    (.Message form x))

(mac repeat (n & body) `(for x 1 ,n ,@body))

(mac For (x start finish step & body)  `(with (,x ,start) (while 
  (or (and (> ,step 0) (< ,x ,finish )) (and (< ,step 0) (< ,finish ,x))) 
   (do ,@body (= ,x (+ ,x ,step))))))
   
   
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;   
;(= spos 80)
;(def getServoPos (x) spos)
;(def setServoPos (id pos torq)  (= spos pos))

(= widepos  70)
(= closepos 120)
(= maxit    15)
(= dt       0.1)

(def opengripper (d)
   (with (cnt 0 delta 0 cp (coerce (getServoPos 18) "Int32") np 0 delta 5)  
      (if (< cp widepos) (= cp widepos))  
      (while (and (< cnt maxit) (> (Math.abs (- cp widepos)) 2) (> delta 0))
      (do
        (= cnt (+ cnt 1))
        (prn cnt ", " cp ", " delta)
        (setServoPos 18 (- cp d) 4)
        (sleep dt)
        (= np (coerce (getServoPos 18) "Int32"))
        (= delta (Math.abs (- cp np)))
        (= cp np)
      ))
   )
)

(def closegripper (d)
   (with (cnt 0 delta 0 cp (coerce (getServoPos 18) "Int32") np 0 delta 5 )  
      (if (> cp closepos) (= cp closepos))  
      (while (and (< cnt maxit) (> (Math.abs (- cp closepos)) 2) (> delta 2)) 
      (do
        (= cnt (+ cnt 1))
        (prn cnt ", " cp ", " delta)
        (setServoPos 18 (+ d cp) 4)
        (sleep dt)
        (= np (coerce (getServoPos 18) "Int32"))
        (= delta (Math.abs (- cp np)))
        (= cp np)
      ))
   )
)

;(setServoPos 18 70 4) (closegripper 5) (opengripper 5)