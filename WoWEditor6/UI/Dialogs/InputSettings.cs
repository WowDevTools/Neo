using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using WoWEditor6.Settings;

namespace WoWEditor6.UI.Dialogs
{
    public partial class InputSettings : Form
    {
        private static readonly KeysConverter Converter = new KeysConverter();
        private KeyBindingControl mCurrentBinding;
        private readonly List<Keys> mCurrentKeys = new List<Keys>();
        private readonly List<Keys> mCurrentPressedKeys = new List<Keys>();

        public InputSettings()
        {
            InitializeComponent();
        }

        public CheckBox InvertMouseBox => invertMouseBox;

        private void InputSettings_Resize(object sender, EventArgs e)
        {
            bindingPanel.Size = new Size(ClientSize.Width, ClientSize.Height - 50);
        }

        private void ElementClicked(object sender, EventArgs args)
        {
            var button = sender as Button;
            if (button == null)
                return;

            var binding = button.Parent as KeyBindingControl;

            if(binding != null && binding == mCurrentBinding)
            {
                mCurrentBinding.Label.BorderColor = Color.Black;
                mCurrentBinding = null;
                return;
            }

            if (binding == null)
                return;

            if(mCurrentBinding != null)
                mCurrentBinding.Label.BorderColor = Color.Black;

            binding.Label.BorderColor = Color.Red;

            mCurrentBinding = binding;
            mCurrentKeys.Clear();
        }

        private void OnKeyDown(object sender, KeyEventArgs args)
        {
            if (mCurrentPressedKeys.Contains(args.KeyCode))
                return;

            if (mCurrentBinding == null)
                return;

            mCurrentKeys.Add(args.KeyCode);
            mCurrentPressedKeys.Add(args.KeyCode);
            mCurrentBinding.Label.Text = string.Join(" + ", mCurrentKeys.Select(k => Converter.ConvertToString(k)));
        }

        private void OnKeyUp(object sender, KeyEventArgs args)
        {
            if (mCurrentBinding == null)
                return;

            mCurrentPressedKeys.Remove(args.KeyCode);
            if (mCurrentPressedKeys.Count != 0)
                return;

            mCurrentBinding.Label.BorderColor = Color.Black;
            var bindField = mCurrentBinding.Tag as Tuple<FieldInfo, object>;
            if (bindField != null && mCurrentKeys.Count > 0)
            {
                bindField.Item1.SetValue(bindField.Item2, mCurrentKeys.ToArray());
                KeyBindings.Save();
            }
                
            mCurrentBinding = null;
        }

        private void InputSettings_Load(object sender, EventArgs e)
        {
            KeyDown += OnKeyDown;
            KeyUp += OnKeyUp;

            var baseType = typeof (KeyBindings);
            foreach(var category in baseType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var catName = category.Name;
                var instance = category.GetValue(KeyBindings.Instance);

                foreach (var binding in instance.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (binding.FieldType.IsArray == false)
                        continue;

                    var arrType = binding.FieldType.GetElementType();
                    if (arrType != typeof(Keys))
                        continue;

                    var elemName = binding.Name;
                    var keys = (Keys[])binding.GetValue(instance);
                    var control = new KeyBindingControl
                    {
                        Button = {Text = string.Format("{0}.{1}", catName, elemName)},
                        Label = {Text = string.Join(" + ", keys.Select(k => Converter.ConvertToString(k)))},
                        Tag = new Tuple<FieldInfo, object>(binding, instance)
                    };

                    bindingPanel.Controls.Add(control);
                    control.Button.Click += ElementClicked;
                    
                }
            }
        }

        private void helpButton_Click(object sender, EventArgs e)
        {
            bindingToolTip.Show(
@"Click the button containing the action you would like to perform.
Press and hold the new key(s) and release them when you have selected all.

Click the button again while still holding at least one key to reset the
binding to what it was before.",
                bindingPanel, bindingPanel.Width / 2, 0, 8000);
        }
    }
}
