;
; track object in video
;
(do 
  (alert "video tracking")

  (repeat 20
    (= video (readVideo))
    (if   (is video 5)   (prn "aim")
          (<  video 4)   (prn "turn_left")
          (>  video 6)   (prn "turn_right")
    )  
    (.wait form 1000)
  )
  (play "Basic")
)

