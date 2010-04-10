;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
;
;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;


(load "Lisp\\final.lisp")
(load "Lisp\\wckutils18.lisp")
(load "Lisp\\utilities.lisp")


(def readAcc () 
    (if (not (bound 'r)) (= r (new "Random")))
    (if (bound 'pcr) 
         (.readXYZ pcr) 
         (do (prn "sim data ")(list (- (.next r 50) 25)  (- (.next r 50) 25) (- (.next r 50) 25) ))
     )
)

(= pose2 '(143 185 150 42 105 107 61 51 167 141 47 47 49 199 204 203 122 124 127))

(= boo '(  ((0 0  5)  (0 0  4 0 0 0 0 -4 0 0 0 0 0 0 0 0 0 0 0))             
           ((0 0  2)  (0 0  2 0 0 0 0 -2 0 0 0 0 0 0 0 0 0 0 0))
           ((0 0 -5)  (0 0 -2 0 0 0 0  2 0 0 0 0 0 0 0 0 0 0 0))
 ))
 
(def add   (x y) (if (and x y) (cons (+ (car x) (car y)) (add  (cdr x) (cdr y)))))
(def diff  (x y) (if (and x y) (cons (- (car x) (car y)) (diff (cdr x) (cdr y)))))


;> (add (car (cdr (car foo))) pose2)
;(143 185 152 42 105 107 61 49 167 141 47 47 49 199 204 203 122 124 127)


(def vmatch ( a b) 
    (with (min (norm (diff (caar a) b)) res (cadr (car a)))
    (each item (cdr a) 
       (if (> min (= t (norm (diff (car item) b))))  
          (do (= min t) (= res (cadr item) ))
       )
   ) 
  ;(prn min "," res)
  res))
    
;(vmatch boo '(0 0 2))

;(add pose2 (vmatch boo '(2 3 4)))


(def byte?   (x) (and x (number? x) (< 0 x) (< x 255)))

(def getch () (.keychar (Console.ReadKey true)))

(def pause () 
  (with (ch "")
    (prn "Paused - press 'p' to continue")
    ( while (not (member? (= ch (getch)) "pq")) (pr "."))
    (if (is #\q ch) (err "Quit"))
  )
)

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

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;


(= dx 5)
(= dy 5)
(= dz 5)

(= fwd 4)

(= foo '((0 0 1) (0 0 2 0 0 0 0 -2 0 0 0 0 0 0 0 0 0 0 0)))


(def getXYZ () 
 (dcmodeOff)
 (= xyz (.readXYZ pcr))
 (= x (car xyz)) (= y (cadr xyz)) (= z (car (cddr xyz)))
)

(def calibrateXYZ () 
   (getXYZ)
   (= gx x)
   (= gy y)
   (= gz z)
   (prn "Calibrated" gx " " gy " " gz)
)

(def testzf (a) (> (- gz a) dz))

(def testzb (a) (> (- a gz) dz))


(def leanfwd (delta )
 (prn "lean " delta)
 (dcmodeOn)
 (= p2 (getServoPos 2))
 (= p7 (getServoPos 7))
 (setServoPos 2 (coerce (- p2 delta) "Int32") 2)
 (setServoPos 7 (coerce (+ p7 delta) "Int32") 2)
)

; basic pose
(def basicpose () 
  (dcmodeOn)
  (smove (getallServos 18) basic18 10 0.1)
)

(= bmenu '( 1 "Connect to Robobuilder" 
            2 "Display Accel" 
            3 "CalibrateXYZ" 
            4 "basicPose" 
            0 "Exit"))
      
(= fmenu '(plotaccel calibarateXYZ basicpose baltest))

(def balance ()
  "Balance"
 (while (> (= t (ask "Select" "No such option" bmenu) ) 0)
   (if (is t 1) ( do (run_robobuilder "com3") (basicpose))
       (is t 2) ( plotaccel 5)
       (is t 3) ( calibrateXYZ)
       (is t 4) ( basicpose )
       (is t 5) ( baltest )
   )
   (prn "Serial " (serial?) "Remote " (remote?))
 )
)

 


 


 ;(calibrateXYZ)
 ;(pause)

 ;(for i 0 30
 ; (do 
 ;  (getXYZ)
 ;  (prn i " loop z=" z)
 ;  (if  
 ;     (testzb z)  (leanfwd fwd)
 ;     (testzf z)  (leanfwd (- 0 fwd))
 ;  )
 ;  (sleep 0.4)
 ;  )
 ;)


