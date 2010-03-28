;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
; 3D demo - uses CPI.Plot3D library
;
; http://www.codeproject.com/KB/GDI-plus/Plot3D.aspx
;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;




(reference "CPI.Plot3D")
(using     "CPI.Plot3D")

(= pd (new "Plotter3D" g)

(def DS (l) (for i 0 4 (do (.Forward pd l) (.TurnRight pd 90.0))))
(def DC (l) (for i 0 4 (do (DS l) (.forward pd l) (.turndown pd 90.0))))

(def DRS (l a)
  (.Penup     pd)
  (.forward   pd (/ l 2))
  (.turnright pd 90)
  (.forward   pd (/ l 2))
  (.turnleft  pd 90)

  (.turnright pd a)
  (.turnleft  pd 90)  
  (.forward   pd (/ l 2))
  (.turnleft  pd 90)
  (.forward   pd (/ l 2))

  (.turnleft  pd 180)
  (.PenDown   pd)
  (DS l)
)

(.moveto pd (new "Point3D" 100 100 10))


(.show form1)

(DRS 50 45)
(DRS 50 55)
(DRS 50 65)
(DRS 70 65)

(.hide form1)
