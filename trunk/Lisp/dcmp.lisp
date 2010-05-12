(prn  "DCMP Mode")

(= DCMODEPLUS 1)

(def dcmodeOn ()
  "Enter DC mode (amber light on)"
  (if (not (.isopen sport)) (.open sport))
  (if (bound 'wck) 
    (.setDCmode pcr true)
    (= wck (new "RobobuilderLib.wckMotion" pcr))
  )
)

(def dcmodeOff ()
  "exit DC mode (amber light off)"
)

(def readdistance ()
  "Read distance"
  (if (not (.Isopen sport)) 
      (.open sport))
      
  (with (d 0) 
    (.wckReadPos wck 30 5) ; power psd up and read sensor
    (= d (nth (.respnse wck) 0))
  )
)

(def psdon ()
    (.wckReadPos wck 30 3) ; power psd up
)

(def psdoff ()
    (.wckReadPos wck 30 4) ; power psd down
)

(def readAcc ()
  (if (not (.isopen sport)) 
      (.open sport))  
      
  (with (x 0 y 0 z 0) 
    (.wckReadPos wck 30 1) ; get y & Z
    (= y (nth (.respnse wck) 0))
    (= z (nth (.respnse wck) 1))
    (.wckReadPos wck 30 2) ; get x & Z  
    (= x (nth (.respnse wck) 0))
    (if (> x 127) (= x (- x 256)))
    (if (> y 127) (= y (- y 256)))
    (if (> z 127) (= z (- z 256)))
    (list x y z)
  )
)

(def checkver () 
  "Check version of robobuilder firmware"
  (prn "Not available in DCMP")
)

(def runSound (n )
   "Play sound id"
  (prn "Not available in DCMP")
)
(def runMotion (n )
  "Play motion id"
  (prn "Not available in DCMP")
)


(def dtest () 
   (while (not (console.keyavailable)) 
       (do (.wckReadPos wck 30 5) (prn (.PadLeft "*" (nth (.respnse wck) 0) #\-)  ))
   )
   (psdoff)
)
       