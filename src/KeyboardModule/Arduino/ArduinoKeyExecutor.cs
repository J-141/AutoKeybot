using System.IO.Ports;

namespace AutoKeybot.KeyboardModule;

public class ArduinoKeyExecutor : IDisposable, IKeyboardExecutor {
    public SerialPort _port;

    public ArduinoKeyExecutor(string port) {
        _port = new SerialPort(port);

        _port.BaudRate = 9600; // Set your baud rate
        _port.Parity = Parity.None;
        _port.StopBits = StopBits.One;
        _port.DataBits = 8;
        _port.Handshake = Handshake.None;

        _port.Open();
    }

    public void Dispose() {
        _port.Close();
    }

    public void SendCommand(IKeybotCommand cmd) {
        SendByte(GetBytes(cmd));
    }

    public void SendBatchCommand(IEnumerable<IKeybotCommand> cmds) {
        var bytes = cmds.SelectMany(cmd => GetBytes(cmd)).ToArray();
        if (bytes.Any()) {
            SendByte(bytes);
        }
    }

    private byte[] GetBytes(IKeybotCommand cmd) {
        if (cmd.CommandType == KeyCommandType.KEY_PRESS || cmd.CommandType == KeyCommandType.KEY_PRINT || cmd.CommandType == KeyCommandType.KEY_RELEASE) {
            byte key = 0;
            if (cmd.CommandData[0].Length == 1 && ArduinoCharKeys.AcceptKeys.Contains(cmd.CommandData[0][0])) {
                key = (byte)cmd.CommandData[0][0];
            }
            else if (Enum.TryParse<ArduinoControlKeys>(cmd.CommandData[0], out var controlKey)) {
                key = (byte)controlKey;
            }
            else {
                throw new InvalidDataException("Cannot parse Keyboard Command: invalid key data.");
            }
            return new byte[] { (byte)cmd.CommandType, key, (byte)0 };
        }
        else if (cmd.CommandType == KeyCommandType.KEY_RELEASE_ALL) {
            return new byte[] { (byte)cmd.CommandType, 0, 0 };
        }
        else {
            throw new InvalidDataException("Cannot parse Keyboard Command: unsupported command type"); //TODO
        }
    }

    private void SendByte(byte[] data) {
        _port.Write(data, 0, data.Length);
    }
}