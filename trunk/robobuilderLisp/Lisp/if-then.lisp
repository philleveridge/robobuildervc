(def remember (new) (if  (member? new facts) nil (= facts (cons new facts) new)))


(remember "hello")
(remember "world")

(def recall (fact) (if (member? fact facts) fact nil))
 
(= rules '( 
   (rule identify1) (if (animal has hair)) (then (animal is mammal))
   (rule identify2) (if (animal gives milk)) (then (animal is mammal))
   (rule identify3) (if (animal has feathers)) (then (animal is bird))
   ))

(def testif (rule) ((with ifs) ))
(def usethen (rule))
(def tryrule (rule) (and (testif rule) (usethen rule)))


------------------------------------------------------------------------------------------

(def atomcar (z) (str (car (str (car z)))))
(def atomcdr (z) (str (cdr ( str (car p)))))
(def evalstring (s)  (eval (LSharp.Runtime.ReadFromString s)))


(def match ( p d)
  ;;(prn "debug: " p " & " d) 
  (if (and (empty? p) (empty? d)) t
      (or  (empty? p) (empty? d)) nil
      (or  (is (car p) '>)  (is (car p) (car d))) 
        (match (cdr p) (cdr d))
      (is  (car p) '+)
        (or (not (match (cdr p) (cdr d))) (match p (cdr d)))
  )
)




(match '( a b c) '(a b c))  ;; t
(match '( a b c) '(a b d))  ;; f
(match '( a > c) '(a b c))  ;; t
(match '( + b +) '(a b c))  ;; t
(match '(a + b) '(a x y b))

------------------------------------------------------------------------------------------
;;; need putprop / get
;;;

Terry Winograd 1972 - http://hci.stanford.edu/~winograd/shrdlu/index.html

(= propdb (new "System.Collections.Hashtable")) 


(def putprop (i v p) 
   (do (if (no (.contains propdb i)) (.add propdb i (new "System.Collections.Hashtable")) ) 
       (if (.contains (.Item propdb i) p) (.set_Item (.item propdb i) p v) (.add (.Item propdb i) p v))
    v)
)

(def get (i p) (.item (.item propdb i) p))

(def remprop (i p) (putprop i null p))

(putprop 'B1 'BRICK   'TYPE)
(putprop 'B1 '(1 1 0) 'POSITION)
(putprop 'B1 '(2 2 2) 'SIZE)
(putprop 'B1 'TABLE   'SUPPORTED-BY)
(putprop 'B1 '(B2)    'DIRECTLY-SUPPORTS)
(putprop 'B1 'RED     'COLOUR)

(putprop 'B2 'PYRAMID 'TYPE)
(putprop 'B2 '(1 1 2) 'POSITION)
(putprop 'B2 '(2 2)   'SIZE)
(putprop 'B2 'B1      'SUPPORTED-BY)
(putprop 'B2 'BLUE    'COLOUR)


(putprop 'HAND  nil      'GRASPING)
(putprop 'HAND  '(1 1 1) 'POSITION)

(= log ())


(def ungrasp (obj)  (remprop 'HAND 'GRASPING)     (= log (cons (list 'ungrasp obj) log )))

(def grasp (obj)    (putprop 'HAND obj 'GRASPING) (= log (cons (list 'grasp obj) log )))

(def movehand (pos) (putprop 'HAND pos 'POSITION))

(def putat (obj place) (do (grasp obj) (moveobj obj place) (ungrasp obj)))

(def moveobj (obj place) (do ))

> (grasp 'B1)

> (is 'B1 (get 'HAND 'GRASPING))
True
>

------------------------------------------------------------------------------

(def puton (obj sup)
  (with (place 0 plan 0)
    (if (= place (findspace sup obj)) (putat obj place)
        (= place (makespace sup obj)) (putat obj place)
        (reverse plan) )))
    

(def putat (ob pl)
     (grasp obj)
     (moveobj obj pl)
     (ungrasp obj) )

(def grasp (obj)
(with (k 0) 
  (if (is obj (get 'HAND 'GRASPING))  nil
      (get obj 'DIRECT-SUPPORT)       (cleartop obj)
      ((= k (get 'HAND 'GRASPING))    (getridof k)
  )
  (movehand (topcentre obj))
  (putprop 'HAND obj 'GRASPING)
  (= pl (cons (list 'GRASP obj) pl))
))

(def moveobj (obj ns)
   (removesupport obj)
   (movehand (newtopcentre obj ns))
   (putprop obj ns 'POSITION)
   (addsupport obj ns)
   (= plan (cons (list 'moveobj obj ns) plan))
)

(def movehand (pos) (putprop 'HAND pos 'POSITION))

(def ungrasp (obj)  (remprop 'HAND 'POSITION))

(def getridof (obj) (putat obj (findspace 'TABLE obj)))

(def cleartop (obj)
)

(def removesupport (obj)
)

(def addsupport obj pl)
)

(def namespace (sup obj)
)

;;; simulated block world

(def findspace (sup obj)
 (with (k) )
)

(def getobjunder (pl) (caddr pl))
(def topcontre (obj)  (list 'topcentre obj))
(def newtopcentre (obj pl) (list 'newtopcentre obj pl))
