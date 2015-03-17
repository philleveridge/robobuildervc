# Introduction #

This is a set of example lisp programs to show how access can be simplified through a number of Lisp based functions.

  * LSharpConsole application
  * balance.lisp
  * DCMP.lisp

# Details #

Download a ZIP archive with all necessary files : [RobobuilderLib.zip](http://robobuildervc.googlecode.com/files/Lsharp%20with%20RobobuilderLib.zip)

There is an online PDF tutorial to explain how to use LISP/L# to interface [PDF file](http://robobuildervc.googlecode.com/files/PDFOnline.pdf)

On start up the console will load boot.lisp. Example Session - from a cmd prompt

```
>LsharpConsole.exe
Boot.lisp loading
.Boot loader
...Loading Lisp\final.lisp
.....RobobuilderLib version : 1.9.9.8
.............
.Loading Lisp\wckutils18.lisp
........................
.Loading Lisp\utilities.lisp
............................................

L Sharp 2.1.0.0 on 2.0.50727.3603
Copyright (c) Rob Blackwell. All rights reserved.
Mods by l3v3rz.
>
```

### Default functions/variables ###

```
t true
nil false/empty
null nill

+ & xs         : If the first x is a number, returns the sum of xs, otherwise the concatenation of all xs. (+) returns 0.
- & xs         : Subtraction.
/ & xs         : Division.
* & xs         : Returns the product of the xs. (*) returns 1.
> & xs         : Greater than.
< & xs         : Less than.
atom? x        : Returns true if x is an atom.
car seq        : Returns the first item in a sequence.
caar seq       : Returns the first item of the first item in a sequence.
cadr seq       : Returns the second item in a sequence.
cddr seq       : Returns the rest of the rest of a sequence.
cdr seq        : Returns the rest of a sequence.
coerce x t     : Converts object x to type t.
compile expr   : Compiles the expression and returns executable code.
cons x seq     : Creates a new sequence whose head is x and tail is seq.
do & body      : Executes body forms in order, returns the result of the last body.
do1 & body     : Executes body forms in order, returns the result of the first body.
empty? x       : Returns true if x is an empty sequence.
err exception  : Raises an exception
eval expr      : Evaluates an expression.
idfn x         : The identity function, returns x.
is a b         : Returns true if a and b are the same.
last seq       : Returns the last item in a sequence.
length seq     : Returns the length of the sequence.
list & xs      : Creates a list of xs.
load filename  : Loads the lsharp expressions from filename.
member? item seq : Returns true is item is a member of seq.
mod & xs       : Returns the remainder when dividing the args.
new t & xs     : Constructs a new object of type t with constructir arguments xs.
not n          : Returns true if n is false, false otherwise.
nth n seq      : Returns the nth element in sequence.
progn & xs     : progn xs
reference & xs : Loads the given list of assemblies.
reverse seq    : Reverses the sequence.
seq x          : Returns x if x is a sequence, otherwise returns a Sequence represenation of x.
seq? x         : Return true if x is a sequence.
stdin          : Returns the standard input stream.
stdout         : Returns the standard output stream.
stderr         : Returns the standard error stream.
str & xs       : xxx
toarray seq    : Returns an object[] containing all the members of a sequence.
tolist seq     : Returns a list containing all the members of a sequence..
type x          : Returns the Type of x.
typeof t        : Returns the Type object named t.
uniq            : 
using xs        : XXX
set Macro       : (x y) : Set a variable
map (f s)       : Maps a function f over a sequence.
bound (sym)     : Returns true if sym is bound in the current environment.
safeset Macro   : (var val) : 
mac Macro       : (name parms & body) : Creates a new macro.
def Macro       : (name parms & body) : Defines a new function.
first seq       : Returns the first item in a sequence.
rest seq        : Returns the rest of a sequence.
len seq         : Returns the length of the sequence.
no n            : Returns true if n is false, false otherwise.
throw exception : Raises an exception
= Macro : (x y) : Set a variable
apply (f x)     : 
even (n)        : Returns true if n is even
inspect (x)     : Inspects the object x for debugging purposes.
msec            : Returns the current time in milliseconds.
sleep (n)       : Sleeps for n seconds
macex (x)       : 
macex1 (x)      : 
range (a b (c 1)) : 
pr (& xs)       : 
prn (& xs)      : 
help (f)        : Prints help documentation for f
sqrt (n)        : 
expt (x y)      : 
odd (n)         : Returns true if n is odd
isa (x t)       : Returns true if x is of type t.
pair (xs (f list)) : 
reduce (f s)  : 
and Macro     : (& xs) : 
with Macro    : (parms & body) : 
let Macro     : (var val & body) : 
or Macro      : (& xs) : 
each Macro    : (x xs & body) : 
while Macro   : (test & body) : 
for Macro     : (x start finish & body) : 
nor Macro     : args : 
when Macro    : (test & body) : 
unless Macro  : (test & body) : 
time Macro    : (expr)        : Times how long it takes to run the expression.
iso (x y)     : Isomorphic comparison of x and y.
testify (x)   : 
some? (f xs)  : 
every? (f xs) : 
```

### Loaded in at start time from  boot.lisp] ###

```
readSn            : Read serial number
readAcc           : read accelerometer values
readDistance      : Read distance
checkVer          : Check version of robobuilder firmware
serial?           : check if serial port connected
remote?           : check if robot connected
number? (x)       : check if x number
byte? (x)         : check if x byte
play (x)          : Play motion file
show-doc (x)      : Shows doc for an environment entry
dcmodeOn          : Enter DC mode (amber light on)
dcmodeOff         : exit DC mode (amber light off)
getServoPos (n)   : get position of servo id n
setPassive (n)    : set servo id n to read mode
setServoPos (id pos torq)   : set servo position (id: 0-31) (pos: 0-254) (torq: 0-3)
setSyncMove (id torq pos)   : Synchronous mode
getallServos (n)  : Get the current position of attached servos
smove (a b n tm)  : smooth move position a to b in n steps
add (x y)         : Add two vectors x and y together
diff (x y)        : Subtract two vectors x and y 
dot-product (a b) : calculate dot product of two vectors.
norm (a)          : normalise a vector i.e. sqrt(a.a)
basic18           :
stand             : Stand
readIR            : Read IR - simulate
readVideo         : Read Video 
alert (x)         : Display alert box!
message (x)       : Display message
repeat            : repeats
```