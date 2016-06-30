using System;
using System.Collections.Generic;
using System.Linq;

namespace Shapes
{
    public class ActionsRegistrator
    {
        private IViewPortInputAction _activeAction;
        private readonly List<IViewPortInputAction> _actions = new List<IViewPortInputAction>();

        public IViewPortInputAction ActiveAction
        {
            get { return _activeAction; }
        }

        public IViewPortInputAction Register(IViewPortInputAction action)
        {
            _actions.Add(action);
            action.Tag("Owner", this.Tag("Owner"));
            return action;
        }

        public IViewPortInputAction OnButtonDown(Func<IInputInfo, bool> activateCondition, ViewPortEventType deactivateEventTypes = ViewPortEventType.None, Func<IInputInfo, bool> deactivateCondition = null, Func<IInputInfo, bool> activate = null, Func<IInputInfo, bool> deactivate = null,
            Func<IInputInfo, bool> process = null)
        {
            return Register(new ViewPortInputAction(ViewPortEventType.ButtonDown, deactivateEventTypes, activateCondition, deactivateCondition, activate, deactivate, process));
        }

        public IViewPortInputAction OnButtonUp(Func<IInputInfo, bool> activateCondition, ViewPortEventType deactivateEventTypes = ViewPortEventType.None, Func<IInputInfo, bool> deactivateCondition = null, Func<IInputInfo, bool> activate = null, Func<IInputInfo, bool> deactivate = null,
            Func<IInputInfo, bool> process = null)
        {
            return Register(new ViewPortInputAction(ViewPortEventType.ButtonUp, deactivateEventTypes, activateCondition, deactivateCondition, activate, deactivate, process));
        }

        public IViewPortInputAction OnMouseMove(Func<IInputInfo, bool> activateCondition, ViewPortEventType deactivateEventTypes = ViewPortEventType.None, Func<IInputInfo, bool> deactivateCondition = null, Func<IInputInfo, bool> activate = null, Func<IInputInfo, bool> deactivate = null,
            Func<IInputInfo, bool> process = null)
        {
            return Register(new ViewPortInputAction(ViewPortEventType.MouseMove, deactivateEventTypes, activateCondition, deactivateCondition, activate, deactivate, process));
        }

        public IViewPortInputAction OnMouseWheel(Func<IInputInfo, bool> activateCondition, ViewPortEventType deactivateEventTypes = ViewPortEventType.None, Func<IInputInfo, bool> deactivateCondition = null, Func<IInputInfo, bool> activate = null, Func<IInputInfo, bool> deactivate = null,
            Func<IInputInfo, bool> process = null)
        {
            return Register(new ViewPortInputAction(ViewPortEventType.MouseWheel, deactivateEventTypes, activateCondition, deactivateCondition, activate, deactivate, process));
        }

        public IViewPortInputAction OnKeyDown(Func<IInputInfo, bool> activateCondition, ViewPortEventType deactivateEventTypes = ViewPortEventType.None, Func<IInputInfo, bool> deactivateCondition = null, Func<IInputInfo, bool> activate = null, Func<IInputInfo, bool> deactivate = null,
            Func<IInputInfo, bool> process = null)
        {
            return Register(new ViewPortInputAction(ViewPortEventType.KeyDown, deactivateEventTypes, activateCondition, deactivateCondition, activate, deactivate, process));
        }

        public IViewPortInputAction OnKeyUp(Func<IInputInfo, bool> activateCondition, ViewPortEventType deactivateEventTypes = ViewPortEventType.None, Func<IInputInfo, bool> deactivateCondition = null, Func<IInputInfo, bool> activate = null, Func<IInputInfo, bool> deactivate = null,
            Func<IInputInfo, bool> process = null)
        {
            return Register(new ViewPortInputAction(ViewPortEventType.KeyUp, deactivateEventTypes, activateCondition, deactivateCondition, activate, deactivate, process));
        }

        public IViewPortInputAction OnKeysRepeated(Func<IInputInfo, bool> activateCondition, ViewPortEventType deactivateEventTypes = ViewPortEventType.None, Func<IInputInfo, bool> deactivateCondition = null, Func<IInputInfo, bool> activate = null, Func<IInputInfo, bool> deactivate = null,
            Func<IInputInfo, bool> process = null)
        {
            return Register(new ViewPortInputAction(ViewPortEventType.KeysRepeated, deactivateEventTypes, activateCondition, deactivateCondition, activate, deactivate, process));
        }
        

        internal bool ProcessRegistered(IInputInfo info)
        {
            if (_activeAction == null)
            {
                _activeAction = _actions.FirstOrDefault(x => x.Activate(info));
                return _activeAction != null && ProcessRegistered(info);
            }

            if (_activeAction.Deactivate(info))
            {
                info.Completed = false;
                _activeAction = null;
            }
            else if (_activeAction.Process(info))
            {
                return !info.Completed || ProcessRegistered(info);
            }

            return true;
        }
    }
}