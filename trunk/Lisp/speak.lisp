(reference "C:\\Program Files\\Reference Assemblies\\Microsoft\\Framework\\v3.0\\System.Speech.dll")

(using "System.Speech.Synthesis")
(using "System.Speech.Recognition")

(= sp (new "SpeechSynthesizer"))

;(.SelectVoice sp "Microsoft Anna")


;; Display a greeting depending on the hour
(if (< (.Hour (DateTime.Now)) 12) (= greet "Good Morning") 
	(< (.Hour (DateTime.Now)) 18) (= greet  "Good Afternoon") 
	(= greet  "Good Evening"))
	

(.Speak sp greet)

