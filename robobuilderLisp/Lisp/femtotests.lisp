	//
	
	(set 'a '(eq -1 (readIR)))
	(set 'b '(do (prn "loop - IR=" (readIR)) (sleep 50)))
	(while (eval 'a) (eval 'b))
	
		// (while (eq -1 (readIR)) (do (prn "loop - IR=" (readIR)) (sleep 500)))