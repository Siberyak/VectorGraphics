using System;
using System.Collections.Generic;

namespace Shapes
{
    public class SuspendHelper
    {
        private readonly Stack<KeyValuePair<object, Action<bool>>> _suspends = new Stack<KeyValuePair<object, Action<bool>>>();

        public void Suspend(Action<bool> onResumeAction = null, object tag = null)
        {
            lock (_suspends)
                _suspends.Push(new KeyValuePair<object, Action<bool>>(tag, onResumeAction));
        }

        public void Resume(object tag = null, bool checkTag = false)
        {
            lock (_suspends)
            {
                if (!Suspended)
                    return;

                var item = _suspends.Pop();
                if (checkTag && !Equals(item.Key, tag))
                    throw new Exception(string.Format("не совпали метки. нарушен порядок. имеем [{0}], ждали [{1}]", item.Key, tag));

                if (item.Value != null)
                    item.Value(Suspended);
            }
        }

        public bool Suspended
        {
            get { lock (_suspends) return _suspends.Count != 0; }
        }
    }
}