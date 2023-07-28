using AutoKeybot.Core;
using AutoKeybot.Schedulers;

internal enum QueueCommandType {
    ENQUEUE,
    INSERT
}

internal class QueueCommand {
    public QueueCommandType Type { get; set; }

    public ControllerCommand Command { get; set; } = null;

    public QueueCommand(string line) : this(line.Trim().Split()) {
    }

    public QueueCommand(string[] words) {
        if (!Enum.TryParse<QueueCommandType>(words[0], out var typ)) {
            throw new InvalidDataException($"Invalid queue command type: {words[0]}");
        }
        Type = typ;
        Command = new ControllerCommand(words.Skip(1).ToArray());
    }

    public void ExecuteOn(Controller ctr) {
        if (Type == QueueCommandType.ENQUEUE) {
            ctr.EnqueueCommand(Command);
        }
        else {
            ctr.InsertCommand(Command);
        }
    }
}