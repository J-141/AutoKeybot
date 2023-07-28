using AutoKeybot.Core;
using Action = AutoKeybot.Core.Action;

internal enum RoutineCommandType {
    ACTION_COMMAND, // ACTION {block}
    CONTROLLER_COMMAND,
    WAIT_COMMAND
}

internal class RoutineCommand {
    public RoutineCommandType Type { get; set; }

    public Action? RoutineAction { get; set; } = null;
    public ControllerCommand? RoutineControllerCommand { get; set; } = null;

    public int WaitTime { get; set; }
}