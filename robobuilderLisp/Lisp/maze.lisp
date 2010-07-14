;
; Maze solving based on C tutorial algorithm
;
; Note: (from tutorial 59-60)
; Turn right or left run times could be different depends on the floor status.

(load "Lisp\\final.lisp")
(load "Lisp\\utilities.lisp")

(def asknum ()  (with (k 0) (while (is (= k (Console.ReadLine)) "" ) (pr ": ")) (Convert.ToInt32 k)))


(def  runMotion    (x) "debug only" (prn "Run motion " x) (sleep 1))
(def  readDistance ()  "debug only" (prn "distance ? ") (asknum))


(def leftturn90 ()  (prn "LT90")  ( runMotion 3) (runMotion 3) (runMotion 3))
(def leftturn180 () (prn "LT180") ( runMotion 3) (runMotion 3) (runMotion 3)  (runMotion 3)  (runMotion 3))
(def rightturn90 () (prn "RT90")  ( runMotion 5) (runMotion 5) (runMotion 5) (runMotion 5))
(def forward ()     (prn "FWD")   ( runMotion 4) (runMotion 4) (runMotion 5) )
(def back()         (prn "BACK")  ( runMotion 10)  )

(def showmap ()     (for i 0 7 (do (prn (.Substring space (* i 8) 8 )))))
(def getmap (x y)   (str (nth space (+ x (* y 8)) )))
(def putmap (x y c) (with (p (+ x (* y 8))) (= space (+ (.substring space 0 p) c (.substring space (+ p 1))))))
(def clrmap ()      (do (= space  "........") (= space  (+ space space space space space space space space))  ))


(def maze () 
  (= px 4)
  (= py 4)
  (= pd 0) ; up
  (= pt '("^" ">" "v" "<"))
  (clrmap)
  
  (while true
    (do
       (= pd (mod pd 4))
       (putmap px py (nth pt pd)) (showmap) 

       (= d (readDistance))
       (if (> 12 d) (back)
           (> 30 d) (do 
                       (leftturn90) 
                       (= pd (+ pd 3))
                       (= d (readDistance))
                       (if (> 30 d)  (do
                                        (leftturn180) 
                                        (= pd (+ pd 2))
                                        (= d (readDistance))
                                        (if (> 30 d) ( do (rightturn90) 
                                                          (= pd (+ pd 1))))
                                      )
                        )
                      )
           (do 
           (forward)
           (putmap px py "*")
          
           ; update map
           (= pd (mod pd 4))
           (if (is pd 0) (= py (- py 1)) 
               (is pd 1) (= px (+ px 1)) 
               (is pd 2) (= py (+ py 1)) 
               (is pd 3) (= px (- px 1)) 
           )
           (if (< px 0) (= px 0) (< py 0) (= py 0) (> px 7) (= px 7) (> py 7) (= px 7))
           )
        )
        ;(exit?)
     )
   )
)

(maze)

                                      
       