;;init.lisp
(reference "RobobuilderLib" 
           "System.Windows.Forms")

(using     "RobobuilderLib"
           "System.Windows.Forms"
           "System.IO.Ports")
           
(def readSn ()
  "Read serial number"
  (if (not (.Isopen sport)) 
        (.open sport))
  (= sn (.readSN  pcr))
  sn
) 

(def readAcc ()
  "read acceleromter values"
  (if (not (.isopen sport)) 
      (.open sport))  
  (.readXYZ pcr)
)

(def readDistance ()
  "Read distance"
  (if (not (.Isopen sport)) 
      (.open sport))
  (= d (.readDistance  pcr))
  d
)

(def checkVer () 
  "Check version of robobuilder firmware"
  (if (not (.Isopen sport)) 
      (.open sport))
  (= v (.readVer pcr) )

  (if (< v 2.26) 
     (prn "Download new firmware")                       
     (prn "Latest Firmware loaded (" v ")" ) 
  )
  v
)

(def serial? ()  "check if serial port connected" (if (and (bound 'sport) (.isopen sport)) "Green" "Red"))
(def remote? ()  "check if robot connected"       (if (and (bound 'pcr) (is "Green" (serial?))) "Green" "Red"))
(def number? (x) "check if x number" (and x (or (isa x (typeof "Int32")) (isa x (typeof "Double")))))
(def byte?   (x) "check if x byte"   (and x (number? x) (< 0 x) (< x 255)))

(def play (x) "Play motion file" (.play form x))

(def show-doc (x)
     "Shows doc for an environment entry"
     (pr (.key x) " ")
     (help (.value x)))
     
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

(def getallServos(n)
   "Get the current positon of attached servos" 
   (dcmodeOn)
   (with (current () )
      (for i 0 n 
         (= current (cons (getServoPos (- n i)) current))
      )
   )
)

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


(= basic18 '( 143 179 198  83 106 106  69  48 167 141  47  47  49 199 204 204 122 125 127 ))	

(def stand ()     "Stand" (smove (getallServos 17) basic18 10 0.1)) ; basic pose

(def readIR ()    "Read IR - simulate" (.readIR form)) ;eg (is (readIR) (RobobuilderLib.PCremote+RemoCon.A))

(def readVideo () "Read Video " (.readVideo form))

(def alert (x)    "Display alert box!" (MessageBox.Show x))
(def message (x)   "Display message"    (.Message form x))

(mac repeat (n & body) `(for x 1 ,n ,@body))


