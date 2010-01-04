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
  
  'DONE
)