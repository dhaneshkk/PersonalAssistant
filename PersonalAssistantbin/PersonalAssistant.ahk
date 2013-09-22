!A::
  ; Show the Input Box to the user.
  inputbox, text, Diary,,,300,100

  ; Format the time-stamp.
  current=%A_DD%/%A_MM%/%A_YYYY%, %A_Hour%:%A_Min%
  logText ="q|%current%|%text%"
  ; Lets Personal processor Process it
  Run PAInterface.exe -b tcp://127.0.0.1:5000 -m %logText% -d 1000
return

!S::
run Notepad "Quote.txt
return


!D::
  ; Show the Input Box to the user.
  inputbox, text, Dictionary,,,,

  ; Format the time-stamp.
  current=%A_DD%/%A_MM%/%A_YYYY%, %A_Hour%:%A_Min%
  logText ="d|%current%|%text%"
  ; Lets Personal processor Process it
  Run PAInterface.exe -b tcp://127.0.0.1:5000 -m %logText% -d 1000
return

!F::
run Notepad "dictionary.txt
return
