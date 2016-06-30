using System.Windows.Forms;

namespace Shapes
{
    public class ViewPortInputInfoProcessor : InputInfoProcessor
    {
        public static ViewPortInputInfoProcessor New(IViewPort viewPort, ViewPortMode mode = ViewPortMode.Selection, string caption = null)
        {
            var processor = new ViewPortInputInfoProcessor(viewPort) { Mode = mode };
            if (!string.IsNullOrWhiteSpace(caption))
                processor.Tag("Caption", caption);
            return processor;
        }
        public bool AllowChangeZoom { get; set; }
        public bool AllowChangePosition { get; set; }

        public ViewPortInputInfoProcessor(IViewPort viewPort) : base(viewPort)
        {
            // движение вьюпорта мышкой
            Actions.OnMouseMove(x => AllowChangePosition && IsSelectionMode && KeysEmptyAndButonIs(x, MouseButtons.Middle), process: ChangePosition)
                .Tag("Caption", "движение вьюпорта мышкой");

            // движение вьюпорта с клавиатуры
            Actions.OnKeyDown(CheckChangePositionKeys, (ViewPortEventType.KeyDown | ViewPortEventType.KeysRepeated).Invert(), process: ChangePositionByKeys)
                .Tag("Caption", "движение вьюпорта с клавиатуры");


            // зум мышкой
            Actions.OnMouseWheel(x => AllowChangeZoom && IsSelectionMode && x.Keys == Keys.ControlKey, process: ProcessZoom)
                .Tag("Caption", "зум мышкой");

            // зум с клавиатуры
            Actions.OnKeyDown(CheckProcessZoomKeys, process: ProcessZoomByKeys)
                .Tag("Caption", "зум с клавиатуры");
        }

        private bool CheckProcessZoomKeys(IInputInfo info)
        {
            return AllowChangeZoom && IsSelectionMode
                   && info.Keys.Count == 2
                   && info.Keys.Contains(Keys.ControlKey)
                   && info.Keys.ContainsOnlyAny(Keys.ControlKey, Keys.Add, Keys.Oemplus, Keys.Subtract, Keys.OemMinus);
        }



        private bool CheckChangePositionKeys(IInputInfo info)
        {
            return AllowChangePosition && IsSelectionMode && info.Keys.Count >= (info.Keys.Contains(Keys.ControlKey) ? 2 : 1) && info.Keys.ContainsOnlyAny(Keys.ControlKey, Keys.Up, Keys.Down, Keys.Left, Keys.Right);
        }


        bool ChangePosition(IInputInfo info)
        {
            if (!info.ClientOffset.HasValue)
                return false;

            var offset = info.ClientOffset.Value;

            return ChangePosition(offset);
        }

        private bool ChangePositionByKeys(IInputInfo info)
        {
            var d = info.Keys.Contains(Keys.ControlKey) ? 5f : 1f;

            var dx = 0f;
            var dy = 0f;
            if (info.Keys.Contains(Keys.Up))
                dy -= 1;
            if (info.Keys.Contains(Keys.Down))
                dy += 1;
            if (info.Keys.Contains(Keys.Left))
                dx -= 1;
            if (info.Keys.Contains(Keys.Right))
                dx += 1;

            return ChangePosition(new Vector2F(dx * d, dy * d));
        }

        private bool ChangePosition(Vector2F offset)
        {
            InputInfo.ViewPort.Position -= offset;

            return true;
        }

        bool ProcessZoom(IInputInfo info)
        {
            if (!info.Wheel.HasValue || !info.ClientPoint.HasValue)
                return false;

            return ProcessZoom(info.Wheel.Value, info.ClientPoint.Value);
        }

        private bool ProcessZoomByKeys(IInputInfo info)
        {
            var wheel = info.Keys.Contains(Keys.Subtract) || info.Keys.Contains(Keys.OemMinus)
                ? -1f
                : 1f;
            return ProcessZoom(wheel);
        }


        bool ProcessZoom(float wheel, Vector2F? clietnPoint = default(Vector2F?))
        {
            InputInfo.ViewPort.ChangeZoom(wheel, clietnPoint);

            return true;
        }

    }
}