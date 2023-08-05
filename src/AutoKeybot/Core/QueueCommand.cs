using AutoKeybot.Core;
using AutoKeybot.Schedulers;

internal enum QueueCommandType {
    ENQUEUE,
    INSERT,
    EXEC  // only enqueue if queue is empty. otherwise, drop.
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

    public bool ExecuteOn(Controller ctr) {
        if (Type == QueueCommandType.ENQUEUE) {
            ctr.EnqueueCommand(Command);
            return true;
        }
        else if (Type == QueueCommandType.INSERT) {
            ctr.InsertCommand(Command);
            return true;
        }
        else if (Type == QueueCommandType.EXEC) {
            if (ctr.Queue.Count == 0) {
                ctr.EnqueueCommand(Command);
                return true;
            }
        }
        return false;
    }
}