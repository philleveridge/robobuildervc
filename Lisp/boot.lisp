(prn  "Boot loader")

(load "Lisp\\final.lisp")
(load "Lisp\\wckutils18.lisp")
(load "Lisp\\utilities.lisp")

(def standup () (dcmodeOn)(sleep 0.5)  (.playpose wck 1000 10 (toByteArray basic18) true) )

(def rb () (connect "COM5") (standup) "ok")

(def dtest () 
   (while (not (console.keyavailable)) 
       (do (.wckReadPos wck 30 5) (prn (.PadLeft "*" (nth (.respnse wck) 0) #\-)  ))))

(prn "RobobuilderLib version : " (.version (.GetName (reference "RobobuilderLib"))))

(def trace (f) 
  "tracing on or off." 
  (if f (= DEBUG t) (ubound 'DEBUG)))

(load "dcmp.lisp")
(load "Lisp\\balance.lisp")

;(load "Lisp\\scanner.lisp")

;(load "pepep\\PepePLib.lisp")