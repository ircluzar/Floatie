using CefSharp;
using CefSharp.WinForms;
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

    #region Web objects for ChromiumWebBrowser
    public class FloatieWebHelper
    {
        public static Form SyncObject;
        public static void FormExecute(Action<object, EventArgs> a, object[] args = null)
        {
            if (SyncObject.InvokeRequired)
                SyncObject.Invoke(new MethodInvoker(() => { a.Invoke(null, null); }));
            else
                a.Invoke(null, null);
        }
    }
    public class CustomKeyboardHandler : CefSharp.IKeyboardHandler
    {
        public bool OnKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey)
        {
            return false;
        }

        public bool OnPreKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey, ref bool isKeyboardShortcut)
        {
            if (windowsKeyCode == 27) //Escape
            {
                FloatieWebHelper.FormExecute((o, ea) =>
                {
                    ((browserControl as ChromiumWebBrowser).Parent.Parent as Form)?.Close();
                });
            }

            if (windowsKeyCode == 123) //F12
            {
                browser.ShowDevTools();
            }

            if (windowsKeyCode == 13 && modifiers == CefEventFlags.AltDown) //Alt+Enter
            {
                //VRUN_RunForm.ToggleFullscreen();
                return true;
            }

            return false;
        }
    }
    public class CustomMenuHandler : CefSharp.IContextMenuHandler
    {
        public void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
        {
            model.Clear();
        }

        public bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
        {

            return false;
        }

        public void OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        {

        }

        public bool RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
        {
            return false;
        }
    }
    #endregion

}
