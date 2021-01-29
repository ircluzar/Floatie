using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Floatie
{
    public class ScramblerContent : Content
    {
        public override Container cont { get; set; }

        public Timer ScramblerTimer = new Timer();

        public ScramblerContent(Container _cont)
        {
            cont = _cont;
            cont.content = this;

            cont.Hide();

            ScramblerTimer.Interval = 35;
            ScramblerTimer.Tick += ScramblerTimer_Tick;
            ScramblerTimer.Start();

            cont.pnDrag.BackgroundImage = null;
            cont.BackColor = Color.FromArgb(32, 32, 32);
            cont.TransparencyKey = Color.FromArgb(32, 32, 32);
            cont.pnDrag.BorderStyle = BorderStyle.None;

            cont.ScramblerActive = true;

            cont.Width = 256;
            cont.Height = 64;

            cont.TopMost = true;
            cont.Show();
        }

        public override Image imgData { get; set; }

        Bitmap ScramblerBitmap = null;
        private void ScramblerTimer_Tick(object sender, EventArgs e)
        {
            Color ScrambleColor = Color.FromArgb(1, 0, 2);
            Color ScrambleColor2 = Color.FromArgb(254, 255, 253);
            var brush1 = new SolidBrush(ScrambleColor);
            var brush2 = new SolidBrush(ScrambleColor2);

            Bitmap bmp = new Bitmap(cont.Width, cont.Height);
            List<(int x, int y)> pixelCoords = new List<(int x, int y)>();

            using (Graphics g = Graphics.FromImage(bmp))
            {
                for (int x = 0; x < cont.Width; x++)
                    for (int y = 0; y < cont.Height; y++)
                    {
                        if (Main.rnd.Next(0, 20) > 6)
                        {
                            if (Main.rnd.Next(0, 20) > 7)
                                g.FillRectangle(brush1, x, y, 1, 2);
                            //bmp.SetPixel(x, y, ScrambleColor);
                            else
                                g.FillRectangle(brush2, x, y, 2, 1);
                            //bmp.SetPixel(x, y, ScrambleColor2);

                        }
                        else
                            pixelCoords.Add((x, y));

                    }
            }

            Bitmap shotBmp = new Bitmap(cont.Width, cont.Height);
            Rectangle shotBounds = new Rectangle(cont.Location.X, cont.Location.Y, cont.Width, cont.Height);
            using (Graphics g = Graphics.FromImage(shotBmp))
                g.CopyFromScreen(shotBounds.Location, Point.Empty, shotBounds.Size);


            cont.BackgroundImage = bmp;

            var TargetScreen = Main.containers.FirstOrDefault(it => it.content != null && it.content is ScramblerTargetContent);
            if (TargetScreen != null)
            {
                if (ScramblerBitmap == null || TargetScreen.Size != cont.Size)
                {
                    TargetScreen.Size = cont.Size;
                    ScramblerBitmap = new Bitmap(cont.Width, cont.Height);
                }

                using (Graphics g = Graphics.FromImage(ScramblerBitmap))
                    foreach (var coord in pixelCoords)
                    {
                        Color pixel = shotBmp.GetPixel(coord.x, coord.y);
                        if (pixel != ScrambleColor && pixel != ScrambleColor2)
                            g.FillRectangle(new SolidBrush(pixel), coord.x, coord.y, 1, 1);

                    }

                if (Main.rnd.Next(0, 20) > 1)
                {
                    TargetScreen.BackgroundImage = ScramblerBitmap;
                    TargetScreen.Refresh();
                }
            }
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

    public class ScramblerTargetContent : Content
    {
        public override Container cont { get; set; }

        public ScramblerTargetContent(Container _cont)
        {
            cont = _cont;
            cont.content = this;

            cont.Hide();

            cont.pnDrag.BackgroundImage = null;
            cont.BackColor = Color.FromArgb(32, 32, 32);
            cont.pnDrag.BorderStyle = BorderStyle.None;

            //cont.ScramblerTarget = true;
            cont.ScramblerActive = true;

            cont.Width = 256;
            cont.Height = 64;

            cont.Show();

        }

        public override Image imgData { get; set; }

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
