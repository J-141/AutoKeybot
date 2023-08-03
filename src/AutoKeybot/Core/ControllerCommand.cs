namespace AutoKeybot.Core;

public enum ControllerCommandType {
    KEY = 1,
    SKIP = 2,
    START_ROUTINE = 3,
    REMOVE_ROUTINE = 4,
    CREATE_ROUTINE = 5,
    PAUSE_ROUTINE = 6,
    RESUME_ROUTINE = 7,
    EXEC_ACTION = 8,
    CREATE_ACTION = 9,
    RESET = 10, // will stop
    RESTART = 11
}

public interface IControllerCommand {

    // A ControllerCommand is something that the controller can execute sequentially.
    // ControllerCommandType can be divided into 2 groups: ARDUINO, and non-ARDUINO (or controlling command)
    // Will execute multiple commands in a single clock period unless specific controlling commands are executed (e.g. SKIP)
    public ControllerCommandType CommandType { get; }

    public string[] CommandStrings { get; }
}

internal class ControllerCommand : IControllerCommand {
    public ControllerCommandType CommandType { get; private set; }
    public string[] CommandStrings { get; private set; }

    public ControllerCommand(string line) : this(line.Split()) {
    }

    public ControllerCommand(string[] words) {
        if (Enum.TryParse<ControllerCommandType>(words[0], out var cmdType)) {
            if (cmdType == ControllerCommandType.KEY) {
                CommandType = ControllerCommandType.KEY;
            }
            else {
                CommandType = cmdType;
            }
            CommandStrings = words.Skip(1).ToArray();
        }
        else {
            throw new InvalidDataException($"Invalid Controller Command Type: {words[0]}.");
        }
    }

    public void ConstructFrom(ControllerCommandType cmdType) {
        if (cmdType == ControllerCommandType.KEY) {
            throw new InvalidDataException("Please use ControllerCommand(IArduinoCommand cmd) to create ARDUINO_COMMAND-type ControllerCommand.");
        }
        CommandType = cmdType;
    }
}