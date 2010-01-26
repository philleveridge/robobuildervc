;
; test file
;
(do 
  (alert "hello world")

  ; ir is simulated - click on bitmap image
  (message (str "you pressed " (readIR)))

  ; acceleromter test - need to be connected
  (alert "connect now")

  (message (str "Accelerometer " (readAcc)))

  (message (str "Distance " (readDistance)))

  ;robot move - requires motion csv files
  (.NewBasicPose form)
  

  
  (.speak speak "Speech Synthesis and recognition test")
  (.speak speak "Say yes or no")
 
  (.initrecogniser form '["yes" "no"])
  (= r (.SpeechRecognize form))
  (alert r)
  
   'DONE 
)