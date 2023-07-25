using AutoKeybot.KeyboardModule;

namespace AutoKeybot.Display;

public static class KeyAbbr {

    public static Dictionary<ArduinoControlKeys, string> KeyAbbreviations = new Dictionary<ArduinoControlKeys, string>
    {
    { ArduinoControlKeys.KEY_SPACE, "SP" },
    { ArduinoControlKeys.KEY_LEFT_CTRL, "LC" },
    { ArduinoControlKeys.KEY_LEFT_SHIFT, "LS" },
    { ArduinoControlKeys.KEY_LEFT_ALT, "LA" },
    { ArduinoControlKeys.KEY_LEFT_GUI, "LG" },
    { ArduinoControlKeys.KEY_RIGHT_CTRL, "RC" },
    { ArduinoControlKeys.KEY_RIGHT_SHIFT, "RS" },
    { ArduinoControlKeys.KEY_RIGHT_ALT, "RA" },
    { ArduinoControlKeys.KEY_RIGHT_GUI, "RG" },
    { ArduinoControlKeys.KEY_TAB, "Tb" },
    { ArduinoControlKeys.KEY_CAPS_LOCK, "CL" },
    { ArduinoControlKeys.KEY_BACKSPACE, "Bs" },
    { ArduinoControlKeys.KEY_RETURN, "⏎" },
    { ArduinoControlKeys.KEY_MENU, "Mn" },
    { ArduinoControlKeys.KEY_INSERT, "In" },
    { ArduinoControlKeys.KEY_DELETE, "Dl" },
    { ArduinoControlKeys.KEY_HOME, "Hm" },
    { ArduinoControlKeys.KEY_END, "En" },
    { ArduinoControlKeys.KEY_PAGE_UP, "PU" },
    { ArduinoControlKeys.KEY_PAGE_DOWN, "PD" },
    { ArduinoControlKeys.KEY_UP_ARROW, "↑" },
    { ArduinoControlKeys.KEY_DOWN_ARROW, "↓" },
    { ArduinoControlKeys.KEY_LEFT_ARROW, "←" },
    { ArduinoControlKeys.KEY_RIGHT_ARROW, "→" },
    { ArduinoControlKeys.KEY_NUM_LOCK, "NL" },
    { ArduinoControlKeys.KEY_KP_SLASH, "K/" },
    { ArduinoControlKeys.KEY_KP_ASTERISK, "K*" },
    { ArduinoControlKeys.KEY_KP_MINUS, "K-" },
    { ArduinoControlKeys.KEY_KP_PLUS, "K+" },
    { ArduinoControlKeys.KEY_KP_ENTER, "KE" },
    { ArduinoControlKeys.KEY_KP_1, "K1" },
    { ArduinoControlKeys.KEY_KP_2, "K2" },
    { ArduinoControlKeys.KEY_KP_3, "K3" },
    { ArduinoControlKeys.KEY_KP_4, "K4" },
    { ArduinoControlKeys.KEY_KP_5, "K5" },
    { ArduinoControlKeys.KEY_KP_6, "K6" },
    { ArduinoControlKeys.KEY_KP_7, "K7" },
    { ArduinoControlKeys.KEY_KP_8, "K8" },
    { ArduinoControlKeys.KEY_KP_9, "K9" },
    { ArduinoControlKeys.KEY_KP_0, "K0" },
    { ArduinoControlKeys.KEY_KP_DOT, "K." },
    { ArduinoControlKeys.KEY_ESC, "Es" },
    { ArduinoControlKeys.KEY_F1, "F1" },
    { ArduinoControlKeys.KEY_F2, "F2" },
    { ArduinoControlKeys.KEY_F3, "F3" },
    { ArduinoControlKeys.KEY_F4, "F4" },
    { ArduinoControlKeys.KEY_F5, "F5" },
    { ArduinoControlKeys.KEY_F6, "F6" },
    { ArduinoControlKeys.KEY_F7, "F7" },
    { ArduinoControlKeys.KEY_F8, "F8" },
    { ArduinoControlKeys.KEY_F9, "F9" },
    { ArduinoControlKeys.KEY_F10, "F10" },
    { ArduinoControlKeys.KEY_F11, "F11" },
    { ArduinoControlKeys.KEY_F12, "F12" },
    { ArduinoControlKeys.KEY_F13, "F13" },
    { ArduinoControlKeys.KEY_F14, "F14" },
    { ArduinoControlKeys.KEY_F15, "F15" },
    { ArduinoControlKeys.KEY_F16, "F16" },
    { ArduinoControlKeys.KEY_F17, "F17" },
    { ArduinoControlKeys.KEY_F18, "F18" },
    { ArduinoControlKeys.KEY_F19, "F19" },
    { ArduinoControlKeys.KEY_F20, "F20" },
    { ArduinoControlKeys.KEY_F21, "F21" },
    { ArduinoControlKeys.KEY_F22, "F22" },
    { ArduinoControlKeys.KEY_F23, "F23" },
    { ArduinoControlKeys.KEY_F24, "F24" },
    { ArduinoControlKeys.KEY_PRINT_SCREEN, "PS" },
    { ArduinoControlKeys.KEY_SCROLL_LOCK, "SL" },
    { ArduinoControlKeys.KEY_PAUSE, "Pa" }
};

    public static string Abbr(string key) {
        if (key.Length > 1) {
            Enum.TryParse<ArduinoControlKeys>(key, out var str);
            return KeyAbbr.KeyAbbreviations[str];
        }
        else {
            return (key);
        }
    }
}