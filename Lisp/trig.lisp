;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
;  L# software using RobobuilderLib to control
;  Humanoid robot - Robobuilder remotely via serial port
;
;  Example of using a PlayFile motion with a PSD trigger
;
;  l3v3rz - April 2010
;
;  http://en.wikiquote.org/wiki/Lisp_programming_language
;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;


(if (< (.Minor (.version (.GetName (reference "RobobuilderLib")))) 7)
 (prn "Need to update RobobuilderLib to v1.7")
)


(load "Lisp\\final.lisp")
(load "Lisp\\wckutils18.lisp")


 (run_robobuilder "COM3")
 (dcmodeOn)
 
 (.Playfile wck "walk2.csv")
 
   (= trg (new "RobobuilderLib.trigger"))
   (.set_PSD   trg 30 60)
   (.set_timer trg 250)
   (.print     trg)
   (.set_trigger wck trg)
   (.activate  trg true)
 
 (.Playfile wck "walk2.csv")
 
  ;(.set_accel trg -5 -5 50 8 8 70) ;
 
