namespace AutoKeybot.KeyboardModule;

public enum VKCodes {

    // space
    KEY_SPACE = 0x20,

    // Keyboard Modifiers
    KEY_LEFT_CTRL = 0xA2,

    KEY_LEFT_SHIFT = 0xA0,
    KEY_LEFT_ALT = 0xA4,
    KEY_RIGHT_CTRL = 0xA3,
    KEY_RIGHT_SHIFT = 0xA1,
    KEY_RIGHT_ALT = 0xA5,

    // Special keys within the alphanumeric cluster
    KEY_TAB = 0x09,

    KEY_CAPS_LOCK = 0x14,
    KEY_BACKSPACE = 0x08,
    KEY_RETURN = 0x0D,

    // Navigation cluster
    KEY_INSERT = 0x2D,

    KEY_DELETE = 0x2E,
    KEY_HOME = 0x24,
    KEY_END = 0x23,
    KEY_PAGE_UP = 0x21,
    KEY_PAGE_DOWN = 0x22,
    KEY_UP_ARROW = 0x26,
    KEY_DOWN_ARROW = 0x28,
    KEY_LEFT_ARROW = 0x25,
    KEY_RIGHT_ARROW = 0x27,

    // Numeric keypad
    KEY_NUM_LOCK = 0x90,

    KEY_KP_1 = 0x61,
    KEY_KP_2 = 0x62,
    KEY_KP_3 = 0x63,
    KEY_KP_4 = 0x64,
    KEY_KP_5 = 0x65,
    KEY_KP_6 = 0x66,
    KEY_KP_7 = 0x67,
    KEY_KP_8 = 0x68,
    KEY_KP_9 = 0x69,
    KEY_KP_0 = 0x60,

    // Escape and function keys
    KEY_ESC = 0x1B,

    KEY_F1 = 0x70,
    KEY_F2 = 0x71,
    KEY_F3 = 0x72,
    KEY_F4 = 0x73,
    KEY_F5 = 0x74,
    KEY_F6 = 0x75,
    KEY_F7 = 0x76,
    KEY_F8 = 0x77,
    KEY_F9 = 0x78,
    KEY_F10 = 0x79,
    KEY_F11 = 0x7A,
    KEY_F12 = 0x7B,
    KEY_F13 = 0x7C,
    KEY_F14 = 0x7D,
    KEY_F15 = 0x7E,
    KEY_F16 = 0x7F,
    KEY_F17 = 0x80,
    KEY_F18 = 0x81,
    KEY_F19 = 0x82,
    KEY_F20 = 0x83,
    KEY_F21 = 0x84,
    KEY_F22 = 0x85,
    KEY_F23 = 0x86,
    KEY_F24 = 0x87,

    // Function control keys
    KEY_PRINT_SCREEN = 0x2C,

    KEY_SCROLL_LOCK = 0x91,
    KEY_PAUSE = 0x13
}

public static class VKChars {

    public static readonly Dictionary<char, ushort> CharCode = new Dictionary<char, ushort>
{
    {',', 0xBC},
    {'.', 0xBE},
    {'/', 0xBF},
    {'-', 0xBD},
    {'=', 0xBB},
    {'[', 0xDB},
    {']', 0xDD},
    {'\\', 0xDC},
    {';', 0xBA},
    {'\'', 0xDE},
    {'`', 0xC0},
    {'1', 0x31},
    {'2', 0x32},
    {'3', 0x33},
    {'4', 0x34},
    {'5', 0x35},
    {'6', 0x36},
    {'7', 0x37},
    {'8', 0x38},
    {'9', 0x39},
    {'0', 0x30},
    {'q', 0x51},
    {'w', 0x57},
    {'e', 0x45},
    {'r', 0x52},
    {'t', 0x54},
    {'y', 0x59},
    {'u', 0x55},
    {'i', 0x49},
    {'o', 0x4F},
    {'p', 0x50},
    {'a', 0x41},
    {'s', 0x53},
    {'d', 0x44},
    {'f', 0x46},
    {'g', 0x47},
    {'h', 0x48},
    {'j', 0x4A},
    {'k', 0x4B},
    {'l', 0x4C},
    {'z', 0x5A},
    {'x', 0x58},
    {'c', 0x43},
    {'v', 0x56},
    {'b', 0x42},
    {'n', 0x4E},
    {'m', 0x4D}
};
}