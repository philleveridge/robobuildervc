;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
;  L# software using RobobuilderLib to control
;  Humanoid robot - Robobuilder remotely via serial port
;
;
;  l3v3rz - Dec 2009
;
;  http://en.wikiquote.org/wiki/Lisp_programming_language
;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

(reference "System.Windows.Forms")

(using     "System.Windows.Forms"
           "System.IO.Ports")

(= rbl (reference  (String.Concat (System.IO.Directory.GetCurrentDirectory) "\\Libs\\RobobuilderLib.dll")))
(using "RobobuilderLib")
(prn "RobobuilderLib version : " (.version (.GetName rbl)))

(def connect (pn)
  "Initialise connection to robobuilder"
  (prn "connecting to " pn)
  
  (if (not (bound 'sport)) 
     (= sport (new "SerialPort"))
  )
  (if (.Isopen sport) 
     (.Close sport)
  )
  (.set_BaudRate sport 115200)
  (.set_PortName sport pn)
  (.set_WriteTimeout sport 500)
  (.set_ReadTimeout  sport 500)
  (= pcr (new "RobobuilderLib.PCremote" sport))
  (.setDCmode pcr false)
)

(def askPort ()
  "Ask user to input available COM port ID)"
  (with (k 0 p 0 y 0)
      (prn "Available Ports:")
      (= p (System.IO.Ports.SerialPort.GetPortNames))
      (each y p (prn y))
      (prn "Select:")
      (while (is (= k (Console.ReadLine)) "" ) (pr ": "))
      (if (not (member? k p)) (do (prn "Invalid port?") (askPort)) k)
  )
) 

(def getsn ()
  "Read serial number"
  (if (not (.Isopen sport)) 
        (.open sport))
  (= sn (.readSN  pcr))
  sn
) 

(def readdistance ()
  "Read distance"
  (if (not (.Isopen sport)) 
      (.open sport))
  (= d (.readDistance  pcr))
  d
)

(def runMotion (n )
  "Play motion id"
  (if (not (.isopen sport)) 
      (.open sport))
  (.runMotion  pcr n)
)

(def runSound (n )
   "Play sound id"
  (if (not (.Isopen sport)) 
      (.open sport))
    (.runSound pcr n)
)

(def readAcc ()
  (if (not (.isopen sport)) 
      (.open sport))  
  (tolist (.readXYZ pcr))
)

(def checkver () 
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

(def readSensors ()
    "Read XYZ 10 times over the next 5 secs"
    (do (prn "i  X  Y  Z  Distance")
       (for i 0 10 
          (do (= xyz (readAcc)) 
          (prn i " " (nth xyz 0) "," (nth xyz 1) "," (nth xyz 2) " : " (readdistance))  
          (sleep 0.5))
        )
    )
)

(= menu '( 1 "GET UP A" 
           2 "GET UP B" 
           3 "TURN LEFT" 
           4 "MOVE FORWARD" 
           5 "TURN RIGHT" 
           6 "MOVE LEFT" 
           7 "BASIC POSTURE" 
           8 "MOVE RIGHT" 
           9 "ATTACK LEFT" 
          10 "MOVE BACWARDS" 
          11 "ATTACK RIGHT" 
          12 "Read Sensors"
           0 "EXIT"))

(def ask (prompt error menu)
  "Request user supply input from menu list"
  (with (k 0 )
      (prn prompt)
      (each x (pair menu) (prn (car x) " - " (cdr x)))
      (while (is (= k (Console.ReadLine)) "" ) (pr ": "))
      (= k (coerce k "Int32"))
      (if (not (member? k menu)) (do (prn error) (ask menu)))
      k
  )
) 

(def domenu ()
  "User to select menu item - 0 to exit"
  (with (item 1) 
     (while (not (is item 0)) 
         (= item (ask "Choice :" "No such option" menu)) 
         (if (is item 12) (readSensors) 
             (> item 0)   (runMotion item)
         )
      )
  )
)

;;;;;;;;;;;;;;;;;;;;;;;
;
; (def runMotion  (n ) (prn "Debug motion " n))
;

;;;;;;;;;;;;;;;;;;;;;;;;
;
; top level function
;
;;;;;;;;;;;;;;;;;;;;;;;;


(def run_robobuilder (& args)
  "Top level entry point"

  (if (empty? args) (= cp (askPort)) (= cp (car args)) )
  (connect cp)
  (MessageBox.Show "make sure robot is connected to serial port and on"  "warning" )
  (.open sport)

  (checkver)
  (prn "Serial Number = " (getsn))

  ; main loop until 0 selected
  (if (empty? args) (domenu))

  (.close sport)
  'ok 
) 
