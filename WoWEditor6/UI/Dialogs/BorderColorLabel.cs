using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WoWEditor6.UI.Dialogs
{
    public class BorderColorLabel : Label
    {
        private Color mColor = Color.Black;

        public Color BorderColor
        {
            get { return mColor; }
            set
            {
                mColor = value;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle, mColor, ButtonBorderStyle.Solid);
        }
    }
}
