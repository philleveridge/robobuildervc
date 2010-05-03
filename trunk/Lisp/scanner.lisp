

(def scanw (a b)
  (= p1 (new "Pen" (Color.FromName "Black")))
  (.set_DashStyle p1 (System.Drawing.Drawing2D.DashStyle.DashDot))

  (createwindow "Plot Distance Demo" 250 250)
  (.show form1) 
  
  (with (d () history () n 0)
  (for i a b
   (do 
    (dcmodeOff)
    (= d (readdistance) )
    (dcmodeOn)
    (sleep 0.1)
    ; n = -100 + (i-a)*(200/(b-a))
    (= n (+ -100 (* (- i a) (/ 200 (- b a)))))
    (scope n d) 
    
     (= history (cons (list n d) history))
     ;(= h (list (car history) (cadr history) (car (cdr (cdr history)))))
     (drawlist g history "Blue")
     
    (setServoPos 20 i 4)
   )
  d)
  (pause)
  (.close form1)
  )
 )
