using CommandLine;

namespace AutoKeybot.Options;

public class GlobalOptions {

    [Option("root", Required = false, HelpText = "Set root path")]
    public string Root { get; set; } = ".";

    [Option("port", Required = false, HelpText = "Set output port. if configured, will use arduino hardware executor.")]
    public string Port { get; set; } = "";

    [Option("max-time", Required = false, HelpText = "Maximum time to run in mins. 0 => run forever")]
    public int MaxRunTimeInMins { get; set; } = 17;

    [Option("global-int", Required = false, HelpText = "Set global interval")]
    public int GlobalClockInterval { get; set; } = 30;

    [Option("jiggle-int-delta", Required = false, HelpText = "The delta for global interval jiggling")]
    public int GlobalIntervalDelta { get; set; } = 15;

    [Option("display-int", Required = false, HelpText = "Set display refresh interval")]
    public int DisplayInterval { get; set; } = 200;

    [Option("max-queue-length", Required = false, HelpText = "Set max queue length")]
    public int MaxQueueLength { get; set; } = 150;

    [Option("min-queue-length", Required = false, HelpText = "Set min queue length")]
    public int MinQueueLength { get; set; } = 50;

    [Option("display-length", Required = false, HelpText = "Set max displayed queue length")]
    public int MaxDisplayQueueSize { get; set; } = 100;

    [Option("queue", Required = false, HelpText = "Run as queue mode. In this mode the command queue is exposed, and let external program enter the controller command into the queue.")]
    public bool IsQueueMode { get; set; } = false;

    [Option("minPrintTime", Required = false, HelpText = "the minimal time interval between the press and release event of the same key. applies only for software executor")]
    public int? minPrintTime { get; set; } = null;
}