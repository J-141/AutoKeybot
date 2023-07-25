using AutoKeybot.Core;
using AutoKeybot.Display;
using AutoKeybot.KeyboardModule;
using AutoKeybot.Options;
using Timer = System.Timers.Timer;

namespace AutoKeybot.Schedulers;

internal class Controller {
    private Timer _timer { get; set; }
    public int GlobalClockInterval;
    public CommandQueue Queue { get; set; } //null command means skip one clock interval
    public int ToSkip = 0;
    private int maxQueueLength { get; set; }
    private int minQueueLength { get; set; }

    private IKeyboardExecutor _sender { get; set; }
    private DisplayManager _displayManager;
    private bool addingNewCommands { get; set; } = true;
    public Dictionary<string, (Routine r, bool loop)> Routines { get; set; } = new Dictionary<string, (Routine r, bool loop)>();

    public Controller(GlobalOptions options, IKeyboardExecutor sender, DisplayManager display) {
        _timer = new Timer(options.GlobalClockInterval);
        _timer.AutoReset = true;
        _timer.Elapsed += (s, o) => Execute();
        maxQueueLength = options.MaxQueueLength;
        minQueueLength = options.MinQueueLength;
        Queue = new CommandQueue();
        _sender = sender;
        GlobalClockInterval = options.GlobalClockInterval;
        _displayManager = display;
        display.Routines = Routines;
    }

    public void Run() {
        _timer.Start();
        foreach (var (r, l) in Routines.Values) {
            r.Start(Queue, l);
        };
    }

    public void AddRoutine(string identifier, bool loop = false) {
        var routine = ScriptManager.GetRoutine(identifier);
        routine.FinalAction = () => { if (Routines.ContainsKey(identifier)) Routines.Remove(identifier); };
        Routines[identifier] = (ScriptManager.GetRoutine(identifier), loop);
    }

    public void StartRoutine(string identifier, bool loop = false) {
        //if already started, stop and then restart it
        AddRoutine(identifier, loop);
        Routines[identifier].r.Reset();
        Routines[identifier].r.Start(this.Queue, loop);
    }

    public void RemoveRoutine(string identifier) {
        if (Routines.TryGetValue(identifier, out var rl)) {
            rl.r.Reset();
            Routines.Remove(identifier);
        }
    }

    public void CreateAndRunRoutine(string[] words, bool loop = false) {
        var identifier = string.Join("_", words);
        CreateRoutine(words, loop);
        Routines[identifier].r.Reset();
        Routines[identifier].r.Start(this.Queue, loop);
    }

    public void CreateRoutine(string[] words, bool loop = false) {
        var identifier = string.Join("_", words);
        var routine = ScriptManager.GetRoutineFromTemplate(words[0], words.Skip(1).ToArray());
        routine.FinalAction = () => { if (Routines.ContainsKey(identifier)) Routines.Remove(identifier); };
        Routines[identifier] = (ScriptManager.GetRoutine(identifier), loop);
    }

    public void Stop() {
        _timer.Stop();
        foreach (var (r, l) in Routines.Values) {
            r.Stop();
        };
        _sender.SendCommand(new KeybotCommand(new string[] { "KEY_RELEASE_ALL" }));
    }

    public void Reset() {
        _timer.Stop();
        Queue.Reset();
        foreach (var (r, l) in Routines.Values) {
            r.Reset();
        };
        Routines.Clear();
        _sender.SendCommand(new KeybotCommand(new string[] { "KEY_RELEASE_ALL" }));
    }

    private void Execute() {
        var commands = new List<IKeybotCommand>();
        if (Queue.Count > maxQueueLength) {
            foreach (var (r, l) in Routines.Values) {
                r.Stop();
            };
            addingNewCommands = false;
        }
        else if (Queue.Count < minQueueLength && addingNewCommands == false) {
            foreach (var (r, l) in Routines.Values) {
                r.Start(Queue, l);
            };
            addingNewCommands = true;
        }
        if (ToSkip > 0) {
            ToSkip--;
            _displayManager.EnqueueCommand(" ");
            return;
        }

        while (Queue.TryDequeue(out IControllerCommand command)) {
            if (command.CommandType == ControllerCommandType.KEY) {
                var cmd = new KeybotCommand(command.CommandStrings);
                commands.Add(cmd);
                if (command.CommandStrings.Length > 1)
                    _displayManager.EnqueueCommand(KeyAbbr.Abbr(command.CommandStrings[1]));
            }
            else if (command.CommandType == ControllerCommandType.SKIP) {
                ToSkip += int.Parse(command.CommandStrings[0]) / GlobalClockInterval;
                break;
            }
            else if (command.CommandType == ControllerCommandType.START_ROUTINE) {
                if (command.CommandStrings.Count() > 1 && command.CommandStrings[0] == "LOOP") {
                    StartRoutine(command.CommandStrings[1], true);
                }
                else {
                    StartRoutine(command.CommandStrings[0]);
                }
            }
            else if (command.CommandType == ControllerCommandType.REMOVE_ROUTINE) {
                RemoveRoutine(command.CommandStrings[0]);
            }
            else if (command.CommandType == ControllerCommandType.CREATE_ROUTINE) {
                if (command.CommandStrings.Count() > 1 && command.CommandStrings[0] == "LOOP") {
                    CreateAndRunRoutine(command.CommandStrings.Skip(1).ToArray(), true);
                }
                else {
                    CreateAndRunRoutine(command.CommandStrings.ToArray(), false);
                }
            }
        }
        if (commands.Any()) {
            _sender.SendBatchCommand(commands);
        }
    }
}