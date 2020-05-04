using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Floatie
{
    public partial class Main : Form
    {
        public static List<Container> containers = new List<Container>();
        public static Random rnd = new Random((int)DateTime.Now.Ticks);

        public Main(string[] args)
        {
            InitializeComponent();
            FloatieWebHelper.SyncObject = this;

            //steal icon from ressources
            systray.Icon = ((System.Drawing.Icon)(new System.ComponentModel.ComponentResourceManager(typeof(Container)).GetObject("$this.Icon")));

            SaveManager.Init();


            foreach (var arg in args)
            {
                var cont = AddNewContainer();
                cont.LoadImage(arg);
            }

        }

        private void Main_Load(object sender, EventArgs e)
        {
            this.Hide();

            //initiate systray
            this.ShowInTaskbar = false;

            ContextMenuStrip cms = new ContextMenuStrip();
            cms.Items.Add("New floatie from clipboard", null, new EventHandler((ob, ev) =>
            {
                var cont = AddNewContainer();
                cont.LoadFromClipboard();
            }));

            cms.Items.Add("New blank floatie", null, new EventHandler((ob, ev) =>
            {
                AddNewContainer();
            }));

            cms.Items.Add(new ToolStripSeparator());

            cms.Items.Add("New text floatie", null, new EventHandler((ob, ev) =>
            {
                LoadText();
            }));

            cms.Items.Add("New capture floatie", null, new EventHandler((ob, ev) =>
            {
                LoadCapture();
            }));

            cms.Items.Add("New censor floatie", null, new EventHandler((ob, ev) =>
            {
                LoadCensor();
            }));

            cms.Items.Add("New scrambler floatie", null, new EventHandler((ob, ev) =>
            {
                LoadScrambler();
            }));

            cms.Items.Add(new ToolStripSeparator());

            cms.Items.Add("Hide all", null, new EventHandler((ob, ev) =>
            {
                HideAllFloaties();
            }));

            cms.Items.Add("Show all", null, new EventHandler((ob, ev) =>
            {
                ShowAllFloaties();
            }));

            cms.Items.Add("Reset", null, new EventHandler((ob, ev) =>
            {
                if (MessageBox.Show( "This will reset Floatie's memory. Do you want to continue?", "Reset Floatie?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    SaveManager.Reset();
            }));

            cms.Items.Add(new ToolStripSeparator());

            cms.Items.Add("Close", null, new EventHandler((ob, ev) =>
            {
                foreach (var cont in containers)
                {
                    if (cont.ScramblerActive || cont.CensorActive || cont.content == null || (cont.content.imgData == null && cont.TextData == null))
                    {
                        cont.destroyOnClose = true;
                        cont.Close();
                    }
                }

                Application.Exit();

            }));

            systray.ContextMenuStrip = cms;
            systray.Visible = true;

        }

        public static Container AddNewContainer(bool setForLoading = false)
        {
            var cont = new Floatie.Container(setForLoading:setForLoading);
            containers.Add(cont);

            if (!setForLoading)
            {
                cont.WindowState = FormWindowState.Minimized;
                cont.Show();
                cont.WindowState = FormWindowState.Normal;
                cont.BringToFront();
                cont.Focus();
            }

            return cont;
        }

        private void systray_DoubleClick(object sender, EventArgs e)
        {
            var cont = AddNewContainer();

            //if (Clipboard.ContainsText() || Clipboard.ContainsImage() || Clipboard.ContainsFileDropList())
            //    cont.LoadFromClipboard();
        }

        public void ShowAllFloaties()
        {
            foreach (var cont in containers)
                if (!cont.IsDisposed)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        cont.WindowState = FormWindowState.Minimized;
                        cont.Show();
                        cont.WindowState = FormWindowState.Normal;
                        cont.BringToFront();
                        cont.Focus();
                    }
                }
        }

        public void HideAllFloaties()
        {
            foreach (var cont in containers)
                if (!cont.IsDisposed)
                    cont.Hide();
        }


        public void LoadCensor()
        {
            AddNewContainer().LoadCensor();
        }

        public void LoadText()
        {
            AddNewContainer().LoadText();
        }

        public void LoadCapture()
        {
            AddNewContainer().LoadCapture();
        }

        public void LoadScrambler()
        {
            CloseAllScramblers();
            AddNewContainer().LoadScrambler();
            AddNewContainer().LoadScramblerTarget();
        }

        public void CloseAllScramblers()
        {
            List<Container> unregister = new List<Container>();
            foreach (var scrm in containers.Where(it => it.content != null && (it.content is ScramblerContent || it.content is ScramblerTargetContent)))
                unregister.Add(scrm);

            foreach (var scrm in unregister)
            {
                try
                {
                    (scrm.content as ScramblerContent)?.ScramblerTimer.Stop();
                    scrm.Close();
                    containers.Remove(scrm);
                    scrm.Dispose();
                }
                catch { } //bugs don't exist
            }

        }

        internal static void Restart()
        {
            try
            {
                Application.Restart();
            }
            catch { } //bugs don't exist
        }
    }
}
