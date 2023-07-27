using AutoKeybot;
using AutoKeybot.Core;
using AutoKeybot.Display;
using AutoKeybot.KeyboardModule;
using AutoKeybot.Options;
using AutoKeybot.Schedulers;
using CommandLine;
using System.Timers;
using Timer = System.Timers.Timer;

internal class Program {

    private enum Mode {
        Input,
        Display,
        Queue
    }

    private static Mode mode = Mode.Input;
    private static Timer timer;
    private static DisplayManager displayManager;
    private static Controller controller;

    private static void Main(string[] args) {
        var option = Parser.Default.ParseArguments<GlobalOptions>(args).Value;
        IKeyboardExecutor sender;
        if (option.Port != "") {
            sender = new ArduinoKeyExecutor(option.Port);
        }
        else {
            sender = new KeyboardExecutor(option.minPrintTime);
        }
        ScriptManager.Root = option.Root;
        displayManager = new DisplayManager(option);
        controller = new Controller(option, sender, displayManager);

        if (option.IsQueueMode) {
            while (true) {
                try {
                    if (Console.KeyAvailable) {
                        // Process the input
                        string? command = Console.ReadLine();
                        EnterQueue(command);
                    }
                }
                catch (Exception e) {
                    Console.Write(e.ToString());
                }
            }
        }
        else {
            displayManager.SetComingQueue(controller.Queue);
            timer = new Timer(option.DisplayInterval); // Set the refresh interval
            timer.Elapsed += TimerElapsed;
            displayManager.Refresh();

            while (true) {
                try {
                    if (Console.KeyAvailable) {
                        if (mode == Mode.Input) {
                            // Process the input
                            string? command = Console.ReadLine();
                            ExecuteCommand(command);
                        }
                        else {
                            mode = Mode.Input;
                            timer.Stop();
                            controller.Stop();
                            displayManager.Refresh();
                        }
                    }
                }
                catch (Exception e) {
                    timer.Stop();
                    Console.Write(e.ToString());
                }
            }
        }
    }

    private static void TimerElapsed(object sender, ElapsedEventArgs e) {
        if (mode == Mode.Display) {
            displayManager.Refresh();
        }
    }

    private static void EnterQueue(string? command) {
        if (string.IsNullOrEmpty(command))
            return;
        var cmd = new ControllerCommand(command.Trim());
        controller.EnqueueCommand(cmd);
    }

    private static void ExecuteCommand(string? command) {
        if (command == null)
            return;
        var words = command.Split();
        if (words[0] == "run") {
            Thread.Sleep(3000);
            controller.Run();
            timer.Start();
            mode = Mode.Display;
        }
        else if (words[0] == "reset") {
            controller.Reset();
        }
        else if (words[0] == "add") {
            if (words[1] == "loop") {
                controller.AddRoutine(words[2], true);
            }
            else {
                controller.AddRoutine(words[1], false);
            }
        }
        else if (words[0] == "remove") {
            controller.RemoveRoutine(words[1]);
        }
        else if (words[0] == "create") {
            if (words[1] == "loop") {
                controller.CreateRoutine(words.Skip(2).ToArray(), true);
            }
            else {
                controller.CreateRoutine(words.Skip(1).ToArray());
            }
        }
        displayManager.Refresh();
    }
}