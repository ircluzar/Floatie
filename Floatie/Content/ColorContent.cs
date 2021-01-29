using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Floatie
{
    public class ColorContent : Content
    {
        public override Image imgData { get; set; }

        public override Container cont { get; set; }

        public ColorContent(Container _cont)
        {
            cont = _cont;

            cont.Hide();

            var bmp = new Bitmap(1, 1);
            bmp.SetPixel(0, 0, Color.Gray);

            imgData = bmp;
            cont.pnDrag.BackgroundImage = null;
            cont.BackgroundImage = imgData;
            cont.pnDrag.BorderStyle = BorderStyle.None;

            cont.AspectRatio = false;
            cont.CensorActive = true;
            cont.TopMost = true;
            cont.Size = new Size(540, 196);

            cont.Show();

            for (int i = 0; i < 16; i++)
            {
                Color c;
                switch (i)
                {
                    default:
                    case 0:
                        c = Color.Gray;
                        break;
                    case 1:
                        c = Color.FromArgb(255, 192, 192);
                        break;
                    case 2:
                        c = Color.FromArgb(192, 255, 192);
                        break;
                    case 3:
                        c = Color.FromArgb(192, 192, 255);
                        break;
                    case 4:
                        c = Color.FromArgb(255, 128, 255);
                        break;
                    case 5:
                        c = Color.FromArgb(255, 192, 128);
                        break;
                    case 6:
                        c = Color.Yellow;
                        break;
                    case 7:
                        c = Color.Blue;
                        break;
                    case 8:
                        c = Color.Lime;
                        break;
                    case 9:
                        c = Color.Red;
                        break;
                    case 10:
                        c = Color.FromArgb(255, 128, 0);
                        break;
                    case 11:
                        c = Color.Aqua;
                        break;
                    case 12:
                        c = Color.Purple;
                        break;
                    case 13:
                        c = Color.Fuchsia;
                        break;
                    case 14:
                        c = Color.White;
                        break;
                    case 15:
                        c = Color.Black;
                        break;
                }

                Button b = new Button();
                b.BackColor = c;
                b.FlatStyle = FlatStyle.Flat;
                b.FlatAppearance.BorderSize = 0;
                int squaresize = 16;

                Size s = new Size(squaresize, squaresize);
                b.Size = s;
                int xpos = ((s.Width * 2) * (i + 1)) - s.Width;
                int ypos = cont.pnDrag.Height - ( 2 * s.Height );
                Point p = new Point(xpos, ypos);
                cont.pnDrag.Controls.Add(b);
                b.Location = p;
                b.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
                b.Show();

                b.Click += (o, e) =>
                {
                    var ob = (o as Button);
                    ob.Parent.BackColor = ob.BackColor;
                };
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
}
