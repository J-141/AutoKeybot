using AutoKeybot.KeyboardModule;

namespace UnitTest {

    public class Tests {

        [Test]
        public void Test1() {
            var sender = new ArduinoKeyExecutor("COM5");
            var cmd = new KeybotCommand(new string[] { "KEY_PRINT", "H" });
            sender.SendCommand(cmd);
        }
    }
}