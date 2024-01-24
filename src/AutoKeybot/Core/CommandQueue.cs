using AutoKeybot.Display;
using System.Windows.Input;

namespace AutoKeybot.Core {

    public class CommandQueue {
        public readonly LinkedList<IControllerCommand> _queue = new LinkedList<IControllerCommand>();
        private readonly object _lock = new object();

        public void Enqueue(IControllerCommand item) {
            lock (_lock) {
                _queue.AddFirst(item.Copy());
            }
        }

        public void Insert(IControllerCommand item) {
            lock (_lock) {
                _queue.AddLast(item.Copy());
            }
        }

        public void InsertBatch(IEnumerable<IControllerCommand> items) {
            lock (_lock) {
                foreach (var item in items.Reverse()) {
                    _queue.AddLast(item.Copy());
                }
            }
        }

        public void EnqueueBatch(IEnumerable<IControllerCommand> items) {
            lock (_lock) {
                foreach (var item in items) {
                    _queue.AddFirst(item.Copy());
                }
            }
        }

        public IControllerCommand? PeekLast() {
            if (_queue.Count > 0) {
                return _queue.Last();
            }
            return null;
        }

        public bool TryDequeue(out IControllerCommand item) {
            lock (_lock) {
                if (_queue.Count > 0) {
                    item = _queue.Last();
                    _queue.RemoveLast();
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

        public int Count {
            get {
                lock (_lock) {
                    return _queue.Count;
                }
            }
        }
    }
}