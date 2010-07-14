(prn  "Boot loader")

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
(include "dcmp.lisp")   ;uncomment if using DCMP firmware


