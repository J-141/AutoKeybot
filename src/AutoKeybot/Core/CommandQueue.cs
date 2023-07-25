using AutoKeybot.Display;

namespace AutoKeybot.Core {

    public class CommandQueue {
        private readonly Queue<IControllerCommand> _queue = new Queue<IControllerCommand>();
        private readonly object _lock = new object();

        public void Enqueue(IControllerCommand item) {
            lock (_lock) {
                _queue.Enqueue(item);
            }
        }

        public void EnqueueBatch(IEnumerable<IControllerCommand> items) {
            lock (_lock) {
                foreach (var item in items) {
                    _queue.Enqueue(item);
                }
            }
        }

        public bool TryDequeue(out IControllerCommand item) {
            lock (_lock) {
                if (_queue.Count > 0) {
                    item = _queue.Dequeue();
                    return true;
                }
                else {
                    item = default;
                    return false;
                }
            }
        }

        public void Reset() {
            lock (_lock) {
                _queue.Clear();
            }
        }

        public string GetString() {
            return string.Join(string.Empty, _queue.Select(x => x.CommandType == ControllerCommandType.KEY ? KeyAbbr.Abbr(x.CommandStrings[1]) : "").ToArray());
        }

        public int Count {
            get {
                lock (_lock) {
                    return _queue.Count;
                }
            }
        }
    }
}