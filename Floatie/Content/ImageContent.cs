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
    public class ImageContent : Content
    {
        public override Image imgData { get; set; }
        public override Container cont { get; set; }
        public ImageContent(Container _cont, string path)
        {
            cont = _cont;
            cont.content = this;

            cont.Hide();

            try
            {
                Image img = Image.FromFile(path);

                imgData = img;
                displayImage();
            }
            catch { } //bugs don't exist
        }

        public ImageContent(Container _cont, byte[] data)
        {
            cont = _cont;
            cont.content = this;

            cont.Hide();

            try
            {
                MemoryStream stream = new MemoryStream(data);
                imgData = Image.FromStream(stream);
                displayImage();
            }
            catch { } //bugs don't exist
        }

        public ImageContent(Container _cont, Image img, bool isLoadingContainer)
        {
            cont = _cont;
            cont.content = this;

            cont.Hide();

            imgData = img;
            displayImage(isLoadingContainer);
        }

        public override void ShowContent()
        {
            cont.BackgroundImage = imgData;
            cont.BackgroundImageLayout = ImageLayout.Stretch;
        }

        public override void HideContent()
        {

            cont.BackgroundImage = null;
            cont.BackgroundImageLayout = ImageLayout.Stretch;
        }

        public void displayImage(bool isLoadingContainer = false)
        {
            //cont.Hide();

            cont.pnDrag.BackgroundImage = null;
            cont.BackColor = Color.Black;

            cont.BackgroundImage = imgData;
            cont.BackgroundImageLayout = ImageLayout.Stretch;
            cont.pnDrag.BorderStyle = BorderStyle.None;

            if (!isLoadingContainer)
            {
                cont.Width = imgData.Width;
                cont.Height = imgData.Height;

                var scrBounds = Screen.FromControl(cont).Bounds;

                if (imgData.Width >= scrBounds.Width || imgData.Height >= scrBounds.Height)
                {
                    NormalizeImageSize(cont, Convert.ToInt32(scrBounds.Height * 0.8));

                    int newLocX = (scrBounds.Width / 2) - (cont.Width / 2);
                    int newLocY = (scrBounds.Height / 2) - (cont.Height / 2);
                    cont.Location = new Point(newLocX, newLocY);
                }
            }

            if (!isLoadingContainer)
                SaveManager.SetData(cont, this);


            cont.Show();
            cont.BringToFront();
            cont.Focus();

        }

        public void NormalizeImageSize(Container cont, int normalSize = 666)
        {
            cont.Size = new Size(normalSize, normalSize);
            cont.EnforeAspectRatio(true);
        }


    }


}
