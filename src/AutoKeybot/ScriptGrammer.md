## ControllerCommand

## Action

should be a .action file

valid lines: (controller command)

	KEY [KeyCommandType] word1, word2, .. 
	SKIP [int(millisecond)]

	START_ROUTINE [LOOP] [RoutineIdentifier] 

	PAUSE_ROUTINE  [RoutineIdentifier] 

	REMOVE_ROUTINE [RoutineIdentifier] 

    RESUME_ROUTINE [RoutineIdentifier] 

	CREATE_ROUTINE [LOOP] [TemplateIdentifier] [args] // create routine from template and start
	CREATE_ACTION [TemplateIdentifier] [args] // create action from template and execute

    EXEC_ACTION [ActionIdentifier]

    RESET // will stop
    RESTART 

	[Command 1] || [Command 2] || ... // would randomly pick one command to run. "||" is a reserved symbol. leave empty to do nothing.
	[Command 1] >> [Command 2] >> ... // similar to || but would execute the subcommand round-robin instead of randomly.
	every sequential controller command is like a singleton, because it need to hold state.

	# // comment, would be ignored

Action is 1-1 mapped to a global identifier. (see Identifier)


## Routine
should be a .routine file

valid lines: (controller command, ACTION {}, REPEAT {}, WAIT, or RANDOM)

	WAIT [millisecond to wait] // different from skip, this is not blocking

	ACTION {
	
	} // anonymous action. newlines around braces must be strictly same as the example.
	// action cannot be nested!! the lines in the braces must follow all the rules for a .action file. 

	REPEAT [num] {
	
	} // repeat lines in the braces several times. newlines around braces must be strictly same as the example.
	// this can be nested

	[Controller command] (same as # Action)

	[Command 1] || [Command 2] || ... // would randomly pick one command to run. "||" is a reserved symbol. leave empty to do nothing.
	// the routine command here must be single-line (no ACTION {} or REPEAT {})
	
	# // comment, would be ignored

Routine is 1-1 mapped to a global identifier. (see Identifier) 
Routine is considered as singleton, you cannot run 2 instances of same routine simultaneously.
if you start_routine with some routine already running, it will be restarted.
	

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