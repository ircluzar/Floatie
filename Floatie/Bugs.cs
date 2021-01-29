using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Floatie
{
    public class Bugs
    {
        public static bool DenyExistanceOfBugs = true;

        public static void Exist(Exception ex)
        {
            if (DenyExistanceOfBugs)
                return;

            string except = ex.ToString();
            string inner = "";
            if (ex.InnerException != null)
                inner = ex.InnerException.ToString();

            MessageBox.Show($"{except}\n\n{inner}", "ow", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
