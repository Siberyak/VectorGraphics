using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Shapes
{
    class ViewPortInputAction : IViewPortInputAction
    {
        private readonly ViewPortEventType _activateEventType;
        private readonly ViewPortEventType _deactivateEventType;
        private readonly Func<IInputInfo, bool> _activateCondition;
        private readonly Func<IInputInfo, bool> _deactivateCondition;

        private readonly Func<IInputInfo, bool> _activate;
        private readonly Func<IInputInfo, bool> _deactivate;
        private readonly Func<IInputInfo, bool> _process;
        private static bool _show = true;

        public ViewPortInputAction(Func<IInputInfo, bool> activateCondition, Func<IInputInfo, bool> deactivateCondition = null, Func<IInputInfo, bool> activate = null, Func<IInputInfo, bool> deactivate = null, Func<IInputInfo, bool> process = null)
            : this(0, ViewPortEventType.None, null, activateCondition, deactivateCondition, activate, deactivate, process)
        { }

        public ViewPortInputAction(ViewPortEventType activateEventType, Func<IInputInfo, bool> activateCondition, Func<IInputInfo, bool> deactivateCondition = null, Func<IInputInfo, bool> activate = null, Func<IInputInfo, bool> deactivate = null, Func<IInputInfo, bool> process = null)
            : this(0, activateEventType, null, activateCondition, deactivateCondition, activate, deactivate, process)
        { }
        public ViewPortInputAction(ViewPortEventType activateEventType, ViewPortEventType deactivateEventType, Func<IInputInfo, bool> activateCondition, Func<IInputInfo, bool> deactivateCondition = null, Func<IInputInfo, bool> activate = null, Func<IInputInfo, bool> deactivate = null, Func<IInputInfo, bool> process = null)
            : this(0, activateEventType, deactivateEventType, activateCondition, deactivateCondition, activate, deactivate, process)
        { }

        private ViewPortInputAction(int tmp, ViewPortEventType activateEventType, ViewPortEventType? deactivateEventType, Func<IInputInfo, bool> activateCondition, Func<IInputInfo, bool> deactivateCondition, Func<IInputInfo, bool> activate, Func<IInputInfo, bool> deactivate, Func<IInputInfo, bool> process)
        {

            if (activateCondition == null)
                throw new ArgumentNullException("activateCondition");

            _activateEventType = activateEventType;
            _deactivateEventType = deactivateEventType ?? ViewPortEventType.All & ~activateEventType;

            _activateCondition = activateCondition;
            _deactivateCondition = x => (_deactivateEventType !=ViewPortEventType.None && (_deactivateEventType | x.EventType) == x.EventType) || deactivateCondition == null ? !activateCondition(x) : deactivateCondition(x);

            _activate = activate;
            _deactivate = deactivate;
            _process = process;
        }

        public bool Activate(IInputInfo info)
        {
            var byEventType = (_activateEventType | info.EventType) == _activateEventType;
            var byCondition = _activateCondition(info);

            var result = byEventType && byCondition;
            if(result)
                Show("Acivating [" + GetCaption() + "] by {0}\n\t- condition: {1}\n\t- by event type: {2}", info, byCondition, byEventType);

            result = result && ProcessActivate(info);
            return result;
        }

        private bool ProcessActivate(IInputInfo info)
        {
            if (_activate == null) 
                return true;

            var result = _activate(info);
            Show("\t- by delegate: {0}", result);

            return result;
        }

        public bool Deactivate(IInputInfo info)
        {
            var byCompleted = info.Completed;
            var byCondition = _deactivateCondition(info);
            var byEventType = _deactivateEventType != ViewPortEventType.None && (_deactivateEventType | info.EventType) == _deactivateEventType;


            var result = (byCompleted || byCondition || byEventType);

            if(result)
                Show("Deacivating [" + GetCaption() + "] by {0}\n\t- condition: {1}\n\t- by event type: {2}", info, byCondition, byEventType);

            
            result = result && ProcessDeactivate(info);
            return result;
        }

        private bool ProcessDeactivate(IInputInfo info)
        {
            if (_deactivate == null) 
                return true;

            var result = _deactivate(info);
            Show("\t- by delegate: {0}", result);

            return result;
        }

        public bool Process(IInputInfo info)
        {
            var result = (_process == null || _process(info));
            return result;
        }

        private void Show(string format, params object[] args)
        {
            if (_show)
                Console.WriteLine(GetOwnerCaption() + ":" + format, args);
        }

        private string GetCaption()
        {
            return this.Tag("Caption").As<string>();
        }

        private string GetOwnerCaption()
        {
            return this.Tag("Owner").Tag("Caption").As<string>();
        }
    }
}