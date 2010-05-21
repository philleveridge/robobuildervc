(prn  "Boot loader")

(= rbl (reference "C:\\Users\\Phil\\Desktop\\Robobuilder\\Code development\\robobuildervc\\RobobuilderLib\\bin\\Release\\RobobuilderLib.exe"))
(using "RobobuilderLib")

(= PATH '( "" "Lisp\\" ))

(def include (s & one)
  (with (f null)
  (if (not (bound 'INCLUDE)) (= INCLUDE null))
  (if (not (bound 'PATH))    (= PATH  '("")))
  
  (each p PATH (if (System.IO.File.exists (+ p s))  (= f (+ p s))))
  
  (if (or f) 
        (if (and (car one) (member? f INCLUDE)) 
          (prn "Already loaded")
          (do   (prn "Loading " f) 
                (load f) 
                
               (if (and (or f) (member? f INCLUDE)) 
               (prn "Warning: " f " already loaded")
               (= INCLUDE (cons f INCLUDE))) 
            )
        )    
        ;else
       (prn "No such file " s))
  )
  INCLUDE
)

(include "final.lisp")
(include "wckutils18.lisp")
(include "utilities.lisp")

(prn "RobobuilderLib version : " (.version (.GetName rbl)))


;(def toByteArray (x) (RobobuilderLib.vectors.convByte (toarray x))) ; from version RBL 1.9.9.7

(def trace (f) 
  "tracing on or off." 
  (if f (= DEBUG t) (ubound 'DEBUG)))

(include "dcmp.lisp")
(include "balance.lisp")
(include "scanner.lisp")
(include "motions.lisp")

(= basic18 '( 143 179 198  83 106 106  69  48 167 141  47  47  49 199 192 204 122 125))	
(def standup () (dcmodeOn)(sleep 0.5)  (.playpose wck 1000 10 (toByteArray basic18) true) )
(def rb () (connect "COM5") (standup) "ok")