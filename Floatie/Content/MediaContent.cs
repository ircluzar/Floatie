using CefSharp.WinForms;
using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Floatie
{
    public class MediaContent : Content
    {
        public override Image imgData { get; set; }
        public override Container cont { get; set; }

        LibVLCSharp.WinForms.VideoView vlc;
        LibVLC lib;

        static bool vlcInitialized = false;
        public MediaContent(Container _cont, string path)
        {
            cont = _cont;

            if (!vlcInitialized)
            {
                LibVLCSharp.Shared.Core.Initialize();
                vlcInitialized = true;
            }


            cont.Width = 800;
            cont.Height = 480;

            cont.pnDrag.BackgroundImage = null;
            cont.pnDrag.Padding = new Padding(16, 16, 16, 16);

            vlc = new LibVLCSharp.WinForms.VideoView();
            lib = new LibVLC();
            vlc.MediaPlayer = new MediaPlayer(lib);

            vlc.BackColor = System.Drawing.Color.Black;
            vlc.Location = new System.Drawing.Point(0, 0);
            vlc.Name = "vlc";
            vlc.Size = new System.Drawing.Size(800, 480);
            vlc.TabIndex = 0;
            vlc.Text = "vlc";

            vlc.MouseMove -= cont.RedirectMouseMove;
            vlc.MouseMove += cont.RedirectMouseMove;

            vlc.KeyPress += Vlc_KeyPress;
            cont.KeyPress += Vlc_KeyPress;

            var mi = new StreamMediaInput(File.OpenRead(path));
            var m = new Media(lib,mi);
            vlc.MediaPlayer.Play(m);

            cont.pnDrag.Controls.Add(vlc);
            vlc.Dock = DockStyle.Fill;


            //SaveManager.Update(cont);
        }

        private void Vlc_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 32)
            {
                vlc.MediaPlayer.Pause();
            }

            if (e.KeyChar == 27)
            {
                cont.Close();
            }
        }

        public override void ShowContent()
        {

        }

        public override void HideContent()
        {

        }

        public override void Close()
        {
            vlc?.MediaPlayer?.Stop();
        }
    }


}
