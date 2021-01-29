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
    public class WebContent : Content
    {
        public override Image imgData { get; set; }
        public override Container cont { get; set; }

        public ChromiumWebBrowser browser = null;


        public WebContent(Container _cont, string text, bool reloadWeb)
        {
            cont = _cont;

            if (!reloadWeb)
            {
                cont.Width = 960;
                cont.Height = 600;
            }

            cont.pnDrag.BackgroundImage = null;
            browser = new ChromiumWebBrowser(text);

            browser.Location = new Point(0, 0);
            browser.Width = 960;
            browser.Height = 600;

            cont.pnDrag.Padding = new Padding(16, 16, 16, 16);

            browser.Visible = false;
            cont.pnDrag.Controls.Add(browser);
            cont.Dock = DockStyle.Fill;


            browser.KeyboardHandler = new CustomKeyboardHandler();
            browser.MenuHandler = new CustomMenuHandler();
            browser.Visible = true;

            browser.Show();

            cont.Show();

            if (reloadWeb)
            {
                cont.WindowState = FormWindowState.Minimized;
                cont.Show();
                cont.WindowState = FormWindowState.Normal;
                cont.BringToFront();
                cont.Focus();
            }
            else
                SaveManager.Update(cont);
        }

        public override void ShowContent()
        {

        }

        public override void HideContent()
        {

        }

        public override void Close()
        {

        }
    }


}
