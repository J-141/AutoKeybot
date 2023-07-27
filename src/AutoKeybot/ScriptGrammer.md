## ControllerCommand

## Action

should be a .action file

valid lines: (each line can be parsed to a controller command)
	
	// controller command

	KEY [KeyCommandType] word1, word2, .. 
	SKIP [int(millisecond)]
	START_ROUTINE [LOOP] [RoutineIdentifier] 
	STOP_ROUTINE  [RoutineIdentifier] 

	// nexted action

	ACTION [ActionIdentifier] // ACTION can call each other

Action is 1-1 mapped to a global identifier. (see Identifier)


## Routine
should be a .routine file

valid lines: (controller command, ACTION, or WAIT)

	WAIT [millisecond to wait] // different from skip, this is not blocking

	ADD_ACTION [ActionIdentifier]
	KEY [KeyCommandType] word1, word2, .. 
	SKIP [int(millisecond)]
	START_ROUTINE [LOOP] [RoutineIdentifier] 
	STOP_ROUTINE  [RoutineIdentifier]
	

Routine is 1-1 mapped to a global identifier. (see Identifier) 
Routine is considered as singleton, you cannot run 2 instances of same routine simultaneously.
if you start_routine with some routine already running, it will be restarted.

	NEW:
	ACTION {
	
	} // anonymous action. newlines around braces must be strictly same as the example.
	// action cannot be nested!! the lines in the braces must follow all the rules for a .action file. 
	
	REPEAT [num] {
	
	} // repeat lines in the braces several times. newlines around braces must be strictly same as the example.
	// this can be nested

	CREATE_ROUTINE [TemplateIdentifier] [args] // will also be started

Note: you cannot create 
# Routine/action Template
contains a set of aliases. can also accept args. should be created from a .[routine/action].template file.
will be text-substituted and then create routine.
	
	ALIAS [word] [
	line1
	line2
	]



## Identifier
must be in format path1.path2. ... .name
would try to find file in root/path1/path2 ... /name[.action/.routine]
root is specified when running the program.

all the things here are case sensitive.