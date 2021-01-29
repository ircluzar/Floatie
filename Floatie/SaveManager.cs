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
        public string TextData { get; set; }
        public Color? ImageColorKey { get; set; }
        public Color? TextColor { get; set; }

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
            TextData = cont.TextData;
            TextColor = cont.TextColor;
            ImageColorKey = cont.ImageColorKey;
        }

        public void ReloadMeta(Container cont)
        {
            cont.ID = ID;
            cont.Size = size;
            cont.Location = point;
            cont.Locked = Locked;
            cont.AspectRatio = AspectRatio;
            cont.TopMost = TopMost;
            cont.TextData = TextData;
            cont.TextColor = TextColor;
            cont.ImageColorKey = ImageColorKey;

            if (cont.ImageColorKey != null && cont.content != null && cont.content.imgData != null)
            {
                cont.TransparencyKey = ImageColorKey.Value;
                if (ImageColorKey == Color.FromArgb(255, 0, 255))
                    cont.BackColor = ImageColorKey.Value;
            }
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
            catch(Exception ex) { Bugs.Exist(ex); } //bugs don't exist
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
            catch(Exception ex) { Bugs.Exist(ex); } //bugs don't exist
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
        public static string SavePath = Path.Combine(appdata,"Floatie","SaveData");
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

            var sm = SaveMeta.Load(metaPath);
            sm?.ReloadMeta(cont);
            SaveManager.LoadData(dataPath, cont);
            sm?.ReloadMeta(cont);

            if (sm == null || cont.TextData == null && (cont.content == null || (cont.content != null && cont.content.imgData == null)))
            {
                SaveManager.Destroy(file.Name.Substring(0, file.Name.IndexOf('.')));
                cont.destroyOnClose = true;
                cont.Close();
                return;
            }

            if (cont.TextData != null)
            {
                if (cont.TextData.ToUpper().StartsWith("HTTP"))
                {
                    cont.loadWeb(cont.TextData, true);
                    sm.ReloadMeta(cont);
                }
                else
                {
                    cont.LoadText(cont.TextData);
                    sm.ReloadMeta(cont);
                }
            }

        }

        public static void SetData(Container cont, Content content)
        {
            SaveData(content);
            SaveMetaData(cont);
        }

        public static void Update(Container cont)
        {
            SaveMetaData(cont);
        }

        public static void Destroy(string ID)
        {

            string metaFile = $"{ID}.meta";
            string metaPath = Path.Combine(SavePath, metaFile);

            string dataFile = $"{ID}.dat";
            string dataPath = Path.Combine(SavePath, dataFile);

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
                catch(Exception ex) { Bugs.Exist(ex); } //bugs don't exist

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



        private static void SaveData(Content content)
        {
            if (content == null)
                throw new Exception("You passed the container's content before the content was assigned to the value");


            try
            {
                if (content.imgData == null)
                    return;

                string dataFile = $"{content.cont.ID}.dat";
                string dataPath = Path.Combine(SavePath, dataFile);

                if (content.cont.CensorActive || content.cont.ScramblerActive)
                {
                    if (File.Exists(dataPath))
                        File.Delete(dataPath);
                }
                else
                    content.imgData.Save(dataPath);
            }
            catch(Exception ex) { Bugs.Exist(ex); } //bugs don't exist
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

                cont.LoadImage(img, true);

            }
            catch(Exception ex) { Bugs.Exist(ex); } //bugs don't exist
        }
    }
}
