using AutoKeybot.Core;
using AutoKeybot.Display;
using AutoKeybot.KeyboardModule;
using AutoKeybot.Options;
using Timer = System.Timers.Timer;

namespace AutoKeybot.Schedulers;

internal class Controller {
    private Timer _timer { get; set; }
    private Timer _beeper { get; set; }
    private Timer _stopTimer { get; set; }
    private bool _running { get; set; } = false;
    private DateTime _startTime { get; set; }
    public int GlobalClockInterval;
    public CommandQueue Queue { get; set; } //null command means skip one clock interval
    private int maxQueueLength { get; set; }
    private int minQueueLength { get; set; }
    private GlobalOptions _options { get; set; }
    private IKeyboardExecutor _sender { get; set; }
    private DisplayManager _displayManager;
    private bool addingNewCommands { get; set; } = true;
    public Dictionary<string, (Routine r, bool loop)> Routines { get; set; } = new Dictionary<string, (Routine r, bool loop)>();

    public Controller(GlobalOptions options, IKeyboardExecutor sender, DisplayManager display) {
        _options = options;
        maxQueueLength = options.MaxQueueLength;
        minQueueLength = options.MinQueueLength;
        Queue = new CommandQueue();
        _sender = sender;
        GlobalClockInterval = options.GlobalClockInterval;
        _displayManager = display;
        display.Routines = Routines;
        _timer = new Timer(GetNextInterval());
        _timer.Elapsed += (s, o) => Execute();
        _timer.Elapsed += (s, o) => ResetTimer();
        _beeper = new Timer(500);
        _beeper.Elapsed += (s, o) => { Console.Beep(); };
        _stopTimer = new Timer();
        _stopTimer.Elapsed += (s, o) => { Stop(); StartBeep(); };
    }

    private void StartBeep() {
        _beeper.Start();
    }

    private void StopBeep() {
        _beeper.Stop();
    }

    private void ResetTimer() {
        if (_running) {
            _timer.Interval = GetNextInterval();
            _timer.Start();
        }
    }

    public void SetStopCountdown() {
        if (_options.MaxRunTimeInMins != 0) {
            _displayManager.SetCountdown(TimeSpan.FromMilliseconds(_stopTimer.Interval - (DateTime.Now - _startTime).TotalMilliseconds));
        }
    }

    private int GetNextInterval() {
        return new Random().Next(_options.GlobalClockInterval - _options.GlobalIntervalDelta, _options.GlobalClockInterval + _options.GlobalIntervalDelta);
    }

    public void Run() {
        _timer.Start();
        _running = true;
        _startTime = DateTime.Now;
        StopBeep();
        foreach (var (r, l) in Routines.Values) {
            r.Start(Queue, l);
        };
        if (_options.MaxRunTimeInMins != 0) {
            _stopTimer.Interval = TimeSpan.FromMinutes(_options.MaxRunTimeInMins).TotalMilliseconds;
            _stopTimer.Start();
        }
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

    public void ExecAction(string identifier) {
        var action = ScriptManager.GetAction(identifier);
        action.InsertInto(Queue);
    }

    public void CreateAndExecAction(string[] words) {
        var action = ScriptManager.GetActionFromTemplate(words[0], words.Skip(1).ToArray());
        action.InsertInto(Queue);
    }

    public void CreateRoutine(string[] words, bool loop = false) {
        var identifier = string.Join("_", words);
        var routine = ScriptManager.GetRoutineFromTemplate(words[0], words.Skip(1).ToArray());
        routine.FinalAction = () => { if (Routines.ContainsKey(identifier)) Routines.Remove(identifier); };
        Routines[identifier] = (ScriptManager.GetRoutine(identifier), loop);
    }

    public void Stop() {
        _running = false;
        _timer.Stop();
        foreach (var (r, l) in Routines.Values) {
            r.Stop();
        };
        _sender.SendCommand(new KeybotCommand(new string[] { "KEY_RELEASE_ALL" }));
    }

    public void PauseRoutine(string identifier) {
        if (Routines.ContainsKey(identifier))
            Routines[identifier].r.Stop();
    }

    public void ResumeRoutine(string identifier) {
        if (Routines.ContainsKey(identifier))
            Routines[identifier].r.Start(this.Queue, Routines[identifier].loop);
    }

    public void EnqueueCommand(ControllerCommand cmd) {
        if (Queue.Count > maxQueueLength) {
            throw new InvalidOperationException("Command queue too long; please check your application and don't push too many SKIPs.");
        }
        else {
            Queue.Enqueue(cmd);
        }
    }

    public void InsertCommand(ControllerCommand cmd) {
        if (Queue.Count > maxQueueLength) {
            throw new InvalidOperationException("Command queue too long; please check your application and don't push too many SKIPs.");
        }
        else {
            Queue.Insert(cmd);
        }
    }

    public void Reset() {
        _timer.Stop();
        _running = false;
        Queue.Reset();
        foreach (var (r, l) in Routines.Values) {
            r.Reset();
        };
        Routines.Clear();
        ScriptManager.Reset();
        _sender.SendCommand(new KeybotCommand(new string[] { "KEY_RELEASE_ALL" }));
    }

    private void Execute() {
        SetStopCountdown();
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

        while (Queue.PeekLast() != null) {
            var command = Queue.PeekLast()!;
            if (command.CommandType == ControllerCommandType.SKIP) {
                var new_skip = int.Parse(command.CommandStrings[0]) - GlobalClockInterval;
                _displayManager.EnqueueCommand(" ");
                if (new_skip > 0) {
                    command.CommandStrings[0] = new_skip.ToString();
                }
                else {
                    Queue.TryDequeue(out var _);
                }
                break;
            }

            Queue.TryDequeue(out command);

            if (command.CommandType == ControllerCommandType.KEY) {
                var cmd = new KeybotCommand(command.CommandStrings);
                commands.Add(cmd);
                if (command.CommandStrings.Length > 1)
                    _displayManager.EnqueueCommand(KeyAbbr.Abbr(command.CommandStrings[1]));
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
            else if (command.CommandType == ControllerCommandType.PAUSE_ROUTINE) {
                PauseRoutine(command.CommandStrings[0]);
            }
            else if (command.CommandType == ControllerCommandType.RESUME_ROUTINE) {
                ResumeRoutine(command.CommandStrings[0]);
            }
            else if (command.CommandType == ControllerCommandType.EXEC_ACTION) {
                ExecAction(command.CommandStrings[0]);
                Execute();
            }
            else if (command.CommandType == ControllerCommandType.CREATE_ACTION) {
                CreateAndExecAction(command.CommandStrings.ToArray());
            }
            else if (command.CommandType == ControllerCommandType.CREATE_ROUTINE) {
                if (command.CommandStrings.Count() > 1 && command.CommandStrings[0] == "LOOP") {
                    CreateAndRunRoutine(command.CommandStrings.Skip(1).ToArray(), true);
                }
                else {
                    CreateAndRunRoutine(command.CommandStrings.ToArray(), false);
                }
            }
            else if (command.CommandType == ControllerCommandType.RESET) {
                Reset();
            }
            else if (command.CommandType == ControllerCommandType.RESTART) {
                Reset();
                Run();
            }
            else if (command.CommandType == ControllerCommandType.RANDOM) {
                var CommandToGo = command.SubCommands!.OrderBy(x => Guid.NewGuid()).First();
                Queue.Insert(CommandToGo);
                Execute();
            }
            else if (command.CommandType == ControllerCommandType.SEQUENTIAL) {
                var CommandToGo = command.SubCommands!.ElementAt(((SequentialControllerCommand)command).Index);
                ((SequentialControllerCommand)command).Increment();
                Queue.Insert(CommandToGo);
                Execute();
            }
        }
        if (commands.Any()) {
            _sender.SendBatchCommand(commands);
        }
    }
}