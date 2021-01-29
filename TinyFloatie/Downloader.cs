using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TinyFloatie
{
    public partial class Downloader : Form
    {
        Exception lastException = null;
        public Downloader()
        {
            InitializeComponent();

        }

        public void CheckFolders()
        {
            if (!Directory.Exists(Program.FloatiePath))
                Directory.CreateDirectory(Program.FloatiePath);

            if (!Directory.Exists(Program.SavePath))
                Directory.CreateDirectory(Program.SavePath);

            if (!Directory.Exists(Program.ProgramPath))
                Directory.CreateDirectory(Program.ProgramPath);

            if (!Directory.Exists(Program.TempPath))
                Directory.CreateDirectory(Program.TempPath);

        }

        public void DownloadInstallPackage()
        {

            try
            {
                using (WebClient client = new WebClient())
                {

                    if (File.Exists(Program.packageTempPath))
                        File.Delete(Program.packageTempPath);


                    client.DownloadFileCompleted += Client_DownloadFileCompleted;
                    client.DownloadProgressChanged += Client_DownloadProgressChanged;
                    client.DownloadFileAsync(new Uri(Program.packageUrl), Program.packageTempPath);

                }

            }
            catch (Exception ex)
            {
                lastException = ex;
                return;
            }

        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            pbExpand.Maximum = (int)e.TotalBytesToReceive;
            pbExpand.Value = (int)e.BytesReceived;

            string downloadSize = $"{String.Format("{0:0.##}", (e.BytesReceived / 1000000f))}MB / {String.Format("{0:0.##}", (e.TotalBytesToReceive / 1000000f))}MB";
            lbBytes.Text = downloadSize;
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            lbBytes.Visible = false;

            Exception error = null;

            string packageTempPath = Path.Combine(Program.TempPath, "update.zip");

            if (e.Error == null)
            {
                try
                {

                    lbFloatieText.Text = "Extracting Files... Please wait";
                    this.Refresh();


                    using (ZipArchive archive = ZipFile.OpenRead(packageTempPath))
                    {
                        var allEntries = archive.Entries.ToArray();

                        pbExpand.Maximum = allEntries.Length;

                        for (int i=0;i<allEntries.Length;i++)
                        //foreach (var entry in archive.Entries)
                        {
                            var entry = allEntries[i];

                            var entryPath = Path.Combine(Program.ProgramPath, entry.FullName).Replace("/", "\\");

                            if (entryPath.EndsWith("\\"))
                            {
                                if (!Directory.Exists(entryPath))
                                    Directory.CreateDirectory(entryPath);
                            }
                            else
                            {
                                entry.ExtractToFile(entryPath, true);
                            }

                            pbExpand.Value = i;
                        }
                    }

                    //ZipFile.ExtractToDirectory(packageTempPath, Program.ProgramPath);

                    if (File.Exists(packageTempPath))
                        File.Delete(packageTempPath);


                    var rootFiles = Directory.GetFiles(Program.FloatiePath);
                    foreach (var file in rootFiles)
                    {
                        var fi = new FileInfo(file);
                        string movePath = Path.Combine(Program.SavePath, fi.Name);
                        File.Move(file, movePath);
                    }


                    var psi = new ProcessStartInfo();
                    psi.FileName = Program.FloatieExePath;
                    psi.Arguments = Program.TinyFloatieExe;
                    psi.WorkingDirectory = Program.ProgramPath;

                    Process.Start(psi);

                }
                catch(Exception ex)
                {
                    error = ex;
                }
            }

            if (e.Error != null)
                error = e.Error;

            if (error != null)
            {
                var dr = MessageBox.Show("Oops, didn't work.\nWanna know more about it?", "Sorry", MessageBoxButtons.YesNoCancel);

                if (dr == DialogResult.Yes)
                    MessageBox.Show(error?.ToString() ?? "uhhh i don't know");

            }


            Application.Exit();
            Environment.Exit(0);
        }

        private void Downloader_Load(object sender, EventArgs e)
        {
            this.Show();

            CheckFolders();
            DownloadInstallPackage();

        }
    }
}
