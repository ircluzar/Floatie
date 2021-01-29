using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
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
        public Content content = null;


        public Image imgDropPic = ((System.Drawing.Icon)(new System.ComponentModel.ComponentResourceManager(typeof(Container)).GetObject("$this.Icon"))).ToBitmap();

        System.Drawing.Point lastKnownMouseLocation = new Point(0, 0);
        bool ImageJustLoaded = true;

        public Color? ImageColorKey = null;
        public Color? TextColor = null;

        public bool Locked = false;
        public bool AspectRatio = true;
        public bool ScramblerActive = false;
        public bool CensorActive = false;
        public string TextData = null;

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


        public Timer ViewDisplayTimer = new Timer();


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

            ViewDisplayTimer.Interval = 1234;
            ViewDisplayTimer.Tick += ViewDisplayTimer_Tick;
        }

        public void InitContainer(string path = null)
        {
            RewireMouseMove();
            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.None;

            if (path != null)
                LoadImage(path);
            else
                pnDrag.BackgroundImage = imgDropPic;

            ViewDisplayTimer.Interval = 666;
            ViewDisplayTimer.Tick += ViewDisplayTimer_Tick;
        }


        public void ViewDisplayTimer_Reload()
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

        public void LoadImage(Image img, bool isLoadingContainer = false)
        {
            if (content == null)
                content = new ImageContent(this, img, isLoadingContainer);
        }
        public void LoadImage(byte[] data)
        {
            if (content == null)
                content = new ImageContent(this, data);
        }

        public void LoadFile(string path)
        {
            try
            {

                var ext = path.Substring(path.LastIndexOf('.') + 1).ToUpper().Trim();

                if (ext == "MP4" || ext == "WMV" || ext == "WEBM" || ext == "AVI" || ext == "MKV" || ext == "WAV" || ext == "MP3" || ext == "OGG" || ext == "OGV")
                    LoadMedia(path);
                if (ext == "TXT" || ext == "JSON" || ext == "XML" || ext == "NFO" || ext == "INI")
                    LoadText(File.ReadAllText(path));
                else
                    LoadImage(path);

            }
            catch(Exception ex) { Bugs.Exist(ex); } //bugs don't exist
        }
        public void LoadImage(string path)
        {
            if (content == null)
                content = new ImageContent(this,path);
        }


        public void LoadCensor()
        {
            if (content == null)
                content = new CensorContent(this);
        }

        public void LoadColor()
        {
            if (content == null)
                content = new ColorContent(this);
        }
        public void LoadMedia(string text = "")
        {
            if (content == null)
                content = new MediaContent(this, text);
        }
        public void LoadText(string text = "")
        {
            if (content == null)
                content = new TextContent(this, text);
        }

        public void LoadCapture()
        {
            if (content == null)
                content = new CaptureContent(this);
        }

        public void LoadScrambler()
        {
            if (content == null)
                content = new ScramblerContent(this);
        }

        public void LoadAi()
        {
            if (content == null)
                content = new AiContent(this);
        }

        internal void LoadScramblerTarget()
        {
            if (content == null)
                content = new ScramblerTargetContent(this);
        }

        public void loadWeb(string text, bool reloadWeb = false)
        {
            if (content == null)
            {
                content = new WebContent(this, text, reloadWeb);
                TextData = text;
            }
        }

        public void ShowContent() => content?.ShowContent();
        public void HideContent() => content?.HideContent();





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
        public void RedirectMouseMove(object sender, MouseEventArgs e)
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

            //Todo, refactor into a flag for noresize
            if(content is AiContent)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (content != null && content is TextContent tc)
                    {
                        tc.tbText.DeselectAll();
                        tc.tbText.ReadOnly = true;
                        tc.tbText.ReadOnly = false;
                    }

                    ReleaseCapture();
                    SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }

                return;
            }



            if (!Locked)
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
                    if(content != null && content is TextContent tc)
                    {
                        tc.tbText.DeselectAll();
                        tc.tbText.ReadOnly = true;
                        tc.tbText.ReadOnly = false;
                    }

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
                else
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
                            LoadFile(f);
                            first = !first;
                        }
                        else
                        {
                            var cont = Main.AddNewContainer();
                            cont.LoadFile(f);
                        }
                    }
                    return;
                }

                string td = e.Data.GetData(DataFormats.Text)?.ToString(); //text drop

                if (td != null)
                {
                    if (td.ToUpper().StartsWith("HTTP"))
                    {
                        var fileData = new WebClient().DownloadData(td);
                        LoadImage(fileData);

                        if (content.imgData == null)
                        {
                            loadWeb(td);
                            return;
                        }
                    }
                    else
                    {
                        LoadText(td);
                        return;
                    }
                    
                }

            }
            catch(Exception ex) { Bugs.Exist(ex); } //bugs dont exist
        }

        public void EnforeAspectRatio(bool force = false)
        {
            if (content == null || content.imgData == null)
                return;

            if (force || AspectRatio)
            {
                var reqHeight = Convert.ToInt32((Convert.ToDecimal(content.imgData.Height) * Convert.ToDecimal(this.Width)) / Convert.ToDecimal(content.imgData.Width));
                var complementWidth = Convert.ToInt32((Convert.ToDecimal(reqHeight) * Convert.ToDecimal(content.imgData.Width)) / Convert.ToDecimal(content.imgData.Height));

                var reqWidth = Convert.ToInt32((Convert.ToDecimal(content.imgData.Width) * Convert.ToDecimal(this.Height)) / Convert.ToDecimal(content.imgData.Height));
                var complementheight = Convert.ToInt32((Convert.ToDecimal(reqWidth) * Convert.ToDecimal(content.imgData.Height)) / Convert.ToDecimal(content.imgData.Width));

                Size sizeA = new Size(complementWidth, reqHeight);
                Size sizeB = new Size(reqWidth, complementheight);

                //Size sizeA = new Size(this.Width, reqHeight);
                //Size sizeB = new Size(reqWidth, this.Height);

                if (sizeA.Width < sizeB.Width ||sizeA.Height < sizeB.Height)
                    this.Size = sizeA;
                else
                    this.Size = sizeB;

            }
        }

        private void Container_ResizeBegin(object sender, EventArgs e)
        {
            if (content != null && (content is ScramblerContent || content is CaptureContent))
                return;

            HideContent();

            this.BackColor = Color.LightGray;
            this.Opacity = 0.5;
        }

        public bool destroyOnClose = false;


        private void Container_FormClosing(object sender, FormClosingEventArgs e)
        {

            content?.Close();

            if (content != null && content is ScramblerContent sc)
                Main.containers.FirstOrDefault(it => it.content != null & it.content is ScramblerTargetContent)?.Close();

            if(destroyOnClose || e.CloseReason == CloseReason.UserClosing)
                SaveManager.Destroy(ID);
        }

        private void Container_ResizeEnd(object sender, EventArgs e)
        {
            if (content != null && (content is ScramblerContent || content is CaptureContent))
                return;

            if(ImageColorKey != null)
                this.BackColor = ImageColorKey.Value;
            else
                this.BackColor = Color.FromArgb(32, 32, 32);

            this.Opacity = 1;

            ShowContent();


            SaveManager.Update(this);
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
                LoadImage(img);
                return;
            }

            string text = Clipboard.GetText(); //will be blank if containing image data
            if (!string.IsNullOrWhiteSpace(text))
            {
                if (text.ToUpper().StartsWith("HTTP"))
                {

                    try
                    {
                        var fileData = new WebClient().DownloadData(text);
                        LoadImage(fileData);
                    }
                    catch(Exception ex) { Bugs.Exist(ex); } //bugs don't exist

                    if (content != null && content.imgData == null)
                    {
                        content = null;
                        loadWeb(text);
                    }
                }
                else
                {
                    LoadText(text);
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
                                LoadImage(file_name);
                            }
                            else
                            {
                                var cont = Main.AddNewContainer();
                                cont.LoadImage(file_name);
                            }
                        }
                        catch(Exception ex) { Bugs.Exist(ex); } //bugs don't exist
                        
                    }
                    return;
                }
            }

            //if nothing could be handled, close the form.
            CloseContainer();

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
            if (e.KeyCode == Keys.V && e.Control) //CTRL+V Calls for loading whatever from clipboard
                requestLoadClipboard = true;

            if (e.KeyCode == Keys.C && e.Control) //CTRL+C Sends image to clipboard
            {
                if (content != null && content.imgData != null)
                {
                    Clipboard.SetImage(content.imgData);
                }
            }

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

                var paste = (cms.Items.Add("New floatie from clipboard", null, (ob, ev) =>
                {
                    Main.AddNewContainer().LoadFromClipboard();
                }) as ToolStripMenuItem);

                if (content != null && content is TextContent)
                {
                    cms.Items.Add(new ToolStripSeparator());

                    var txtContent = (TextContent)content;

                    var vert = (cms.Items.Add("Show vertical scroll bar", null, (ob, ev) =>
                    {
                        if (txtContent.tbText.ScrollBars == ScrollBars.Vertical)
                            txtContent.tbText.ScrollBars = ScrollBars.None;
                        else
                            txtContent.tbText.ScrollBars = ScrollBars.Vertical;

                    }) as ToolStripMenuItem);
                    vert.Enabled = !Locked;
                    vert.Checked = (txtContent.tbText.ScrollBars == ScrollBars.Vertical);

                    var col = (cms.Items.Add("Change background color", null, (ob, ev) =>
                    {
                        var cd = new ColorDialog();
                        cd.Color = Extensions.GetRandomPastelColor();

                        if (cd.ShowDialog() == DialogResult.OK)
                        {
                            TextColor = cd.Color;
                            txtContent.tbText.BackColor = TextColor.Value;
                            SaveManager.Update(this);
                        }

                    }) as ToolStripMenuItem);
                    col.Enabled = !Locked;
                }

                if (content != null && content is AiContent)
                {
                    cms.Items.Add(new ToolStripSeparator());

                    var neutral = (cms.Items.Add("Neutral behavior", null, (ob, ev) =>
                    {
                        AskGPT2_117M.behavior = BotColoring.NEUTRAL;
                    }) as ToolStripMenuItem);
                    neutral.Checked = AskGPT2_117M.behavior == BotColoring.NEUTRAL;

                    var happy = (cms.Items.Add("Happy behavior", null, (ob, ev) =>
                    {
                        AskGPT2_117M.behavior = BotColoring.HAPPY;
                    }) as ToolStripMenuItem);
                    happy.Checked = AskGPT2_117M.behavior == BotColoring.HAPPY;

                    var toxic = (cms.Items.Add("Toxic behavior", null, (ob, ev) =>
                    {
                        AskGPT2_117M.behavior = BotColoring.TOXIC;
                    }) as ToolStripMenuItem);
                    toxic.Checked = AskGPT2_117M.behavior == BotColoring.TOXIC;
                }

                if (content != null && content is ImageContent)
                {
                    var imgContent = (ImageContent)content;


                    cms.Items.Add(new ToolStripSeparator());

                    var copclip = (cms.Items.Add("Copy image to clipboard", null, (ob, ev) =>
                    {
                        if(content != null && content.imgData != null)
                            Clipboard.SetImage(content.imgData);
                    }) as ToolStripMenuItem);

                    var simas = (cms.Items.Add("Save image as..", null, (ob, ev) =>
                    {
                        if (content != null && content.imgData != null)
                        {


                            string filename;
                            SaveFileDialog saveFileDialog1 = new SaveFileDialog
                            {
                                DefaultExt = "png",
                                Title = "PNG File",
                                Filter = "PNG files|*.png",
                                RestoreDirectory = true
                            };
                            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                            {
                                filename = saveFileDialog1.FileName;
                            }
                            else
                                return;

                            using (var fs = new FileStream(filename, FileMode.OpenOrCreate))
                                content.imgData.Save(fs, ImageFormat.Png);

                        }

                    }) as ToolStripMenuItem);

                    cms.Items.Add(new ToolStripSeparator());


                    var norm = (cms.Items.Add("Normalize image size", null, (ob, ev) =>
                    {
                        if (content.imgData != null)
                        {
                            imgContent.NormalizeImageSize(this);

                            int newPosX = mousePosX - (this.Size.Width / 2);
                            int newPosY = mousePosY - (this.Size.Height / 2);

                            this.Location = new Point(newPosX, newPosY);
                        }
                    }) as ToolStripMenuItem);
                    norm.Enabled = !Locked;
                

                    var max = (cms.Items.Add("100% image size", null, (ob, ev) =>
                    {
                        if (imgContent.imgData != null)
                        {
                            this.Size = imgContent.imgData.Size;

                            int newPosX = mousePosX - (this.Size.Width / 2);
                            int newPosY = mousePosY - (this.Size.Height / 2);

                            this.Location = new Point(newPosX, newPosY);
                        }

                    }) as ToolStripMenuItem);
                    max.Enabled = !Locked;

                    var trans = (cms.Items.Add("Set transparent background", null, (ob, ev) =>
                    {
                        var cd = new ColorDialog();
                        cd.Color = Color.FromArgb(0, 0, 0);

                        if (cd.ShowDialog() == DialogResult.OK)
                        {
                            if(cd.Color == Color.FromArgb(0, 0, 0))
                            {
                                Hide();
                                BackColor = Color.FromArgb(0, 0, 0);
                                cd.Color = Color.FromArgb(0, 0, 0);
                                
                            }


                            ImageColorKey = cd.Color;
                            this.TransparencyKey = ImageColorKey.Value;

                            Show();


                            SaveManager.Update(this);
                        }

                    }) as ToolStripMenuItem);
                    trans.Enabled = !Locked;

                }

                cms.Items.Add(new ToolStripSeparator());

                var locked = (cms.Items.Add("Lock floatie", null, (ob, ev) =>
                {
                    Locked = !Locked;

                }) as ToolStripMenuItem);
                locked.Checked = Locked;

                if (content != null && content is ImageContent)
                {
                    var aspect = (cms.Items.Add("Fixed aspect ratio", null, (ob, ev) =>
                    {
                        AspectRatio = !AspectRatio;

                        if (AspectRatio)
                            EnforeAspectRatio();

                    }) as ToolStripMenuItem);
                    aspect.Checked = AspectRatio;
                    aspect.Enabled = !Locked;
                }

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
