using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Floatie
{
    public class CensorContent : Content
    {
        public override Image imgData { get; set; }

        public override Container cont { get; set; }

        public CensorContent(Container _cont)
        {
            cont = _cont;

            cont.Hide();

            var bmp = new Bitmap(1, 1);
            bmp.SetPixel(0, 0, Color.Black);

            imgData = bmp;
            cont.pnDrag.BackgroundImage = null;
            cont.BackgroundImage = imgData;
            cont.pnDrag.BorderStyle = BorderStyle.None;

            cont.AspectRatio = false;
            cont.CensorActive = true;
            cont.TopMost = true;
            cont.Size = new Size(256, 64);

            cont.Show();

        }

        public override void ShowContent()
        {
            cont.BackgroundImage = imgData;
            cont.BackgroundImageLayout = ImageLayout.Stretch;
        }

        public override void HideContent()
        {

            cont.BackgroundImage = null;
            cont.BackgroundImageLayout = ImageLayout.Stretch;
        }

        public override void Close()
        {

        }
    }
}
