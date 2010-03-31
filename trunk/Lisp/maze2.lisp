;
; maze solving with robobuilder
; l3v3rz March 2010
;
; based on algorith from Robobuilder C programming tutorial
;
;1. If there is no wall, move forward.
;2. If the wall is detected, check left side.
;3. If front and left side wall are detected, check right side. 
;4. If front, left and right side walls detected, go to opposite way.
;
;
; (load "final.lisp") 
; (run_robobuilder)
;
;
; debug only routines
; Real versions (using RobobuilderLib) of runMotion and readDistance are defined in final.lisp
;
(def asknum ()      (with (k 0) (while (is (= k (Console.ReadLine)) "" ) (pr ": ")) (if (is k "q") (err "Done") (Convert.ToInt32 k))))
(def runMotion      (x) "debug only" (prn "Run motion " x) (sleep 1))
(def readdistance   ()  "debug only" (asknum))


; check for break 
;
(def exit? () 
   (if (Console.keyavailable) 
     (if (is (.key (Console.ReadKey true)) (ConsoleKey.Q)) (err "Quit Pressed"))))

; read and display distance
; updated to handle null values returned by readdistance 18/3/2010
(def readDistance   () (with (x (readdistance)) (do (exit?) (if (or (is x "") (is x null)) (= x 0) )(prn "distance = " x))))


;mapping routines
;8x8 grid
;
(def showmap ()     (for i 0 7 (do (prn (.Substring space (* i 8) 8 )))))
(def getmap  (x y)  (str (nth space (+ x (* y 8)) )))
(def putmap (x y c) (with (p (+ x (* y 8))) (= space (+ (.substring space 0 p) c (.substring space (+ p 1))))))
(def clrmap ()      (= space  "........")   (= space (+ space space space space space space space space))  )

;motion routines (uses built in motions) 
;
(def leftturn90  () (prn "LT90")  ( runMotion 3) (runMotion 3) (runMotion 3))
(def leftturn180 () (prn "LT180") ( runMotion 3) (runMotion 3) (runMotion 3) (runMotion 3) (runMotion 3))
(def rightturn90 () (prn "RT90")  ( runMotion 5) (runMotion 5) (runMotion 5) (runMotion 5))
(def forward     () (prn "FWD")   ( runMotion 4) (runMotion 4) (runMotion 5) )
(def back        () (prn "BACK")  ( runMotion 10))
  
;maze algorithm
(def maze () 
  (= px 4) (= py 4)          ; assume center of maze
  (= pd 0)                   ; facing up
  (= pt '("^" ">" "v" "<"))  ; maze turtle character
  (= cs 0)                   ; current state = 0
  (clrmap)                   ; reset map
  
  (while true
     (= pd (mod pd 4))
     (putmap px py (nth pt pd)) (showmap) 
     (= d (readDistance))
     (if  (> 12 d)                   (back)
     
          (and (is cs 0) (> 30 d))   (do (leftturn90)  (= pd (+ pd 3)) (= cs 1))

          (and (is cs 1) (> 30 d))   (do (leftturn180) (= pd (+ pd 2)) (= cs 2))

          (and (is cs 2) (> 30 d))   (do (rightturn90) (= pd (+ pd 1)) (= cs 0))
          
          (do 
             (= cs 0)  
             (forward) 
             
             ; update map       
             (putmap px py "*")
             
             (= pd (mod pd 4))
             (if (is pd 0) (= py (- py 1)) 
                   (is pd 1) (= px (+ px 1)) 
                   (is pd 2) (= py (+ py 1)) 
                   (is pd 3) (= px (- px 1)))
             (if (< px 0) (= px 0) (< py 0) (= py 0) (> px 7) (= px 7) (> py 7) (= px 7))
           ) 
      )
  )
)

;
;  example session - debug only mode
;
;  > (maze)
;  ........
;  ........
;  ........
;  ........
;  ....^...
;  ........
;  ........
;  ........
;  distance ?
;  : 20
;  LT90
;  Run motion 3
;  Run motion 3
;  Run motion 3
;  ........
;  ........
;  ........
;  ........
;  ....<...
;  ........
;  ........
;  ........
;  distance ?
;  20
;  LT180
;  Run motion 3
;  Run motion 3
;  Run motion 3
;  Run motion 3
;  Run motion 3
;  ........
;  ........
;  ........
;  ........
;  ....>...
;  ........
;  ........
;  ........
;  distance ?
;  20
;  RT90
;  Run motion 5
;  Run motion 5
;  Run motion 5
;  Run motion 5
;  ........
;  ........
;  ........
;  ........
;  ....v...
;  ........
;  ........
;  ........
;  distance ?
;  60
;  FWD
;  Run motion 4
;  Run motion 4
;  Run motion 5
;  ........
;  ........
;  ........
;  ........
;  ....*...
;  ....v...
;  ........
;  ........
;  distance ?
;  60
;  FWD
;  Run motion 4
;  Run motion 4
;  Run motion 5
;  ........
;  ........
;  ........
;  ........
;  ....*...
;  ....*...
;  ....v...
;  ........
;  distance ?
;  q
;  Exception : Done
