using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Floatie
{
    public class AiContent : Content
    {
        public override Container cont { get; set; }

        public Timer AiBackgroundTimer = new Timer();
        public Timer AiThinkTimer = new Timer();

        public static volatile string AiReturn = null;

        public AiContent(Container _cont)
        {
            cont = _cont;
            cont.content = this;

            cont.pnDrag.BackgroundImage = GenerateCross1();

            cont.pnDrag.MouseEnter += PnDrag_MouseEnter;
            cont.pnDrag.MouseLeave += PnDrag_MouseLeave;

            cont.pnDrag.MouseDown += PnDrag_MouseDown;


            cont.Hide();

            AiBackgroundTimer.Interval = 69;
            AiBackgroundTimer.Tick += AiBackgroundTimer_Tick;
            AiBackgroundTimer.Start();

            AiThinkTimer.Interval = 22;
            AiThinkTimer.Tick += AiThinkTimer_Tick;

            cont.pnDrag.BackgroundImage = null;
            cont.BackColor = Color.FromArgb(32, 32, 32);

            cont.ScramblerActive = true;

            cont.Width = 128;
            cont.Height = 128;

            //cont.TopMost = true;
            cont.Show();
        }

        private void AiThinkTimer_Tick(object sender, EventArgs e)
        {
            var prevImg = cont.pnDrag.BackgroundImage;


            using (Graphics g = Graphics.FromImage(prevImg))
            {
                for(int i = 0; i< Main.rnd.Next(16);i++)
                {
                    SolidBrush brush1;

                    switch(Main.rnd.Next(4))
                    {
                        case 0:
                            brush1 = new SolidBrush(Extensions.GetRandomAntiPastelColor());
                            break;
                        case 3:
                        case 1:
                            brush1 = new SolidBrush(Extensions.GetRandomColor());
                            break;
                        case 2:
                        case 4:
                        default:
                            brush1 = new SolidBrush(Extensions.GetRandomPastelColor());
                            break;
                    }

                    Rectangle shape = new Rectangle(Main.rnd.Next(16), Main.rnd.Next(16), Main.rnd.Next(16), Main.rnd.Next(16));
                    g.FillRectangle(brush1, shape);

                }
            }

            cont.pnDrag.BackgroundImage = prevImg;


            if(AiReturn != null)
            {
                AiThinkTimer.Stop();
                cont.pnDrag.BackgroundImage = GenerateCross1();
                MessageBox.Show($"117M says:\n{AiReturn}", "Ask 117M", MessageBoxButtons.OK, MessageBoxIcon.Information);
                AiReturn = null;
            }
        }

        public override Image imgData { get; set; }

        public Bitmap GenerateCross1()
        {
            var brush1 = new SolidBrush(Extensions.GetRandomAntiPastelColor());

            Bitmap cross1 = new Bitmap(32, 32);

            using (Graphics g = Graphics.FromImage(cross1))
            {
                Rectangle Shape1 = new Rectangle(8, 0, 8, 24);
                Rectangle Shape2 = new Rectangle(0, 8, 24, 8);
                g.FillRectangle(brush1, Shape1);
                g.FillRectangle(brush1, Shape2);
            }

            return cross1;
        }

        public Bitmap GenerateCross2()
        {

            var brush1 = new SolidBrush(Extensions.GetRandomAntiPastelColor());
            var brush2 = new SolidBrush(Extensions.GetRandomAntiPastelColor());

            Bitmap cross2 = new Bitmap(32, 32);

            using (Graphics g = Graphics.FromImage(cross2))
            {
                Rectangle Shape1 = new Rectangle(8, 0, 8, 24);
                Rectangle Shape2 = new Rectangle(0, 8, 24, 8);
                Rectangle Shape3 = new Rectangle(10, 2, 4, 20);
                Rectangle Shape4 = new Rectangle(2, 10, 20, 4);

                g.FillRectangle(brush1, Shape1);
                g.FillRectangle(brush1, Shape2);
                g.FillRectangle(brush2, Shape3);
                g.FillRectangle(brush2, Shape4);
            }

            return cross2;
        }

        private void AiBackgroundTimer_Tick(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(cont.Width, cont.Height);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                for (int x = 0; x < cont.Width; x++)
                    for (int y = 0; y < cont.Height; y++)
                    {
                        if (Main.rnd.Next(0, 1000) > 995)
                        {
                                g.FillRectangle(new SolidBrush(Extensions.GetRandomPastelColor()), x, y, Main.rnd.Next(16, 64), Main.rnd.Next(16, 64));

                        }

                    }
            }

            cont.BackgroundImage = bmp;

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

        private void PnDrag_MouseLeave(object sender, EventArgs e)
        {
            if (AiThinkTimer.Enabled)
                return;

            cont.pnDrag.BackgroundImage = GenerateCross1();
        }

        private void PnDrag_MouseEnter(object sender, EventArgs e)
        {
            if (AiThinkTimer.Enabled)
                return;

            cont.pnDrag.BackgroundImage = GenerateCross2();
        }

        private void PnDrag_MouseDown(object sender, MouseEventArgs e)
        {
            if (AiThinkTimer.Enabled)
                return;


            if (e.Button == MouseButtons.Left)
            {
                Rectangle hitbox = new Rectangle(new Point((cont.pnDrag.Width / 2) - 12, (cont.pnDrag.Height / 2) - 12), new Size(24, 24));
                if (hitbox.Contains(e.Location))
                {
                    Ask();
                }
            }
        }

        private void Ask()
        {
            string value = "";

            if (Extensions.GetInputBox("Ask 117M", "Your message:", ref value) == DialogResult.OK && !string.IsNullOrWhiteSpace(value))
            {
                //send shit to AI
                new object();
                AiThinkTimer.Start();

                Task.Run(() => {

                    try
                    {
                        AiReturn = AskGPT2_117M.GetMessage(value);
                    }
                    catch
                    {
                        AiReturn = "I don't know";
                    }

                });

                
            }
        }

    }

}
