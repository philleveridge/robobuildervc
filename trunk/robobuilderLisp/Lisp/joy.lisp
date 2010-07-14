;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
; L# software using RobobuilderLib to control
;  Humanoid robot - Robobuilder remotely via serial port
;
;  Joystick library 
;
;  l3v3rz - Dec 2009
;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

(def ask (prompt error menu)
  "Request user supply input from menu list"
  (with (k 0 )
      (prn prompt)
      (each x (pair menu) (prn (car x) " - " (cdr x)))
      (while (is (= k (Console.ReadLine)) "" ) (pr ": "))
      (= k (coerce k "Int32"))
      (if (not (member? k menu)) (do (prn error) (ask menu)))
      k
  )
)


(def exit? () 
   (if (Console.keyavailable) 
     (if (is (.key (Console.ReadKey true)) (ConsoleKey.Q)) (err "Quit Pressed"))))

(reference "Microsoft.DirectX.DirectInput")
(using "Microsoft.DirectX.DirectInput")


(def askjoy ()
  "Pick joystick from List"
  (with ( n 1 menu '(0 Exit)) 
    (= gl (Manager.GetDevices (DeviceClass.GameControl) (EnumDevicesFlags.AttachedOnly) ))
 
    (each l gl (
         do
         (= menu (cons n (cons  (.InstanceName l) menu)))
         (= n (+ n 1)))
    )        
    (= n (ask "Select Joystick Device" "No such option" menu))
    
    ;This picks the item from the list 
    (if (is n 0) null (nth gl (- n 1)))
  )
)
;(askjoy)

(def setjoy (l)
  "Aquire joystick for use by application"
  ;This creates the Device handle
  (= jd (new "Device" (.InstanceGuid l)))

  ;This specifies its a Joystick
  (.SetDataFormat jd (DeviceDataFormat.Joystick))

  ;This grabs it for your application
  (.Acquire jd)

  ;This lets you see the capabilities
  ( = cps (.Caps jd))
  (prn "Aquired - axes=" (.NumberAxes cps) " buttons=" (.NumberButtons cps))
)

(def getjoy ()
  ;This polls the state to see whats changes
  (.poll jd)
  (prn (.CurrentJoystickstate jd))

  ;This displays what buttons have been pressed
  (tolist (.getbuttons (.CurrentJoystickstate jd)))
)

(= rconmap '((0 Button1) (1 Button2) (2 Button3) ))
(= rconx   '((0 Left) (65535 Right)))
(= rcony   '((0 Up)   (65535 Down )))

(def remcon(t l)
   (.poll jd)
   (= cs (.CurrentJoystickstate jd))
   
   (= but (tolist (.getbuttons (.CurrentJoystickstate jd))))
   
   (= r null)  
   (if (is t "BUTTON")
         (each b l 
            (if (> (nth but (car b)) 0) (= r (cadr b)))
         )
       (is t "X") 
         (each b l
            (if (is (.X cs) (car b) )   (= r (cadr b)))
         ) 
       (is t "Y")         
         (each b l
            (if (is (.Y cs) (car b) )   (= r (cadr b)))
         )
   ) 
   r  
)

(def getButton ()
  (with (r () a ())
     (while (not a) (= a (remcon "BUTTON" rconmap)))
     (= r a)
     (while (or a) (= a (remcon "BUTTON" rconmap)))
     r  
))

(def getXY ()
  (with (r () x () y())
     (while (not (or x y))
       (= x (remcon "X" rconx))
       (= y (remcon "Y" rcony))
     )
     (= r (list x y))
     (while (not (or x y))
       (= x (remcon "X" rconx))
       (= y (remcon "Y" rcony))  
     )  
     r 
))

(def demojoy ()
  (= l (askjoy))
  (if (or l)
     (do (setjoy l)
         (while true 
          ( do
           (= a ()) (= b a) (= c a)    
           ; wait for button press
           (prn "Press any button")

           (while (not (or a b c))
           ( do
             (= a (remcon "BUTTON" rconmap))
             (= b (remcon "X" rconx))
             (= c (remcon "Y" rcony))
              (exit?)
           )
           )
           
           (and a (prn a))
           (and b (prn b))
           (and c (prn c))
           
           ;wait for let go
           
           (while (or a b c)
            ( do
             (= a (remcon "BUTTON" rconmap))
             (= b (remcon "X" rconx))
             (= c (remcon "Y" rcony))
             
             (exit?)
           )  
           )                             
           )
         )
       )
    )
)
;(demojoy)

