;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
; L# software using RobobuilderLib to control
;  Humanoid robot - Robobuilder remotely via serial port
;
;  property lists and simple block world (SHRDLU or ASDFG in ascii) library 
;
;  l3v3rz - Dec 2009
;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
; see Terry Winograd 1972 - http://hci.stanford.edu/~winograd/shrdlu/index.html
;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
; utilities
;

(def getch () (.keychar (Console.ReadKey true)))

(def del (e l) (if (not l) null (is e (car l)) (del e (cdr l)) (cons (car l) (del e (cdr l)))))

(def prl  (a) (do (prl1 a)  (prn "") a))
(def prl1 (a) (do (pr "(" ) (each x a (if (seq? x) (prl1 x)(Console.Write "{0:0} " x ))) (pr ")")))


;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
; global property hash list
;
(def props ()
  (if (no (bound 'propdb)) (= propdb (new "System.Collections.Hashtable")) propdb)
)

(def putprop (i v p) 
   (props)
   (do (if (no (.contains propdb i)) (.add propdb i (new "System.Collections.Hashtable")) ) 
       (if (.contains (.Item propdb i) p) (.set_Item (.item propdb i) p v) (.add (.Item propdb i) p v))
    v)
)

(def get (i p)     (props)(.item (.item propdb i) p))

(def remprop (i p) (props)(putprop i null p))

(def prprop (i) 
   (props)
   (prn "ITEM " i)
   (with (x (.item propdb i))
   (each y (.keys x) 
     (Console.Write "{0,14} = " y)
     (if (seq? (.item x y)) 
         (prl (.item x y)) 
         (Console.Writeline "{0}"  (.item x y))
      ))))

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
; basic facts about block world

(def setfacts()
  (= plan ())
  (putprop 'B1 'BLOCK   'TYPE)
  (putprop 'B1 '(1 1 0) 'POSITION)
  (putprop 'B1 '(2 2 2) 'SIZE)
  (putprop 'B1 'TABLE   'SUPPORTED-BY)
  (putprop 'B1 '(B2)    'DIRECTLY-SUPPORTS)
  (putprop 'B1 'RED     'COLOUR)

  (putprop 'B2 'BLOCK   'TYPE)
  (putprop 'B2 '(1 1 2) 'POSITION)
  (putprop 'B2 '(2 2)   'SIZE)
  (putprop 'B2 'B1      'SUPPORTED-BY)
  (putprop 'B2 'BLUE    'COLOUR)

  (putprop 'B3 'BLOCK   'TYPE)
  (putprop 'B3 '(1 1 2) 'POSITION)
  (putprop 'B3 '(2 2)   'SIZE)
  (putprop 'B3 'TABLE   'SUPPORTED-BY)
  (putprop 'B3 '(B4)    'DIRECTLY-SUPPORTS)
  (putprop 'B3 'GREEN   'COLOUR)

  (putprop 'B4 'PYRAMID 'TYPE)
  (putprop 'B4 '(1 1 2) 'POSITION)
  (putprop 'B4 '(2 2)   'SIZE)
  (putprop 'B4 'B3      'SUPPORTED-BY)
  (putprop 'B4 'BLUE    'COLOUR)

  (putprop 'TABLE '(B1 B3)  'DIRECTLY-SUPPORTS)

  (putprop 'HAND  nil      'GRASPING)
  (putprop 'HAND  '(1 1 1) 'POSITION)
  'DONE
)

(def showfacts ()
  (map prprop '(B1 B2 B3 B4 TABLE HAND))
  'DONE
)


;-----
;    |
;   HAND
;
;   ----      ^
;   |B2|     /B4\
;   ----     ----
;   ----     ----
;   |B1|     |B3|
;   ----     ----
; ==================

;
; simple commands
;

;(def ungrasp (obj)  (remprop 'HAND 'GRASPING)     (= plan (cons (list 'ungrasp obj) plan )))
;(def grasp (obj)    (putprop 'HAND obj 'GRASPING) (= plan (cons (list 'grasp obj) plan )))
;(def movehand (pos) (putprop 'HAND pos 'POSITION) (= plan (cons (list 'movehand pos) plan )))
;(def putat (obj place) (do (grasp obj) (moveobj obj place) (ungrasp obj)))
;(def moveobj (obj place) (movehand place))
; example
;> (grasp 'B1)
;> (is 'B1 (get 'HAND 'GRASPING))
;True


;
; More complex commands
;


(def puton (obj sup)
  (= plan ())
  (with (place 0)
    (if (= place (findspace sup obj)) (putat obj place)
        (= place (makespace sup obj)) (putat obj place)
    )
   )
  (reverse plan)
)
    
(def putat (obj pl)
     ;(prn "DBG: putat" obj pl)
     (grasp obj)
     (moveobj obj pl)
     (ungrasp obj) 
)

(def grasp (obj)
 ;(prn "DBG: grasp" obj)
 (with (k 0) 
  (if (is obj (get 'HAND 'GRASPING))  
      nil
  (do
    (if    (get obj 'DIRECTLY-SUPPORTS)    (cleartop obj)
           (= k (get 'HAND 'GRASPING))     (getridof k))
    (movehand (topcentre obj))
    (putprop 'HAND obj 'GRASPING)
    (= plan (cons (list 'grasp obj) plan))
  )
  )
))

(def moveobj (obj ns)
;(prn "DBG: moveobj" obj ns)
   (removesupport obj)
   (movehand (newtopcentre obj ns))
   (putprop obj ns 'POSITION)
   (addsupport obj ns)
   (= plan (cons (list 'moveobj obj ns) plan))
)

(def movehand (pos) 
;(prn "DBG: movehand" pos)
(putprop 'HAND pos 'POSITION)
)

(def ungrasp (obj)  
;(prn "DBG: ungrasp " obj )
  (if  (get obj 'SUPPORTED-BY)
       (do (remprop 'HAND 'GRASPING)
           (= plan (cons (list 'ungrasp obj) plan))
       )
       nil
  )
)

(def getridof (obj) 
;(prn "DBG: getridoff" obj)
(putat obj (findspace 'TABLE obj))
)

(def cleartop (obj)
;(prn "DBG: cleartop" obj)
  (map getridof (get obj 'DIRECTLY-SUPPORTS))
)

(def removesupport (obj)
;(prn "DBG: removesupport" obj)
   (= sup (get obj 'SUPPORTED-BY))
   (if sup (putprop sup (del obj (get sup 'DIRECTLY-SUPPORTS)) 'DIRECTLY-SUPPORTS))
   (putprop obj null 'SUPPORTED-BY)
)

(def addsupport (obj pl)
;(prn "DBG: addsupport" obj pl)
(= sup (getobjunder pl))
;(prn "DBG: addsupport " sup " - " (get sup 'TYPE))

(if (or (is sup 'TABLE) (is (get sup 'TYPE) 'BOX) (is (get sup 'TYPE) 'BLOCK))
       (putprop sup (cons obj (get sup 'DIRECTLY-SUPPORTS)) 'DIRECTLY-SUPPORTS))
(putprop obj sup 'SUPPORTED-BY)
)

(def makespace (sup obj) 
;(prn "DBG: makespace" sup obj)
(with ( p 0 n 0 j 0 fl true)
 (= j (get sup 'DIRECTLY-SUPPORTS))
 (while fl
;(prn "DBG: makespace j=" j)
   (= n (car j))
   (= j (cdr j))
   (getridof n)
   (if (= p (findspace sup obj)) (= fl false))
 )
 p
)
)

;;; simulated block world

(def findspace (sup obj)
 ;(prn "DBG: findspace" sup obj)
 (with (k) 
    (if (is sup 'TABLE)          (list 'space 'above sup 'for obj)
        (= k (get sup 'DIRECTLY-SUPPORTS)) 
		( do
              (prn sup " supports " k)
              (prn "type 't' if findspace wins")
              (if (not (is (str (getch)) "t")) nil (list 'space 'above sup 'for obj))
		)
        (list 'space 'above sup 'for obj)
    )
 )
)

(def getobjunder (pl) 
;(prn "DBG: getobjunder" pl)
(car (cddr pl)))

(def topcentre (obj)  
;(prn "DBG: topcentre" obj)
(list 'topcentre obj))

(def newtopcentre (obj pl) 
;(prn "DBG: newtopcentre" obj pl)
(list 'newtopcentre obj pl))

(setfacts)

(prn "Blockworld! - initial state")
(prn "")
(showfacts)

(prn "")
(prn "Commands : (setfacts)      - reset facts")
(prn "           (showfacts)     - show facts")
(prn "           (puton 'B1 'B3) - put 'B1 on 'B3 and show plan")
'Ready

