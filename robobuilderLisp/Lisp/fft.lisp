; fft from version of RobobuilderLib 1.9.9.8

(def soundTest () 
  (setSampling true)
  (= p1 (new "Pen" (Color.FromName "Black")))
  (.set_DashStyle p1 (System.Drawing.Drawing2D.DashStyle.DashDot))
  (createwindow "Sound Demo" 250 250)
  (.show form1) 
  
  (= nos 64)
   
   (while (not (console.keyavailable)) 
          (= history ())
          (.clear g (Color.FromName "White"))
          (= sx 0) (= sy 0) (= fx 0) (= fy 0)
          (for i 1 nos 
             (= snd (readSound))
             (= fx (* 6 (- i 16) ) )
             (= fy (- (* 5 snd) 20) )     
             (drawline g sx sy fx fy "Blue")
             (= sx fx) (= sy fy)
          )           
   )
   (setSampling false)
   (.close form1)
)

(def drawbar (x h)

   (with (y 120 w 8 p2 (new "Pen" (Color.FromName "Red"))) 
     (= x (coerce x "Int32"))
     (= h (coerce h "Int32")) 
     (prn "x= " x ", h= " h)
     (.drawrectangle g p2 x y w h)
   )
)

(def fftTest () 
  (setSampling true)
  (= nos 64)
   
  (= p1 (new "Pen" (Color.FromName "Black")))
  (.set_DashStyle p1 (System.Drawing.Drawing2D.DashStyle.DashDot))
  (createwindow "FFT Demo" 320 250)
  (.show form1) 
   
   (while (not (console.keyavailable)) 
          (= dp ())

          
          (for i 1 nos 
             (= dp (cons (= snd (readSound)) dp))  ; need to know sampling speed best is 4ms or 125Hz
          )
          (= dp (vectors.ConvDouble (toarray dp)))
          (= fdata (vectors.fft dp))
          (= fdata (vectors.scale fdata (new "double[]" 0) 50.0))
    
          (= fx 0)  
          (.clear g (Color.FromName "White"))   
          (each fy fdata 
          (do 
             (= fx (+ fx 10))
             (drawbar  fx fy)
          ))        
   )
   (setSampling false)
   (.close form1)
)

;(tolist fdata)
