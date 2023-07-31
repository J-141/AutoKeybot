using AutoKeybot.Schedulers;
using AutoKeybot.Templates;

namespace AutoKeybot;

public static class ScriptManager {
    public static string Root { get; set; }

    // this is to manage the scripts (to define actions/routines/metaroutines).
    private static readonly Dictionary<string, Routine> _routines = new Dictionary<string, Routine>();

    private static readonly Dictionary<string, Core.Action> _actions = new Dictionary<string, Core.Action>();

    private static readonly Dictionary<string, RoutineTemplate> _routineTemplates = new Dictionary<string, RoutineTemplate>();
    private static readonly Dictionary<string, ActionTemplate> _actionTemplates = new Dictionary<string, ActionTemplate>();

    public static Routine GetRoutine(string identifier) {
        if (!_routines.TryGetValue(identifier, out var routine)) {
            string filePath = Path.Combine(Root, identifier.Replace(".", "/") + ".routine");
            routine = new Routine();
            routine.ConstructFromFile(filePath);
            _routines[identifier] = routine;
        }
        return routine;
    }

    public static Routine GetRoutineFromTemplate(string identifier, string[] args) {
        var id = identifier + "_" + string.Join("_", args);
        if (!_routines.TryGetValue(identifier, out var routine)) {
            if (!_routineTemplates.TryGetValue(identifier, out var template)) {
                string filePath = Path.Combine(Root, identifier.Replace(".", "/") + ".routine.template");
                template = new RoutineTemplate(filePath);
                _routineTemplates[identifier] = template;
            }
            routine = _routineTemplates[identifier].CreateRoutine(args);
            _routines[id] = routine;
        }
        return routine;
    }

    public static Core.Action GetActionFromTemplate(string identifier, string[] args) {
        var id = identifier + "_" + string.Join("_", args);
        if (!_actions.TryGetValue(identifier, out var action)) {
            if (!_actionTemplates.TryGetValue(identifier, out var template)) {
                string filePath = Path.Combine(Root, identifier.Replace(".", "/") + ".action.template");
                template = new ActionTemplate(filePath);
                _actionTemplates[identifier] = template;
            }
            action = _actionTemplates[identifier].CreateAction(args);
            _actions[id] = action;
        }
        return action;
    }

    public static void Reset() {
        _routines.Clear();
        _actions.Clear();
        _actionTemplates.Clear();
        _routineTemplates.Clear();
    }

    public static Core.Action GetAction(string identifier) {
        if (!_actions.TryGetValue(identifier, out var action)) {
            string filePath = Path.Combine(Root, identifier.Replace(".", "/") + ".action");
            action = new Core.Action(filePath);
            _actions[identifier] = action;
        }
        return action;
    }
}