;;init.lisp
(reference "RobobuilderLib" 
           "System.Windows.Forms")

(using     "RobobuilderLib"
           "System.Windows.Forms"
           "System.IO.Ports")
           
(def getsn ()
  "Read serial number"
  (if (not (.Isopen sport)) 
        (.open sport))
  (= sn (.readSN  pcr))
  sn
) 

(def readAcc ()
  (if (not (.isopen sport)) 
      (.open sport))  
  (.readXYZ pcr)
)

(def readdistance ()
  "Read distance"
  (if (not (.Isopen sport)) 
      (.open sport))
  (= d (.readDistance  pcr))
  d
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

(def serial? () (if (and (bound 'sport) (.isopen sport)) "Green" "Red"))
(def remote? () (if (and (bound 'pcr) (is "Green" (serial?))) "Green" "Red"))


(def play (x) "Play motion file" (.play form x))

(def show-doc (x)
     "Shows doc for an environment entry"
     (pr (.key x) " ")
     (help (.value x)))

