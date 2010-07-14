;;;robobuilder
;;; (load "\\Users\\Phil\\Desktop\\rbc.ls")

(reference "C:\\Users\\Phil\\Desktop\\Robobuilder\\Code development\\robobuildervc\\RobobuilderLib\\bin\\Debug\\RobobuilderLib.dll")
(reference "System.Windows.Forms")
(reference "System.IO.Ports")

(using "RobobuilderLib")
(using "System.Windows.Forms")
(using "System.IO.Ports")


(def connect () 
 (= sport (new "SerialPort"))
 (.set_BaudRate sport 115200)
 (.set_PortName sport "COM3")
 (= pcr (new "RobobuilderLib.PCremote" sport))
 (.open sport)
)

;;; PCremote command

(def getsn () (.readSN  pcr))
(def getX () (car (.readXYZ pcr)) )
(def getY () (car (cdr (.readXYZ pcr))) )
(def getZ () (car (cdr (cdr (.readXYZ pcr)))) )

(def checkver () 
 (if (is (.readVer pcr) "2.26") 
          (prn "latest") 
          (prn "Download new firmware")
 )
)

;;; wck based commands

(def dcmodeOn ()
  (= wck (new "RobobuilderLib.wckMotion" pcr))
)

(def dcmodeOff ()
  (.Close wck)
)

(def getServoPos (n)
  (.wckReadPos wck n)
  (car (cdr (.respnse wck)))
)

(def setServoPos (id pos torq)
  (.wckMovePos wck id pos torq)
  (car (cdr (.respnse wck)))
)

;;;wk.SyncPosSend(lastid, speed, p, 0);

(def setSyncMove (lastid torq position)
  (.SyncPosSend wck lastid torq position 0)
)


(MessageBox.Show  "Loaded?" "RoboBbuilder is Go!")


(= basic [143	179	198	83	106	106	69	48	167	141	47	47	49	199	204	204	122	125	127	0])

(def md (n xs ys step)
  (if (and (> n 0) xs ys)
      (cons (/ (- (car xs) (car ys)) step) (md (- n 1) (cdr xs) (cdr ys) step))
      nil))

(= s 5.0)
(md 3 '(4 3 2) '(3 1 1) s)

;;; (setSyncMove 18 2 basic)

(= x ()) 
(for i 1 10 (= x (cons  i x)))
(reverse x)

(with (a ()) (reverse (for i 1 10 (= a (cons  i a)))))


(def getallServos(n) (with (x () (for i 0 n (= x (cons (getServoPos i) x))))))


(time (for i 1 10 (progn (prn i "." ) (sleep 1))) )

(= zz '(1 apple 3 pear 2 apple 1 pear 1 apple))


(each x (pair zz) (progn (prn  (cdr x)) (sleep (car x)) ))








