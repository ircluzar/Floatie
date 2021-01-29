using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Floatie
{
    public abstract class Content
    {
        public abstract Image imgData { get; set; }

        public abstract Container cont { get; set; }

        public abstract void ShowContent();
        public abstract void HideContent();

        public abstract void Close();

    }

}
