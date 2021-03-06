
(= rstep (list    '(123 156 212 80 108 126 73 40 150 141 68 44 40 138 208 195)
            '(130 165 201 81 115 134 81 31 147 149 72 44 40 145 209 201)
            '(132 171 197 83 117 137 86 28 148 152 78 43 41 154 209 206)
            '(132 175 195 87 117 139 91 27 152 154 87 43 43 164 209 211)
            '(132 178 197 91 117 137 95 28 157 152 97 43 48 172 209 213)
            '(130 179 201 95 115 134 96 31 161 149 105 43 53 179 210 214)
            '(127 178 206 98 112 130 95 35 166 145 111 42 57 182 210 214)
            '(124 175 212 100 109 127 92 40 170 142 113 42 59 183 210 214))
)

(= lstep (list    '(124 175 212 100 109 127 92 40 170 142 113 42 59 183 210 214)
            '(120 172 217 102 105 123 88 46 170 138 111 42 57 182 210 214)
            '(116 167 221 103 101 120 83 51 169 135 106 43 53 179 210 214)
            '(113 162 224 102 98 118 77 55 167 133 97 43 48 173 209 213)
            '(111 157 225 98 96 118 73 57 163 133 87 43 43 164 209 211)
            '(113 153 224 93 98 118 70 55 159 133 79 43 41 154 209 206)
            '(116 152 221 89 101 120 69 51 155 135 72 44 40 146 209 201)
            '(120 153 217 84 105 123 70 46 152 138 69 44 40 140 208 197)
            '(123 156 212 80 108 126 73 40 150 141 68 44 40 138 208 195))
)

(def setupTrig (min max)
   (prn "setup tigger")
   (if (not (bound 'trg))
      (= trg (new "RobobuilderLib.trigger")))
   (.set_PSD   trg min max) 
   (.set_timer trg 250)
   (.print     trg)
   (.set_trigger wck trg)
   (.activate  trg true)
)

(= first true)

(def poselist (dur stp pos)
   (= r true)
   (if (or pos)
      ( do ;(car pos)
         (= r (.PlayPose wck dur stp (toByteArray (cv18 (car pos))) first))
         ;(= r (.PlayPose wck dur stp (toByteArray (car pos)) first))
         (= first false)
         (if r (poselist dur stp (cdr pos)) )
      )
   )
   r
)

(def main ()
   (dcmodeOn)
   (setupTrig 0 60)   ; trigger set so can't stop
   (= ld 25)
   (= rd 25)
   (= first  true)    ; load current servo positions
   (= done   false)   

   (while (not done)  ; loop until done  
      (if (Console.keyavailable) 
           (do 
               (= key (.key (Console.ReadKey true)) )
               (prn "Pressed: " key)
               (if (is key (ConsoleKey.Q))          (= done true)
                   (is key (ConsoleKey.LeftArrow))  (do (= ld 30) (= rd 25))
                   (is key (ConsoleKey.RightArrow)) (do (= ld 25) (= rd 30))
                   (is key (ConsoleKey.SpaceBar))   (do (= ld 25) (= rd 25))
               )
               (prn "dbg: " ld ", " rd ", " done)
             )
       )
       
       (if (not (poselist rd 1 rstep)) (= done true)) ;left step - break on trigger     
       (if (not (poselist ld 1 lstep)) (= done true)) ; right step - break on trigger
    )
) 

;(main)