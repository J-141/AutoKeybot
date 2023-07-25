namespace AutoKeybot.Core;

public class Action {
    // Action is a sequence of ControllerCommand that must be executed continuously
    // no other command can be inserted between 2 continuous commands in the [Commands] of an Action.
    // can be created from text file.

    public IEnumerable<IControllerCommand> Commands;

    public Action(string filePath) : this(File.ReadAllLines(filePath)) {
    }

    public Action(IEnumerable<string> lines) {
        var cmdList = new List<IControllerCommand>();
        foreach (string line in lines) {
            try {
                var trimmed = line.Trim();

                if (trimmed.Length > 0) {
                    var words = trimmed.Split();
                    if (words[0] == "ACTION") {
                        cmdList.AddRange(ScriptManager.GetAction(words[1]).Commands);
                    }
                    else {
                        cmdList.Add(new ControllerCommand(trimmed));
                    }
                }
            }
            catch (Exception e) {
                throw new InvalidDataException($"Exception thrown when parsing Action line: {line}", e);
            }
        }
        Commands = cmdList;
    }

    public void Enqueue(CommandQueue queue) {
        queue.EnqueueBatch(Commands);
    }
}