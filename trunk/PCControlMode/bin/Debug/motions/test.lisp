;
; test file
;
(do 
  (alert "hello world")

  (message "hello")

; ir is simulated - click on image
  (= IR (readIR))
  (message "you pressed ")

;acceleromter test - need to be connected
  (alert "connect now")

  (message "acc ")
  (prn (tolist (readAcc)))

  (message "Distance ")
  (prn (readDistance))

;robot move - requires motion csv files
  (.NewBasicPose form)

)