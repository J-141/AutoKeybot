namespace AutoKeybot.KeyboardModule;

public interface IKeyboardExecutor {

    public void SendCommand(IKeybotCommand cmd);

    public void SendBatchCommand(IEnumerable<IKeybotCommand> cmds);
}