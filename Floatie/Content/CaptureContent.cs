using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Floatie
{
    public class CaptureContent : Content
    {
        public override Image imgData { get; set; }
        public override Container cont { get; set; }

        Bitmap cross1 = new Bitmap(48, 48);
        Bitmap cross2 = new Bitmap(48, 48);

        public CaptureContent(Container _cont)
        {
            cont = _cont;

            var brush1 = new SolidBrush(Color.White);
            var brush2 = new SolidBrush(Color.Black);

            using (Graphics g = Graphics.FromImage(cross1))
            {
                Rectangle Shape1 = new Rectangle(16, 0, 16, 64);
                Rectangle Shape2 = new Rectangle(0, 16, 64, 16);
                g.FillRectangle(brush1, Shape1);
                g.FillRectangle(brush1, Shape2);
            }

            using (Graphics g = Graphics.FromImage(cross2))
            {
                Rectangle Shape1 = new Rectangle(16, 0, 16, 64);
                Rectangle Shape2 = new Rectangle(0, 16, 64, 16);
                Rectangle Shape3 = new Rectangle(20, 4, 8, 40);
                Rectangle Shape4 = new Rectangle(4, 20, 40, 8);

                g.FillRectangle(brush1, Shape1);
                g.FillRectangle(brush1, Shape2);
                g.FillRectangle(brush2, Shape3);
                g.FillRectangle(brush2, Shape4);
            }

            cont.pnDrag.BackgroundImage = cross1;

            cont.pnDrag.MouseEnter += PnDrag_MouseEnter;
            cont.pnDrag.MouseLeave += PnDrag_MouseLeave;

            cont.pnDrag.MouseDown += PnDrag_MouseDown;

            cont.Width = 420;
            cont.Height = 420;


            cont.pnDrag.Padding = new Padding(8, 16, 8, 8);

            cont.BackColor = Color.LightGray;
            cont.Opacity = 0.3;

            cont.WindowState = FormWindowState.Minimized;
            cont.Show();
            cont.WindowState = FormWindowState.Normal;
            cont.BringToFront();
            cont.Focus();

            //SaveManager.Update(cont);

        }

        private void PnDrag_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            { 
                Rectangle hitbox = new Rectangle(new Point((cont.pnDrag.Width / 2) - 24, (cont.pnDrag.Height / 2) - 24), new Size(48, 48));
                if (hitbox.Contains(e.Location))
                {
                    Capture();
                }
            }
        }

        private void Capture()
        {
            Bitmap shotBmp = new Bitmap(cont.Width, cont.Height);
            Rectangle shotBounds = new Rectangle(cont.Location.X, cont.Location.Y, cont.Width, cont.Height);
            using (Graphics g = Graphics.FromImage(shotBmp))
            {
                var lastLocation = cont.Location;
                cont.Hide();
                g.CopyFromScreen(shotBounds.Location, Point.Empty, shotBounds.Size);
                var newCont = Main.AddNewContainer();
                newCont.LoadImage(shotBmp);
                newCont.Location = lastLocation;// new Point(lastLocation.X+16, lastLocation.Y+16);
                cont.destroyOnClose = true;
                cont.Close();
            }
        }

        private void PnDrag_MouseLeave(object sender, EventArgs e)
        {
            cont.pnDrag.BackgroundImage = cross1;
        }

        private void PnDrag_MouseEnter(object sender, EventArgs e)
        {
            cont.pnDrag.BackgroundImage = cross2;
        }

        private void TbText_MouseHover(object sender, EventArgs e)
        {
            cont.ViewDisplayTimer_Reload();
        }


        public override void ShowContent()
        {
            //cont.BackgroundImage = imgData;
            //cont.BackgroundImageLayout = ImageLayout.Stretch;
        }

        public override void HideContent()
        {

            //cont.BackgroundImage = null;
            //cont.BackgroundImageLayout = ImageLayout.Stretch;
        }
    }


}
