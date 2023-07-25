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

    public void SendBatchCommand(IEnumerable<IKeybotCommand> cmds) {
        var inputs = cmds.Select(x => GetInputs(x)).SelectMany(x => x).ToArray();
        SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
    }

    public void SendCommand(IKeybotCommand cmd) {
        var inputs = GetInputs(cmd);
        SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
    }

    private INPUT[] GetInputs(IKeybotCommand cmd) {
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
                        new INPUT
                            {
                                Type = 1,
                                u=new InputUnion{kb = new KEYBDINPUT
                                {
                                    wVk = wVk,
                                    wScan = 0,
                                    dwFlags = 0,
                                    time = 0,
                                    dwExtraInfo = 0
                                }
                                }
                            }
                    },
                    KeyCommandType.KEY_RELEASE => new INPUT[] {
                        new INPUT
                            {
                                Type = 1,
                                u=new InputUnion{
                                kb = new KEYBDINPUT
                                {
                                    wVk = wVk,
                                    wScan = 0,
                                    dwFlags = KEYEVENTF_KEYUP,
                                    time = 0,
                                    dwExtraInfo = 0
                                }
                                }
                            }
                    },
                    KeyCommandType.KEY_PRINT => new INPUT[] {
                         new INPUT
                            {
                                Type = 1,
                                u=new InputUnion{
                                kb = new KEYBDINPUT
                                {
                                    wVk = wVk,
                                    wScan = 0,
                                    dwFlags = 0,
                                    time = 0,
                                    dwExtraInfo = 0
                                }
                                }
                            },
                        new INPUT
                            {
                                Type = 1,
                     u=new InputUnion{
                                kb = new KEYBDINPUT
                                {
                                    wVk = wVk,
                                    wScan = 0,
                                    dwFlags = KEYEVENTF_KEYUP,
                                    time = 0,
                                    dwExtraInfo = 0
                                }
                     }
                            }
                    },
                    _ => throw new InvalidDataException()
                }
                ;
            }
            else if (cmd.CommandType == KeyCommandType.KEY_RELEASE_ALL) {
                var keys1 = ((VKCodes[])Enum.GetValues(typeof(VKCodes))).Select(x => (ushort)x).ToArray();
                var keys2 = VKChars.CharCode.Values.ToArray();
                return keys1.Concat(keys2).Select(x => new INPUT {
                    Type = 1,
                    u = new InputUnion {
                        kb = new KEYBDINPUT {
                            wVk = x,
                            wScan = 0,
                            dwFlags = KEYEVENTF_KEYUP,
                            time = 0,
                            dwExtraInfo = 0
                        }
                    }
                }).ToArray();
            }
            else {
                throw new InvalidDataException();
            }
        }
        catch (Exception e) {
            throw new InvalidDataException($"Exception thrown when parsing input: {cmd.CommandData[0]}", e);
        }
    }
}