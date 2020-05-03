using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Floatie
{
    public partial class Container : Form
    {
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int
            HT_CAPTION = 0x2,
            HT_LEFT = 0xA,
            HT_RIGHT = 0xB,
            HT_TOP = 0xC,
            HT_TOPLEFT = 0xD,
            HT_TOPRIGHT = 0xE,
            HT_BOTTOM = 0xF,
            HT_BOTTOMLEFT = 0x10,
            HT_BOTTOMRIGHT = 0x11;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public string ID;

        public Image imgDropPic = ((System.Drawing.Icon)(new System.ComponentModel.ComponentResourceManager(typeof(Container)).GetObject("$this.Icon"))).ToBitmap();
        public Image imgData;
        public ChromiumWebBrowser browser = null;

        Point lastKnownMouseLocation = new Point(0, 0);
        bool ImageJustLoaded = true;

        public bool Locked = false;
        public bool AspectRatio = true;
        public bool ScramblerActive = false;
        public bool CensorActive = false;
        public string Web = null;

        private bool _DisplayHeader = false;
        public bool DisplayHeader
        {
            get
            {
                return _DisplayHeader || ScramblerActive || !TransparencyKey.IsEmpty || pnDrag.BackgroundImage != null;
            }
            set
            {
                _DisplayHeader = value;
            }
        }

        private bool _DisplayKnob = false;
        public bool DisplayKnob
        {
            get
            {
                return _DisplayKnob || AspectRatio || ScramblerActive || CensorActive || !TransparencyKey.IsEmpty;
            }
            set
            {
                _DisplayKnob = value;
            }
        }


        public bool ScramblerTarget = false;
        public Timer ViewDisplayTimer = new Timer();
        public Timer ScramblerTimer = new Timer();

        public Container(string path = null, bool setForLoading = false)
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            if (!setForLoading)
            {
                ID = Guid.NewGuid().ToString();
                InitContainer(path);
                SaveManager.Update(this);
            }
            else
            {
                InitContainer(setForLoading);
            }

            



        }

        public void InitContainer(bool setForLoading)
        {
            RewireMouseMove();
            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.None;

            ViewDisplayTimer.Interval = 666;
            ViewDisplayTimer.Tick += ViewDisplayTimer_Tick;
        }

        public void InitContainer(string path = null)
        {
            RewireMouseMove();
            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.None;

            if (path != null)
            {
                loadImage(path);
            }
            else
            {
                pnDrag.BackgroundImage = imgDropPic;
            }

            ViewDisplayTimer.Interval = 666;
            ViewDisplayTimer.Tick += ViewDisplayTimer_Tick;
        }

        public void LoadCensor()
        {
            this.Hide();

            var bmp = new Bitmap(1, 1);
            bmp.SetPixel(0, 0, Color.Black);
            loadImage(bmp);
            AspectRatio = false;
            CensorActive = true;
            TopMost = true;
            Size = new Size(256, 64);

            this.Show();
        }

        public void LoadScrambler()
        {

            this.Hide();

            ScramblerTimer.Interval = 35;
            ScramblerTimer.Tick += ScramblerTimer_Tick;
            ScramblerTimer.Start();

            pnDrag.BackgroundImage = null;
            BackColor = Color.FromArgb(32, 32, 32);
            TransparencyKey = Color.FromArgb(32, 32, 32);

            ScramblerActive = true;

            Width = 256;
            Height = 64;

            this.TopMost = true;
            this.Show();
        }

        internal void LoadScramblerTarget()
        {
            this.Hide();

            pnDrag.BackgroundImage = null;
            BackColor = Color.FromArgb(32, 32, 32);

            ScramblerTarget = true;
            ScramblerActive = true;

            Width = 256;
            Height = 64;

            this.Show();
        }

        Bitmap ScramblerBitmap = null;
        private void ScramblerTimer_Tick(object sender, EventArgs e)
        {
            Color ScrambleColor = Color.FromArgb(1, 0, 2);
            Color ScrambleColor2 = Color.FromArgb(254, 255, 253);
            var brush1 = new SolidBrush(ScrambleColor);
            var brush2 = new SolidBrush(ScrambleColor2);

            Bitmap bmp = new Bitmap(Width, Height);
            List<(int x, int y)> pixelCoords = new List<(int x, int y)>();

            using (Graphics g = Graphics.FromImage(bmp))
            {
                for (int x = 0; x < Width; x++)
                    for (int y = 0; y < Height; y++)
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

            Bitmap shotBmp = new Bitmap(Width, Height);
            Rectangle shotBounds = new Rectangle(Location.X, Location.Y, Width, Height);
            using (Graphics g = Graphics.FromImage(shotBmp))
                g.CopyFromScreen(shotBounds.Location, Point.Empty, shotBounds.Size);


            this.BackgroundImage = bmp;

            var TargetScreen = Main.containers.FirstOrDefault(it => it.ScramblerTarget);
            if(TargetScreen != null)
            {
                if (ScramblerBitmap == null || TargetScreen.Size != Size )
                {
                    TargetScreen.Size = Size;
                    ScramblerBitmap = new Bitmap(Width, Height);
                }

                //foreach (var coord in pixelCoords)
                //{
                //    Color pixel = shotBmp.GetPixel(coord.x, coord.y);
                //    if(pixel != ScrambleColor && pixel != ScrambleColor2)
                //        ScramblerBitmap.SetPixel(coord.x, coord.y, pixel);
                //}
                using (Graphics g = Graphics.FromImage(ScramblerBitmap))
                    foreach (var coord in pixelCoords)
                        {
                            Color pixel = shotBmp.GetPixel(coord.x, coord.y);
                            if (pixel != ScrambleColor && pixel != ScrambleColor2)
                                g.FillRectangle(new SolidBrush(pixel), coord.x, coord.y, 1, 1);
                        //ScramblerBitmap.SetPixel(coord.x, coord.y, pixel);
                    }

                if (Main.rnd.Next(0, 20) > 1)
                {
                    //TargetScreen.BackgroundImage = null;
                   TargetScreen.BackgroundImage = ScramblerBitmap;
                   TargetScreen.Refresh();
                }
            }
        }

        private void ViewDisplayTimer_Reload()
        {
            ViewDisplayTimer.Stop();
            ViewDisplayTimer.Start();

            if (DisplayKnob)
                pnDisplayKnob.Visible = true;

            if (DisplayHeader)
                pnDisplayHeader.Visible = true;
        }
        private void ViewDisplayTimer_Tick(object sender, EventArgs e)
        {
            (sender as Timer).Stop();

            pnDisplayKnob.Visible = false;
            pnDisplayHeader.Visible = false;
        }

        public void loadImage(Image img, bool isLoadingContainer = false)
        {
            imgData = img;
            displayImage(isLoadingContainer);
        }
        public void loadImage(byte[] data)
        {
            try
            {
                MemoryStream stream = new MemoryStream(data);
                imgData = Image.FromStream(stream);
                displayImage();
            }
            catch { } //bugs don't exist
        }
        public void loadImage(string path)
        {
            try
            {
                Image img;
                using (var bmpTemp = new Bitmap(path))
                {
                    img = new Bitmap(bmpTemp);
                }

                //imgData = Image.FromFile(path);
                imgData = img;
                displayImage();
            }
            catch { } //bugs don't exist

        }
        public void displayImage(bool isLoadingContainer = false)
        {
            this.Hide();

            pnDrag.BackgroundImage = null;

            this.BackgroundImage = imgData;
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.Width = imgData.Width;
            this.Height = imgData.Height;

            int imageArea = imgData.Width * imgData.Height;
            var scrBounds = Screen.FromControl(this).Bounds;
            int screenArea = scrBounds.Width * scrBounds.Height;

            if(imageArea>screenArea)
            {
                NormalizeImageSize(Convert.ToInt32(scrBounds.Height*0.8));

                int newLocX = (scrBounds.Width / 2) - (this.Width / 2);
                int newLocY = (scrBounds.Height / 2) - (this.Height / 2);
                this.Location = new Point(newLocX, newLocY);
            }

            if(!isLoadingContainer)
                SaveManager.SetData(this);

            this.WindowState = FormWindowState.Minimized;
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.BringToFront();
            this.Focus();

        }

        public void NormalizeImageSize(int normalSize = 666)
        {
            this.Size = new Size(normalSize, normalSize);
            EnforeAspectRatio(true);
        }

        public void showImage()
        {
            this.BackgroundImage = imgData;
            this.BackgroundImageLayout = ImageLayout.Stretch;
        }

        public void hideImage()
        {
            this.BackgroundImage = null;
            this.BackgroundImageLayout = ImageLayout.Stretch;
        }


        int spacer => 15 + (AspectRatio ? 15 : 0); 
        Rectangle ClientTop => new Rectangle(0, 0, this.ClientSize.Width, spacer);
        Rectangle ClientLeft => new Rectangle(0, 0, spacer, this.ClientSize.Height);
        Rectangle ClientBottom => new Rectangle(0, this.ClientSize.Height - spacer, this.ClientSize.Width, spacer);
        Rectangle ClientRight => new Rectangle(this.ClientSize.Width - spacer, 0, spacer, this.ClientSize.Height);
        Rectangle ClientTopLeft => new Rectangle(0, 0, spacer, spacer);
        Rectangle ClientTopRight => new Rectangle(this.ClientSize.Width - spacer, 0, spacer, spacer);
        Rectangle ClientBottomLeft => new Rectangle(0, this.ClientSize.Height - spacer, spacer, spacer);
        Rectangle ClientBottomRight => new Rectangle(this.ClientSize.Width - spacer, this.ClientSize.Height - spacer, spacer, spacer);

        private void ResizeWindow(MouseEventArgs e, int wParam)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, wParam, 0);

            }
        }
        private void RedirectMouseMove(object sender, MouseEventArgs e)
        {
            Control control = (Control)sender;
            Point screenPoint = control.PointToScreen(new Point(e.X, e.Y));
            Point formPoint = PointToClient(screenPoint);
            MouseEventArgs args = new MouseEventArgs(e.Button, e.Clicks,
                formPoint.X, formPoint.Y, e.Delta);
            OnMouseMove(args);
        }

        private void Container_MouseMove(object sender, MouseEventArgs e)
        {
            if (Locked)
                return;

            //Ignore mouse state when the image just loaded
            if (ImageJustLoaded && e.Button != MouseButtons.Left)
                ImageJustLoaded = false;

            if (ImageJustLoaded)
                return;

            lastKnownMouseLocation = e.Location;

            //if (!TransparencyKey.IsEmpty)
            //if((AspectRatio || ScramblerActive) && !Locked)
            if(!Locked)
                ViewDisplayTimer_Reload();

            Cursor.Current = Cursors.Default;
            var cursor = this.PointToClient(Cursor.Position);
            if (ClientBottomRight.Contains(cursor))
            {
                Cursor.Current = Cursors.SizeNWSE;
                ResizeWindow(e, HT_BOTTOMRIGHT);
            }
            else if (!ScramblerActive && ClientTopLeft.Contains(cursor) && !AspectRatio)
            {
                if (!AspectRatio)
                {
                    Cursor.Current = Cursors.SizeNWSE;
                    ResizeWindow(e, HT_TOPLEFT);
                }
            }
            else if (!ScramblerActive && ClientTopRight.Contains(cursor))
            {
                if (!AspectRatio)
                {
                    Cursor.Current = Cursors.SizeNESW;
                    ResizeWindow(e, HT_TOPRIGHT);
                }
            }
            else if (ClientBottomLeft.Contains(cursor))
            {
                if (!AspectRatio)
                {
                    Cursor.Current = Cursors.SizeNESW;
                    ResizeWindow(e, HT_BOTTOMLEFT);
                }
            }
            else if (!ScramblerActive && ClientTop.Contains(cursor))
            {
                if (!AspectRatio)
                {
                    Cursor.Current = Cursors.SizeNS;
                    ResizeWindow(e, HT_TOP);
                }
            }
            else if (ClientLeft.Contains(cursor))
            {
                if (!AspectRatio)
                {
                    Cursor.Current = Cursors.SizeWE;
                    ResizeWindow(e, HT_LEFT);
                }
            }
            else if (ClientRight.Contains(cursor))
            {
                if (!AspectRatio)
                {
                    Cursor.Current = Cursors.SizeWE;
                    ResizeWindow(e, HT_RIGHT);
                }
            }
            else if (ClientBottom.Contains(cursor))
            {
                if (!AspectRatio)
                {
                    Cursor.Current = Cursors.SizeNS;
                    ResizeWindow(e, HT_BOTTOM);
                }
            }
            else //if (this.ClientRectangle.Contains(cursor))
            {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            }

        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= 0x20000; // <--- use 0x20000
                return cp;
            }
        }

        private void Container_Resize(object sender, EventArgs e)
        {
            if(Cursor.Current == Cursors.SizeNWSE)
                EnforeAspectRatio();
        }

        private void RewireMouseMove()
        {
            foreach (Control control in Controls)
            {
                control.MouseMove -= RedirectMouseMove;
                control.MouseMove += RedirectMouseMove;
            }


            this.MouseMove -= Container_MouseMove;
            this.MouseMove += Container_MouseMove;
        }

        private void Container_KeyUp(object sender, KeyEventArgs e)
        {
            if (requestLoadClipboard)
            {

                if (this.BackgroundImage == null)
                    LoadFromClipboard();
                else // if (!pnDrag.Visible)
                {
                    var cont = Main.AddNewContainer();
                    cont.LoadFromClipboard();
                }

                requestLoadClipboard = false;
            }

        }

        private void pnDrag_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var formats = e.Data.GetFormats();
                e.Effect = DragDropEffects.Move;

                string[] fd = (string[])e.Data.GetData(DataFormats.FileDrop); //file drop

                if (fd != null && fd.Length > 0)
                {
                    bool first = (this.BackgroundImage == null);

                    foreach (var f in fd)
                    {
                        if (first)
                        {
                            loadImage(f);
                            first = !first;
                        }
                        else // if !first
                        {
                            var cont = Main.AddNewContainer();
                            cont.loadImage(f);
                        }
                    }
                    return;
                }

                string td = e.Data.GetData(DataFormats.Text)?.ToString(); //text drop

                if (td != null && td.ToUpper().Contains("HTTP"))
                {
                    var fileData = new WebClient().DownloadData(td);
                    loadImage(fileData);

                    if(imgData == null)
                    {
                        loadWeb(td);
                    }

                    return;
                }

            }
            catch { } //bugs dont exist
        }

        private void Container_ResizeBegin(object sender, EventArgs e)
        {
            hideImage();
            this.BackColor = Color.LightGray;

            if(!ScramblerActive)
            this.Opacity = 0.5;
        }

        public bool destroyOnClose = false;
        private void Container_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ScramblerActive && !ScramblerTarget)
                Main.containers.FirstOrDefault(it => it.ScramblerTarget)?.Close();

            if(destroyOnClose || e.CloseReason == CloseReason.UserClosing)
                SaveManager.Destroy(this);
        }

        private void Container_ResizeEnd(object sender, EventArgs e)
        {
            showImage();
            this.BackColor = Color.FromArgb(32, 32, 32);
            this.Opacity = 1;

            SaveManager.Update(this);

            //EnforeAspectRatio();
        }

        public void EnforeAspectRatio(bool force = false)
        {
            if (imgData == null)
                return;

            if (force || AspectRatio)
            {
                var reqHeight = (imgData.Height * this.Width) / imgData.Width;
                var reqWidth = (imgData.Width * this.Height) / imgData.Height;

                Size sizeA = new Size(this.Width, reqHeight);
                Size sizeB = new Size(reqWidth, this.Height);

                if (sizeA.Width*sizeA.Height < sizeB.Width*sizeB.Height)
                    this.Size = sizeA;
                else
                    this.Size = sizeB;
                
            }
        }

        private void pnContainer_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        public void LoadFromClipboard()
        {

            Image img = Clipboard.GetImage();
            if (img != null)
            {
                loadImage(img);
                return;
            }

            string text = Clipboard.GetText(); //will be blank if containing image data
            if (!string.IsNullOrWhiteSpace(text) && text.ToUpper().Contains("HTTP"))
            {
                try
                {
                    var fileData = new WebClient().DownloadData(text);
                    loadImage(fileData);
                }
                catch { }   //bugs don't exist

                if(imgData == null)
                {
                    loadWeb(text);
                }

                return;
            }

            IDataObject data_object = Clipboard.GetDataObject();
            if (data_object != null) //attempt reading file from disk
            {
                // Look for a file drop.
                if (data_object.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] files = (string[])
                        data_object.GetData(DataFormats.FileDrop);

                    bool first = true;
                    foreach (string file_name in files)
                    {
                        try
                        {
                            if (first)
                            {
                                first = false;
                                loadImage(file_name);
                            }
                            else
                            {
                                var cont = Main.AddNewContainer();
                                cont.loadImage(file_name);
                            }
                        }
                        catch { } //bugs don't exist
                        
                    }
                    return;
                }
            }

            //if nothing could be handled, close the form.
            CloseContainer();

        }

        public void loadWeb(string text, bool reloadWeb = false)
        {
            Web = text;

            if (!reloadWeb)
            {
                Width = 960;
                Height = 600;
            }

            pnDrag.BackgroundImage = null;
            browser = new ChromiumWebBrowser(text);

            browser.Location = new Point(0, 0);
            browser.Width = 960;
            browser.Height = 600;

            pnDrag.Padding = new Padding(16, 16, 16, 16);

            browser.Visible = false;
            pnDrag.Controls.Add(browser);
            Dock = DockStyle.Fill;


            browser.KeyboardHandler = new CustomKeyboardHandler();
            browser.MenuHandler = new CustomMenuHandler();
            browser.Visible = true;

            browser.Show();

            if (reloadWeb)
            {
                WindowState = FormWindowState.Minimized;
                Show();
                WindowState = FormWindowState.Normal;
                BringToFront();
                Focus();
            }
            else
                SaveManager.Update(this);
        }

        private void CloseContainer()
        {
            destroyOnClose = true;
            Main.containers.Remove(this);
            Close();
        }

        bool requestLoadClipboard = false;


        private void Container_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.V && e.Control)
                requestLoadClipboard = true;

            if (e.KeyCode == Keys.Escape && !Locked)
                CloseContainer();
        }

        private void Container_MouseDown(object sender, MouseEventArgs e)
        {
            int mousePosX = e.X + this.Location.X;
            int mousePosY = e.Y + this.Location.Y;

            if (e.Button == MouseButtons.Right)
            {
                ContextMenuStrip cms = new ContextMenuStrip();

                var top = (cms.Items.Add("Always on top", null, (ob, ev) =>
                {
                    TopMost = !TopMost;
                }) as ToolStripMenuItem);
                top.Checked = TopMost;
                top.Enabled = !Locked;

                var paste = (cms.Items.Add("Paste from Clipboard", null, (ob, ev) =>
                {
                    Main.AddNewContainer().LoadFromClipboard();
                }) as ToolStripMenuItem);

                cms.Items.Add(new ToolStripSeparator());

                var norm = (cms.Items.Add("Normalize image size", null, (ob, ev) =>
                {
                    if (imgData != null)
                    {
                        NormalizeImageSize();

                        int newPosX = mousePosX - (this.Size.Width / 2);
                        int newPosY = mousePosY - (this.Size.Height / 2);

                        this.Location = new Point(newPosX, newPosY);
                    }
                }) as ToolStripMenuItem);
                norm.Enabled = !Locked;

                var max = (cms.Items.Add("100% image size", null, (ob, ev) =>
                {
                    if (imgData != null)
                    {
                        this.Size = imgData.Size;

                        int newPosX = mousePosX - (this.Size.Width / 2);
                        int newPosY = mousePosY - (this.Size.Height / 2);

                        this.Location = new Point(newPosX, newPosY);
                    }

                }) as ToolStripMenuItem);
                max.Enabled = !Locked;

                var trans = (cms.Items.Add("Set Transparent Background", null, (ob, ev) =>
                {
                    var cd = new ColorDialog();
                    cd.Color = Color.FromArgb(255,0,255);

                    if (cd.ShowDialog() == DialogResult.OK)
                        this.TransparencyKey = cd.Color;

                }) as ToolStripMenuItem);
                trans.Enabled = !Locked;

                cms.Items.Add(new ToolStripSeparator());

                var locked = (cms.Items.Add("Lock floatie", null, (ob, ev) =>
                {
                    Locked = !Locked;

                }) as ToolStripMenuItem);
                locked.Checked = Locked;

                var aspect = (cms.Items.Add("Fixed Aspect ratio", null, (ob, ev) =>
                {
                    AspectRatio = !AspectRatio;

                    if(AspectRatio)
                        EnforeAspectRatio();

                }) as ToolStripMenuItem);
                aspect.Checked = AspectRatio;
                aspect.Enabled = !Locked;

                cms.Items.Add(new ToolStripSeparator());

                var close = (cms.Items.Add("Close (ESC)", null, (ob, ev) =>
                {
                    CloseContainer();
                }) as ToolStripMenuItem);
                close.Enabled = !Locked;

                cms.Show((Control)sender, e.Location);
            }
        }

    }
}
