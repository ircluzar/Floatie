using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Floatie
{
    public class TextContent : Content
    {
        public override Image imgData { get; set; }
        public override Container cont { get; set; }

        public int FontSize = 14;

        public TextBox tbText = null;
        Timer SaveTimer;

        public void SetSize()
        {
            System.Drawing.Font myFont = new System.Drawing.Font("Segoe UI", FontSize);
            tbText.Font = myFont;
        }
        public TextContent(Container _cont, string text)
        {
            cont = _cont;

            cont.pnDrag.BackgroundImage = null;



            tbText = new TextBox();
            tbText.Text = text;
            cont.TextData = tbText.Text;

            tbText.Multiline = true;

            SetSize();

            if (cont.TextColor == null)
                cont.TextColor = Extensions.GetRandomPastelColor();


            tbText.BackColor = cont.TextColor.Value;
            tbText.BorderStyle = BorderStyle.None;
            tbText.MaxLength = 65536;

            cont.Width = 256;
            cont.Height = 256;

            tbText.Location = new Point(8, 24);
            tbText.Width = 256-16;
            tbText.Height = 256-32;

            tbText.MouseWheel += TbText_MouseWheel;
            tbText.MouseHover += TbText_MouseHover;
            //tbText.ScrollBars = ScrollBars.Vertical;

            tbText.TextChanged += TbText_TextChanged;
            tbText.KeyDown += TbText_KeyDown;
            tbText.KeyUp += TbText_KeyUp;

            cont.pnDrag.Padding = new Padding(8, 16, 8, 8);

            tbText.Visible = false;
            cont.pnDrag.Controls.Add(tbText);
            //tbText.Dock = DockStyle.Fill;
            tbText.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);

            tbText.Visible = true;

            tbText.Show();

            cont.WindowState = FormWindowState.Minimized;
            cont.Show();
            cont.WindowState = FormWindowState.Normal;
            cont.BringToFront();
            cont.Focus();

            SaveManager.Update(cont);

            SaveTimer = new Timer();
            SaveTimer.Interval = 1500;
            SaveTimer.Tick += SaveTimer_Tick;
        }

        private void TbText_MouseHover(object sender, EventArgs e)
        {
            cont.ViewDisplayTimer_Reload();
        }

        private void SaveTimer_Tick(object sender, EventArgs e)
        {
            SaveTimer.Stop();

            int cursorPos = tbText.SelectionStart;


            if (tbText.Text.Length > 4 && tbText.Text.Substring(0, 4).ToUpper() == "HTTP")
            {
                tbText.Text = " " + tbText.Text;
                tbText.SelectionStart = cursorPos + 1;
            }

            cont.TextData = tbText.Text;

            SaveManager.Update(cont);
        }

        bool ControlIsHeld = false;
        private void TbText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                cont.destroyOnClose = true;
                cont.Close();
            }

            if (e.KeyCode == Keys.ControlKey)
                ControlIsHeld = true;

        }

        private void TbText_KeyUp(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.ControlKey)
                ControlIsHeld = false;

        }

        private void TbText_TextChanged(object sender, EventArgs e)
        {
            SaveTimer.Stop();
            SaveTimer.Start();
        }

        private void TbText_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!ControlIsHeld)
                return;

            var move = Convert.ToInt32(e.Delta / 120);

            FontSize += move;

            if (FontSize < 4)
                FontSize = 4;

            SetSize();

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
