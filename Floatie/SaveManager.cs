using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Floatie
{
    [Serializable]
    class SaveMeta
    {
        public string ID { get; set; }
        public Size size { get; set; }
        public Point point { get; set; }
        public bool Locked { get; set; }
        public bool AspectRatio { get; set; }
        public bool TopMost { get; set; }

        public string Web { get; set; }

        public SaveMeta()
        {

        }

        public SaveMeta(Container cont)
        {
            ID = cont.ID;
            size = cont.Size;
            point = cont.Location;
            Locked = cont.Locked;
            AspectRatio = cont.AspectRatio;
            TopMost = cont.TopMost;
            Web = cont.Web;
        }

        public void ReloadMeta(Container cont)
        {
            cont.ID = ID;
            cont.Size = size;
            cont.Location = point;
            cont.Locked = Locked;
            cont.AspectRatio = AspectRatio;
            cont.TopMost = TopMost;
            cont.Web = Web;
        }

        public void Save(string metaFile)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = null;
            try
            {
                stream = new FileStream(metaFile, FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, this);
                
            }
            catch { } //bugs don't exist
            finally
            {
                stream?.Close();
            }
        }

        public static SaveMeta Load(string metaFile)
        {
            SaveMeta sm = null;

            IFormatter formatter = new BinaryFormatter();
            Stream stream = null;
            try
            {
                stream = new FileStream(metaFile, FileMode.Open);
                sm = (SaveMeta)formatter.Deserialize(stream);
            }
            catch { } //bugs don't exist
            finally
            {
                stream?.Close();
            }

            return sm;
        }
    }

    class SaveManager
    {
        public static string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string SavePath = Path.Combine(appdata,"Floatie");
        //public static string SavePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static DirectoryInfo SaveDi = new DirectoryInfo(SavePath);

        public static void Init()
        {
            if (!SaveDi.Exists)
                SaveDi.Create();

            LoadFolder();

        }

        public static void LoadFolder(string savename = null)
        {
            string folderPath = SaveDi.FullName;

            if (savename != null)
                folderPath = Path.Combine(folderPath, savename);

            if (!Directory.Exists(folderPath))
                return;

            var files = SaveDi.GetFiles().Where(it => it.Extension == ".meta");

            foreach (var file in files)
                LoadContainer(file);
        }

        private static void LoadContainer(FileInfo file)
        {
            string metaPath = file.FullName;
            string dataPath = metaPath.Replace(".meta", ".dat");

            Container cont = Main.AddNewContainer(true);
            SaveManager.LoadData(dataPath, cont);

            var sm = SaveMeta.Load(metaPath);
            sm?.ReloadMeta(cont);

            if (sm == null || (cont.imgData == null && cont.Web == null))
            {
                SaveManager.Destroy(file.Name.Substring(0, file.Name.IndexOf('.')));
                cont.destroyOnClose = true;
                cont.Close();
                return;
            }

            if (cont.Web != null)
            {
                cont.loadWeb(cont.Web, true);
                sm.ReloadMeta(cont);
            }

        }

        public static void SetData(Container cont)
        {
            SaveData(cont);
            SaveMetaData(cont);
        }

        public static void Update(Container cont)
        {
            SaveMetaData(cont);
        }

        public static void Destroy(Container cont) => Destroy(cont.ID.ToString());
        public static void Destroy(string ID)
        {


            string metaFile = $"{ID}.meta";
            string metaPath = Path.Combine(SavePath, metaFile);

            string dataFile = $"{ID}.dat";
            string dataPath = Path.Combine(SavePath, dataFile);

            //cont.Close();

            //if (cont != null && !cont.IsDisposed)
            //    cont.Dispose();

            if (File.Exists(metaPath))
                File.Delete(metaPath);

            if (File.Exists(dataPath))
                File.Delete(dataPath);

        }

        public static void Reset()
        {
            foreach(var cont in Main.containers)
            {
                string metaFile = $"{cont.ID}.meta";
                string metaPath = Path.Combine(SavePath, metaFile);

                string dataFile = $"{cont.ID}.dat";
                string dataPath = Path.Combine(SavePath, dataFile);

                cont.Close();

                if (cont != null && !cont.IsDisposed)
                    cont.Dispose();

                if (File.Exists(metaPath))
                    File.Delete(metaPath);

                if (File.Exists(dataPath))
                    File.Delete(dataPath);
            }

            foreach(var file in SaveDi.GetFiles())
                try
                {
                    file.Delete();
                }
                catch { } //bugs don't exist

            Main.Restart();
        }

        private static void SaveMetaData(Container cont)
        {
            var meta = new SaveMeta(cont);
            string metaFile = $"{cont.ID}.meta";
            string metaPath = Path.Combine(SavePath, metaFile);

            if(cont.CensorActive || cont.ScramblerActive)
            {
                if (File.Exists(metaPath))
                    File.Delete(metaPath);
            }
            else
                meta.Save(metaPath);
        }



        private static void SaveData(Container cont)
        {
            try
            {
                if (cont.imgData == null)
                    return;

                string dataFile = $"{cont.ID}.dat";
                string dataPath = Path.Combine(SavePath, dataFile);

                if (cont.CensorActive || cont.ScramblerActive)
                {
                    if (File.Exists(dataPath))
                        File.Delete(dataPath);
                }
                else
                    cont.imgData.Save(dataPath);
            }
            catch { } //bugs don't exist
        }

        private static void LoadData(string dataPath, Container cont)
        {
            try
            {
                //var img = Image.FromFile(dataPath);

                Image img;
                using (var bmpTemp = new Bitmap(dataPath))
                {
                    img = new Bitmap(bmpTemp);
                }

                if (img == null)
                    return;

                cont.loadImage(img, true);
            }
            catch { } //bugs don't exist
        }
    }
}
