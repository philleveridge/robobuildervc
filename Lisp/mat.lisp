(= mat (new "matrix" 2 2))
(.set mat '[1.0 2.0 3.0 4.0])
(.print mat)
(.scale mat 1.1)
(.print mat)

(= mat2 (new "matrix" 2 2))
(.set mat2 '[2.0 4.0 3.0 1.0])

(.add mat mat2)
(.print mat)


(= matA (new "matrix" 2 3))
(.set matA '[1.0 4.0 2.0 5.0 3.0 6.0])
(.print matA)

(= matB (new "matrix" 3 2))
(.set matB '[7.0 8.0 9.0 10.0 11.0 12.0])
(.print matB)

;(= w (new "matrix" 4 4 '[0 -1 1 -1 -1 0 -1 1 1 -1 0 -1 -1 1 -1 0]))

(def clear()
  (= w (new "matrix" 4 4))
  (.print w)
)

; eg (train '[0 1 0 1])
;
(def train (x)
    (= inMat (new "matrix" 1 4 (matrix.bipolar x)))
    (= inv (.transpose inMat))
    (= res (.multiply inMat inv))
    (.zeroI res)
    (prn "Adding weights .. ")
    (.print res)
    (.add w res)
    (prn "result .. ")
    (.print w)
)






; eg (recog '[0 1 0 1] afn)
;
(def recog (x f)
   (= inp (new "matrix" 4 1 x))
   (.print inp)

   (for node 0 3 
     (prn node ": " (f (vectors.dotprod (.getrow inp 0) (.getrow w node)))))
)

(def tfn (r i o) (* r i o)) ; Hebbs



(def hebbs (n)
  (for i 1 n 
     (prn "epoc " i)
     
  )
)


(def xor (p)
  (= k (vectors.convDouble '[-1.0])) ;
  (= p (toarray (map afn p)))
  
  ;layer 1/input 
  (= inp   (new "matrix" 1 3 (vectors.append (vectors.convDouble p) k)))
  (prn "Input: ")(.print inp)
  
  ;scratch layer 12
  (= mat12 (new "matrix" 3 2 '[1.0 1.0 1.5 1.0 1.0 0.5]))        ; weights & thresholds
  (= out12 (.multiply mat12 inp))
  (prn "Output 2: ")(.print out12)
  
  ;scratch layer 23
  (= t (vectors.convDouble (toarray (+ (map afn (.getcol out12 0)) '(-1)))))   ; check activation + append '1'
  (= mat23 (new "matrix" 3 1 '[-1.0 1.0 0.5]))                                 ; weights & thresholds
  (= inp23 (new "matrix" 1 3  t))
  (= out23 (.multiply mat23 inp23))
  
  ;output layer 3
  (prn "Output 3: ")(.print out23)
  (map afn (.getcol out23 0))
)

(xor '[1.0 1.0])

; feed forward network (2 input, 3 hidden, 1 output)

(def afn (x) (if (> x 0) 1 0))  ; simple activation function

(def feedforward (p)
  (if (not (bound 'mat12))
     ;set  weights & thresholds
     (= mat12 (new "matrix" 3 3 '[0.1 0.2 0.7 0.3 0.1 0.7 0.4 0.2 0.7])) 
  )
     
  (if (not (bound 'mat23))
     ;set  weights & thresholds
     (= mat23 (new "matrix" 4 1 '[0.1 0.2 0.3 0.7])) 
  )

  (= k (vectors.convDouble '[-1.0])) ;
  (= p (toarray (map afn p)))        ; process inputs 0 or 1

  ;layer 1/input 
  (= inp   (new "matrix" 1 3 (vectors.append (vectors.convDouble p) k)))
  (prn "Input: ")(.print inp)
  
  ;scratch layer 12
  (= out12 (.multiply mat12 inp))
  (prn "Output 2: ")(.print out12)
  
  ;scratch layer 23
  (= t (vectors.convDouble (toarray (+ (map afn (.getcol out12 0)) '(-1)))))   ; check activation + append '1'
  (= inp23 (new "matrix" 1 4  t))
  (= out23 (.multiply mat23 inp23))
  
  ;output layer 3
  (prn "Output 3: ")(.print out23)
  (map afn (.getcol out23 0))
)

(feedforward '[1.0 -2.5])

(def sigmoid (x) (/ 1.0 (+ 1.0 (Math.exp x))))

(def dsigdot (x) (* (sigmoid x) (- 1.0 (sigmoid x))))
(.
(def edot (o) (* o (- 1.0 o)))

(= target 1.0)
(= lrate 0.5)

(def backpropagate (target)
    ; calculate deltas
    
    (= out (car (.getcol out23 0))) ; output layer
    (= delta (- target out))
    (prn "out " out ", Delta " delta ) ; 
    
    (= delo  (new "matrix" 1 1  (toarray (list delta))))
    
    (= del23 (.multiply delo mat23))    ; 4x1 matrix  = 1x1 * 4*1
    (= del23 (.resize del23 3 1))  

    (= del12 (.multiply del23 mat12))       ; 3x1 * 3x3  => 1x3 
    (= del12 (.resize del12 2 1))       ; input weights
    
    (prn "Del12")(.print del12)
    (prn "Del23")(.print del23)
    (prn "DelO") (.print delo )
      
    ;
    ; fix /update weights
    ;    weight  += learningrate * d1 * x1 * df(e)/de   (= dfde (* delta input (- 1 input)))
    ;    (.delta mat12) += LR * [x1 x2] *  [d3*DFE3 d4*DFE4 d5*DFE5]
    (.set      del23 (toarray (map dsigdot (.getAll del23))))
    (.scale    del23 lrate)
    (.add mat12 (.transpose (.multiply inp del23)))
  
    ;    weight  += learningrate * d1 * x1 * df(e)/de   (= dfde (* delta out (- 1 out)))
    ;    (.delta mat23) += LR * [y1 y2 y3] *  [d6*DFE6]
    (.set      delo (toarray (map dsigdot (.getAll delo))))
    (.scale    delo lrate)
    (.add  mat23 (.resize (.transpose (.multiply out12 delo)) 4 1) )
)


(def foo (n)
      (prn "Epoc - " (= n (+ n 1)))
      (feedforward '[0.0 0.0])
      (backpropagate 0.0)
      
      (feedforward '[1.0 0.0])
      (backpropagate 1.0)
      
      (feedforward '[0.0 1.0])
      (backpropagate 1.0)
      
      (feedforward '[1.0 1.0])
      (backpropagate 0.0)  
)

(def footest (n lr)
  (= lrate lr) 
  (= mat12 (new "matrix" 3 3 '[0.1 0.2 0.7 0.3 0.1 0.7 0.4 0.2 0.7]))        ; weights & thresholds
  (= mat23 (new "matrix" 4 1 '[0.1 0.2 0.3 0.7]))   
  (for epoc 0 n 
      (foo epoc))
)

(footest 20 0.5)

;(= m (new "matrix" 3 3 '[1.0 2.0 3.0 4.0 5.0 6.0 7.0 8.0 9.0]))
;(.print m)
;(.print (.resize m 2 2))
;(.print (.resize m 4 1))
