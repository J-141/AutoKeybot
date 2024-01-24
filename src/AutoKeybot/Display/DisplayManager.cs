using AutoKeybot.Core;
using AutoKeybot.Options;
using AutoKeybot.Schedulers;

namespace AutoKeybot.Display;

public class DisplayManager {
    private readonly Queue<string> executedQueue;
    private readonly int _maxSize;
    private CommandQueue comingQueue;
    public Dictionary<string, (Routine r, bool loop)> Routines { get; set; } = null;

    public DisplayManager(GlobalOptions option) {
        _maxSize = option.MaxDisplayQueueSize;
        executedQueue = new Queue<string>(_maxSize);
    }

    public string GetExecutedString() {
        var line = string.Join(" ", executedQueue.ToArray());
        if (line.Length > Console.WindowWidth) {
            return new string(line.Skip(line.Length - Console.WindowWidth).Take(Console.WindowWidth).ToArray());
        }
        else { return line; }
    }

    public void SetComingQueue(CommandQueue q) {
        comingQueue = q;
    }

    public void EnqueueCommand(string key) {
        executedQueue.Enqueue(key);

        if (executedQueue.Count >= _maxSize) {
            executedQueue.Dequeue();
        }
    }

    public string GetComingQueueDisplay() {
        return string.Join(string.Empty, comingQueue._queue.Select(x => GetCommandDisplay(x)).Reverse().ToArray());
    }

    private string GetCommandDisplay(IControllerCommand cmd) {
        if (cmd.CommandType == ControllerCommandType.KEY) {
            return $" {KeyAbbr.Abbr(cmd.CommandStrings[1])} ";
        }
        if (cmd.CommandType == ControllerCommandType.EXEC_ACTION) {
            return $"[{cmd.CommandStrings[0]}]";
        };
        return string.Empty;
    }

    public void Refresh() {
        Console.Clear();
        Console.WriteLine(string.Join(string.Empty, Enumerable.Repeat("-", Console.WindowWidth)));
        var space = string.Join(string.Empty, Enumerable.Repeat(" ", (Console.WindowWidth - 10) / 2 - 1));
        Console.WriteLine("|" + space + "AutoKeybot" + space + "|");
        Console.WriteLine(string.Join(string.Empty, Enumerable.Repeat("-", Console.WindowWidth)));
        foreach (var r in Routines.Keys) { Console.WriteLine(r); }
        Console.WriteLine(string.Join(string.Empty, Enumerable.Repeat("-", Console.WindowWidth)));
        Console.WriteLine(GetExecutedString());
        Console.WriteLine(string.Join(string.Empty, Enumerable.Repeat("-", Console.WindowWidth)));
        Console.WriteLine(string.Join(string.Empty, Enumerable.Repeat("-", Console.WindowWidth)));
        var commingStr = GetComingQueueDisplay();
        int i = 0;
        while (i < commingStr.Length) {
            Console.WriteLine(new string(commingStr.Skip(i).Take(Console.WindowWidth).ToArray()));
            i += Console.WindowWidth;
        }
        Console.WriteLine(string.Join(string.Empty, Enumerable.Repeat("-", Console.WindowWidth)));
    }
}