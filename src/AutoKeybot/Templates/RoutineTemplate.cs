namespace AutoKeybot.Templates {

    internal class ActionTemplate {
        public Dictionary<string, string[]> SubRules = new Dictionary<string, string[]>();

        public ActionTemplate(string filePath) : this(File.ReadAllLines(filePath)) {
        }

        public ActionTemplate(string[] lines) {
            int index = 0;
            var strArray = lines.Select(l => l.Trim()).Where(l => l.Length > 0).ToArray();
            while (index < strArray.Length) {
                try {
                    ParseAlias(ref index, strArray);
                }
                catch (Exception ex) {
                    throw new InvalidDataException($"Exception thrown when parsing alias: {strArray[index]}", ex);
                }
            }
        }

        private void ParseAlias(ref int index, string[] strArray) { // parse whole alias group
            var words = strArray[index].Trim().Split();
            if (words.Length == 3 && words[0] == "ALIAS" && words[2] == "[") {
                var key = words[1];
                index += 1;
                words = strArray[index].Trim().Split();
                var strs = new List<string>();
                while (words[0] != "]") {
                    strs.Add(strArray[index]);
                    index += 1;
                    words = strArray[index].Trim().Split();
                }
                SubRules[key] = strs.ToArray();
            }
            else {
                throw new InvalidDataException($"Cannot parse alias in routine template: {strArray[index]}");
            }
            index += 1;
        }

        public Core.Action CreateAction(string[] words) {
            var strs = new List<string>();
            foreach (var w in words) {
                if (SubRules.TryGetValue(w, out var cmds)) {
                    strs.AddRange(cmds);
                }
                else {
                    throw new InvalidDataException($"Unknown word '{w}': no substitution rule found.");
                }
            }
            var action = new Core.Action(strs);
            return action;
        }
    }
}