using CommandLine;

namespace AutoKeybot.Options;

public class GlobalOptions {

    [Option("root", Required = false, HelpText = "Set root path")]
    public string Root { get; set; } = ".";

    [Option("port", Required = false, HelpText = "Set output port")]  // if configured, will use arduino.
    public string Port { get; set; } = "";

    [Option("int", Required = false, HelpText = "Set global interval")]
    public int GlobalClockInterval { get; set; } = 50;

    [Option("int", Required = false, HelpText = "Set display refresh interval")]
    public int DisplayInterval { get; set; } = 200;

    [Option("max-queue-length", Required = false, HelpText = "Set max queue length")]
    public int MaxQueueLength { get; set; } = 500;

    [Option("min=queue-length", Required = false, HelpText = "Set min queue length")]
    public int MinQueueLength { get; set; } = 50;

    [Option("display-length", Required = false, HelpText = "Set max displayed queue length")]
    public int MaxDisplayQueueSize { get; set; } = 100;

    // queue mode: expose the command queue, and let external program enter the controller command into the queue.
    [Option("queue", Required = false, HelpText = "queue mode")]
    public bool IsQueueMode { get; set; } = false;
}