namespace AutoKeybot.KeyboardModule;

public enum KeyCommandType {
    KEY_RELEASE = 1,
    KEY_PRESS = 2,
    KEY_PRINT = 3,
    KEY_RELEASE_ALL = 4
}

public interface IKeybotCommand {
    public KeyCommandType CommandType { get; }
    public string[] CommandData { get; set; } // length 2
}

public class KeybotCommand : IKeybotCommand {
    public string[] CommandData { get; set; }
    public KeyCommandType CommandType { get; set; }

    public KeybotCommand(string[] commandData) {
        if (!Enum.TryParse<KeyCommandType>(commandData[0], out var t)) {
            throw new InvalidDataException("Cannot parse Keyboard Command: unsupported command type");
        }
        CommandType = t;
        CommandData = commandData.Skip(1).ToArray();
        // currently only support keyboard command
    }
}