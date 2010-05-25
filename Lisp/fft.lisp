(reference "fft")
(using "MathNet.Numerics")


(def soundTest (nos) 
   (setSampling true)
   (= fft (new "Demo.fft"))
   
  (= p1 (new "Pen" (Color.FromName "Black")))
  (.set_DashStyle p1 (System.Drawing.Drawing2D.DashStyle.DashDot))
  (createwindow "Plot Distance Demo" 250 250)
  (.show form1) 
   
   (while (not (console.keyavailable)) 
          (= dp ())
          (= history ())
          (for i 1 nos 
             (= dp (cons (= snd (readSound)) dp))
             (prn (.PadLeft "*" snd #\-)  )
             ;(plot "MIC" (* 6 (- i 16) ) (- (* 5 snd) 20))
             (= history (cons (list (* 6 (- i 16) ) (- (* 5 snd) 20) ) history))
          )
          (= dp (vectors.ConvDouble (toarray dp)))
          (.fftReal fft dp)
          (= p ())
          (= h1 (reverse 
              (for n 0 (- nos 1) 
                (do
                 (= f (- (* n 4) 64))
                 (= a (* 2 (nth (.freqReal fft) n)))
                 (= p (cons (list f a) p))
                )
              )))
              
          (drawlist g history "Blue")  
          (drawlist g h1 "Red")  
          
          (= bw (vectors.scale (.freqReal fft) (vectors.convDouble '[0 0]) 1.0))
                  
          (= t (String.Format "Pk Amp = {0:#.##} , Pk Frq = {1}" 
                (vectors.maxValue bw)  
                (vectors.maxItem  bw)))
          (plot t 0 0 )
   )
   (setSampling false)
   (.close form1)
)


