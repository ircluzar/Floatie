using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TinyFloatie
{
    static class Program
    {
        public static string TinyFloatieExe = System.Reflection.Assembly.GetEntryAssembly().Location;

        public static string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public static string FloatiePath = Path.Combine(appdata, "Floatie");
        public static string SavePath = Path.Combine(appdata, "Floatie", "SaveData");
        public static string ProgramPath = Path.Combine(appdata, "Floatie", "Bin");
        public static string TempPath = Path.Combine(appdata, "Floatie", "Temp");
        public static string FloatieExePath = Path.Combine(ProgramPath, "FloatieApp.exe");
        public static string VersionFilePath = Path.Combine(ProgramPath, "version.txt");

        //public static string packageUrl = "http://redscientist.com/software/floatie/update.zip";
        public static string packageUrl = "http://cc.r5x.cc/floatie/update.zip";

        //public static string versionUrl = "http://redscientist.com/software/floatie/version.txt";
        public static string versionUrl = "http://cc.r5x.cc/floatie/version.txt";

        public static string packageTempPath = Path.Combine(Program.TempPath, "update.zip");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            bool doUpdate = false;

            try
            {
                if (args.Length > 0 && args[0] != null && args[0].ToUpper().Contains("UPDATE"))
                {
                    int localversion = 0;

                    if (File.Exists(VersionFilePath))
                        localversion = Convert.ToInt32(File.ReadAllText(VersionFilePath));

                    int onlineversion = Convert.ToInt32(new WebClient().DownloadString(Program.versionUrl));

                    if (onlineversion > localversion)
                        doUpdate = true;
                    else
                    {
                        MessageBox.Show($"Already up to date (v{localversion})");
                        Environment.Exit(0);
                        return;
                    }
                }
            }
            catch(Exception ex) 
            { 
                _ = ex; 
            }


            //if(Debugger.IsAttached)
            if(!doUpdate && File.Exists(FloatieExePath))
            {
                var psi = new ProcessStartInfo();
                psi.FileName = FloatieExePath;
                psi.Arguments = TinyFloatieExe;
                psi.WorkingDirectory = ProgramPath;

                Process.Start(psi);
            }
            else
            {
                var psi = new ProcessStartInfo();
                psi.FileName = "taskkill";
                psi.Arguments = "/F /IM \"FloatieApp.exe\"";
                //psi.WorkingDirectory = ProgramPath;

                var p = Process.Start(psi);
                p.WaitForExit();

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Downloader());
            }

        }
    }
}
