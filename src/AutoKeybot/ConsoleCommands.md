## [any key]
press any key would stop all the routines and enter "input mode" in which you can type commands.
will display the routines to be run.
the routines would be restored 

## run
Start running, leave "input mode"
will display all routines running

# reset
will stop and reset the added routine

## add [loop] [RoutineIdentifier] 

## create [loop] [TemplateIdentifier] [word-list] 
crete routine from template. will also add to routine list.
the identifier would be template name_word1_word2_...

## remove  [RoutineIdentifier]

all the things here are case sensitive.

# cmd line options 

## queue mode [--queue]

in this mode, normal commands are disabled. each line you type in the input stream would be parsed into a queue command and enqueue/insert.
Use this if you have another program take control of the commands. 
[ENQUEUE/INSERT] [ControllerCommand]