using AutoKeybot.Core;

namespace AutoKeybot.Schedulers;

public class Routine {

    // an IRoutine is a scheduler which contains a list of Actions or ControllerCommands.
    // It would insert ControllerCommands into CommandQueue at certain time.
    private CommandQueue Queue { get; set; } = null;

    private Timer _timer;
    private RoutineCommand[] RoutineCommands;
    private int CommandIndex = 0;
    public bool Loop { get; set; } = false;

    public System.Action FinalAction = () => { };

    public void ConstructFromFile(string filePath) {
        ConstructFromLines(File.ReadLines(filePath));
    }

    public void ConstructFromLines(IEnumerable<string> lines) {
        var strArray = lines.ToArray();
        var index = 0;
        var cmdList = new List<RoutineCommand>();
        while (index < strArray.Length) {
            try {
                ParseLine(cmdList, ref index, strArray);
            }
            catch (Exception e) {
                throw new InvalidDataException($"Exception thrown when parsing line {index}: {strArray[index]}", e);
            }
        }
        RoutineCommands = cmdList.ToArray();
    }

    public void Start(CommandQueue queue, bool loop = false) {
        Loop = loop;
        Queue = queue;
        _timer = new Timer(Execute, null, 0, Timeout.Infinite);
    }

    private void ParseLine(List<RoutineCommand> outCmdList, ref int index, string[] strArray) {
        RoutineCommandType type;
        var line = strArray[index].Trim();
        if (line.Length == 0) {
            index += 1;
            return;
        }
        var words = strArray[index].Trim().Split();

        if (words[0] == "ACTION") {
            if (words[1] == "{") {
                index += 1;
                ParseAction(outCmdList, ref index, strArray);
            }
            else {
                throw new InvalidDataException("ACTION should be followed by a {} block.");
            }
        }
        else if (words[0] == "REPEAT" && words[2] == "{") {
            ParseRepeat(outCmdList, ref index, strArray, int.Parse(words[1]));
        }
        else if (words[0] == "WAIT") {
            index += 1;
            outCmdList.Add(new RoutineCommand() {
                Type = RoutineCommandType.WAIT_COMMAND,
                WaitTime = int.Parse(words[1]),
            });
        }
        else {  // controller command
            index += 1;
            type = RoutineCommandType.CONTROLLER_COMMAND;
            var cmd = new ControllerCommand(line);
            outCmdList.Add(new RoutineCommand() {
                RoutineControllerCommand = cmd,
                Type = type
            });
        }
    }

    private void ParseBlock(List<RoutineCommand> outCmdList, ref int index, string[] strArray) {
        while (strArray[index].Trim() != "}") {
            ParseLine(outCmdList, ref index, strArray);
        }
        index += 1;
    }

    private void ParseRepeat(List<RoutineCommand> outCmdList, ref int index, string[] strArray, int repeatNumber) {
        var start = index + 1;
        for (var i = 0; i < repeatNumber; i += 1) {
            index = start;
            ParseBlock(outCmdList, ref index, strArray);
        }
    }

    private void ParseAction(List<RoutineCommand> outCmdList, ref int index, string[] strArray) {
        var actionlines = new List<string>();
        while (strArray[index].Trim() != "}") {
            actionlines.Add(strArray[index].Trim());
            index += 1;
        }
        index += 1;
        var action = new Core.Action(actionlines);
        outCmdList.Add(new RoutineCommand() {
            RoutineAction = action,
            Type = RoutineCommandType.ACTION_COMMAND
        });
    }

    private void Execute(object? x) {
        while (true) {
            var rcmd = RoutineCommands[CommandIndex];
            if (rcmd.Type == RoutineCommandType.ACTION_COMMAND) {
                rcmd.RoutineAction!.Enqueue(Queue);
            }
            else if (rcmd.Type == RoutineCommandType.CONTROLLER_COMMAND) {
                Queue.Enqueue(rcmd.RoutineControllerCommand!);
            }
            else if (rcmd.Type == RoutineCommandType.WAIT_COMMAND) {
                _timer.Change(rcmd.WaitTime, Timeout.Infinite);
                CommandIndex = (CommandIndex + 1);
                if (CommandIndex >= RoutineCommands.Length) {
                    if (Loop)
                        CommandIndex -= RoutineCommands.Length;
                    else
                        Stop();
                }
                break;
            }
            CommandIndex = (CommandIndex + 1);
            if (CommandIndex >= RoutineCommands.Length) {
                if (Loop) {
                    CommandIndex -= RoutineCommands.Length;
                }
                else {
                    Stop();
                    break;
                }
            }
        }
    }

    public void Stop() {
        if (_timer != null)
            _timer.Dispose();
        if (Loop) {
            while (RoutineCommands[CommandIndex].Type != RoutineCommandType.WAIT_COMMAND) {
                CommandIndex = (CommandIndex - 1 + RoutineCommands.Length) % RoutineCommands.Length;
            }
        }
        else {
            FinalAction.Invoke();
        }
    }

    public void Reset() {
        if (_timer != null)
            _timer.Dispose();
        CommandIndex = 0;
    }
}