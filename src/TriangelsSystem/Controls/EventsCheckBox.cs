using MonoGame.Extended.Gui.Controls;
using System;
using System.Linq;
using System.Reflection;

namespace TriangelsSystem.Controls
{
    internal class EventsCheckBox : CheckBox
    {
        public event Action<bool> CheckedStateChanged;

        public EventsCheckBox()
        {
            var toggleField1 = typeof(CheckBox).GetRuntimeFields();

            var toggleField = typeof(CheckBox).GetRuntimeFields()
                .Where(f => f.FieldType == typeof(ToggleButton) && f.Name == "_toggleButton")
                .Single();

            var toggle = toggleField.GetValue(this) as ToggleButton;
            toggle.CheckedStateChanged += Toggle_CheckedStateChanged;
        }

        private void Toggle_CheckedStateChanged(object sender, EventArgs e)
        {
            CheckedStateChanged?.Invoke(this.IsChecked);
        }
    }
}
