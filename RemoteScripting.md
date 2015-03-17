# Introduction #

Using Lisp embedded within VC. Uses the LSharp.NET / L# engine (see LispClient)

### Variables initialised at start time ###

```
form    Windows form access that LISP is running in
pcr     PCremote object handle
wck     wckMotion object handle
sport   Serial port object handle
```

### Additional functions ###

```
readIR            : Read IR - simulate
readVideo         : Read Video 
alert (x)         : Display alert box!
message (x)       : Display message
```

### example ###

```
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
```