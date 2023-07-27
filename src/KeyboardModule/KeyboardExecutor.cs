using System.Runtime.InteropServices;

namespace AutoKeybot.KeyboardModule;

public class KeyboardExecutor : IKeyboardExecutor {

    [DllImport("user32.dll")]
    private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

    [StructLayout(LayoutKind.Sequential)]
    private struct INPUT {
        public uint Type;
        public InputUnion u;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct InputUnion {
        [FieldOffset(0)] public MOUSEINPUT mi;
        [FieldOffset(0)] public KEYBDINPUT kb;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MOUSEINPUT {
        public int dx;
        public int dy;
        public uint mouseData;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    private const int KEYEVENTF_EXTENDEDKEY = 0x0001;
    private const int KEYEVENTF_KEYUP = 0x0002;

    [StructLayout(LayoutKind.Sequential)]
    private struct KEYBDINPUT {
        public ushort wVk;
        public ushort wScan;
        public uint dwFlags;
        public long time;
        public uint dwExtraInfo;
    }

    private readonly int? _minPrintTime;
    private Dictionary<Guid, Timer> _timers = new();

    /// <summary>
    /// minPrintTime: if it is not null, for KEY_PRINT events, will send the release event after a period of time.
    /// </summary>
    /// <param name="minPrintTime"></param>
    public KeyboardExecutor(int? minPrintTime = null) {
        _minPrintTime = minPrintTime;
    }

    public void SendBatchCommand(IEnumerable<IKeybotCommand> cmds) {
        var firstInputs = cmds.Select(x => GetFirstInputs(x)).SelectMany(x => x).ToArray();
        var delayInputs = cmds.Select(x => GetDelayedInputs(x)).SelectMany(x => x).ToArray();
        SendInputs(firstInputs, delayInputs);
    }

    public void SendCommand(IKeybotCommand cmd) {
        var firstInputs = GetFirstInputs(cmd);
        var delayInputs = GetDelayedInputs(cmd);
        SendInputs(firstInputs, delayInputs);
    }

    private void SendInputs(INPUT[] first, INPUT[] delayed) {
        if (_minPrintTime.HasValue) {
            SendInput((uint)first.Length, first, Marshal.SizeOf(typeof(INPUT)));
            var id = Guid.NewGuid();
            _timers[id] = new Timer(
                s => {
                    SendInput((uint)delayed.Length, delayed, Marshal.SizeOf(typeof(INPUT)));
                    _timers[id].Dispose();
                }
                , null
                , _minPrintTime.Value
                , Timeout.Infinite);
        }
        else {
            var inputs = first.Concat(delayed).ToArray();
            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }
    }

    private INPUT GetKeyRelease(ushort wVk) {
        return new INPUT {
            Type = 1,
            u = new InputUnion {
                kb = new KEYBDINPUT {
                    wVk = wVk,
                    wScan = 0,
                    dwFlags = KEYEVENTF_KEYUP,
                    time = 0,
                    dwExtraInfo = 0
                }
            }
        };
    }

    private INPUT GetKeyPress(ushort wVk) {
        return new INPUT {
            Type = 1,
            u = new InputUnion {
                kb = new KEYBDINPUT {
                    wVk = wVk,
                    wScan = 0,
                    dwFlags = 0,
                    time = 0,
                    dwExtraInfo = 0
                }
            }
        };
    }

    private INPUT[] GetFirstInputs(IKeybotCommand cmd) {
        try {
            if (cmd.CommandType == KeyCommandType.KEY_PRESS || cmd.CommandType == KeyCommandType.KEY_PRINT || cmd.CommandType == KeyCommandType.KEY_RELEASE) {
                ushort wVk;
                if (cmd.CommandData[0].Length > 1) {
                    Enum.TryParse<VKCodes>(cmd.CommandData[0], out var e);
                    wVk = (ushort)e;
                }
                else {
                    wVk = VKChars.CharCode[cmd.CommandData[0][0]];
                }
                return cmd.CommandType switch {
                    KeyCommandType.KEY_PRESS => new INPUT[] {
                        GetKeyPress(wVk)
                    },
                    KeyCommandType.KEY_RELEASE => new INPUT[] {
                        GetKeyRelease(wVk)
                    },
                    KeyCommandType.KEY_PRINT => new INPUT[] {
                        GetKeyPress(wVk)
                    },
                    _ => throw new InvalidDataException()
                };
                ;
            }
            else if (cmd.CommandType == KeyCommandType.KEY_RELEASE_ALL) {
                var keys1 = ((VKCodes[])Enum.GetValues(typeof(VKCodes))).Select(x => (ushort)x).ToArray();
                var keys2 = VKChars.CharCode.Values.ToArray();
                return keys1.Concat(keys2).Select(x => GetKeyRelease(x)).ToArray();
            }
            else {
                throw new InvalidDataException();
            }
        }
        catch (Exception e) {
            throw new InvalidDataException($"Exception thrown when parsing input: {cmd.CommandData[0]}", e);
        }
    }

    private INPUT[] GetDelayedInputs(IKeybotCommand cmd) {
        try {
            if (cmd.CommandType == KeyCommandType.KEY_PRINT) {
                ushort wVk;
                if (cmd.CommandData[0].Length > 1) {
                    Enum.TryParse<VKCodes>(cmd.CommandData[0], out var e);
                    wVk = (ushort)e;
                }
                else {
                    wVk = VKChars.CharCode[cmd.CommandData[0][0]];
                }
                return cmd.CommandType switch {
                    KeyCommandType.KEY_PRINT => new INPUT[] {
                        GetKeyRelease(wVk)
                    },
                    _ => throw new InvalidDataException()
                };
            }
            else {
                return new INPUT[] { };
            }
        }
        catch (Exception e) {
            throw new InvalidDataException($"Exception thrown when parsing input: {cmd.CommandData[0]}", e);
        }
    }
}