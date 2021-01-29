using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Floatie
{
    public static class Extensions
    {
		/// <summary>
		/// Creates color with corrected brightness.
		/// </summary>
		/// <param name="color">Color to correct.</param>
		/// <param name="correctionFactor">The brightness correction factor. Must be between -1 and 1.
		/// Negative values produce darker colors.</param>
		/// <returns>
		/// Corrected <see cref="Color"/> structure.
		/// </returns>
		public static Color ChangeColorBrightness(this Color color, float correctionFactor)
		{
			float red = (float)color.R;
			float green = (float)color.G;
			float blue = (float)color.B;

			if (correctionFactor < 0)
			{
				correctionFactor = 1 + correctionFactor;
				red *= correctionFactor;
				green *= correctionFactor;
				blue *= correctionFactor;
			}
			else
			{
				red = (255 - red) * correctionFactor + red;
				green = (255 - green) * correctionFactor + green;
				blue = (255 - blue) * correctionFactor + blue;
			}

			return Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
		}

		public static DialogResult GetInputBox(string title, string promptText, ref string value)
		{
			Form form = new Form();
			Label label = new Label();
			TextBox textBox = new TextBox();
			Button buttonOk = new Button();
			Button buttonCancel = new Button();

			form.Text = title;
			label.Text = promptText;
			textBox.Text = value;
			//textBox.GotFocus += (o, e) => UICore.UpdateFormFocusStatus(false);
			//textBox.LostFocus += (o, e) => UICore.UpdateFormFocusStatus(false);

			buttonOk.Text = "OK";
			buttonCancel.Text = "Cancel";
			buttonOk.DialogResult = DialogResult.OK;
			buttonCancel.DialogResult = DialogResult.Cancel;

			label.SetBounds(9, 20, 372, 13);
			textBox.SetBounds(12, 36, 372, 20);
			buttonOk.SetBounds(228, 72, 75, 23);
			buttonCancel.SetBounds(309, 72, 75, 23);

			label.AutoSize = true;
			textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
			buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

			form.ClientSize = new Size(396, 107);
			form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
			form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
			form.FormBorderStyle = FormBorderStyle.FixedDialog;
			form.StartPosition = FormStartPosition.CenterScreen;
			form.MinimizeBox = false;
			form.MaximizeBox = false;
			form.AcceptButton = buttonOk;
			form.CancelButton = buttonCancel;
			form.Shown += (f, g) =>
			{
				form.TopMost = true;
				form.Focus();
				form.BringToFront();
			};
			DialogResult dialogResult = form.ShowDialog();
			value = textBox.Text;
			return dialogResult;
		}


		public static Color GetRandomPastelColor()
		{
			int r = Main.rnd.Next(70, 200);
			int g = Main.rnd.Next(100, 225);
			int b = Main.rnd.Next(100, 230);

			Color c = Color.FromArgb(r, g, b);

			c = c.ChangeColorBrightness(0.35f);

			return c;
		}

		public static Color GetRandomColor()
		{
			int r = Main.rnd.Next(70, 200);
			int g = Main.rnd.Next(100, 225);
			int b = Main.rnd.Next(100, 230);

			Color c = Color.FromArgb(r, g, b);

			//c = c.ChangeColorBrightness(0.35f);

			return c;
		}

		public static Color GetRandomAntiPastelColor()
        {
			byte r = Convert.ToByte(Main.rnd.Next(70, 200));
			byte g = Convert.ToByte(Main.rnd.Next(100, 225));
			byte b = Convert.ToByte(Main.rnd.Next(100, 230));

			Color c = Color.FromArgb(r, g, b);

			c = c.ChangeColorBrightness(0.35f);

			byte flip = 128;

            unchecked
			{
				r = (byte)(c.R + flip);
				g = (byte)(c.G + flip);
				b = (byte)(c.B + flip);
			}
			c = Color.FromArgb(r, g, b);

			return c;
		}
	}

	public static class SuspendUpdate
	{
		private const int WM_SETREDRAW = 0x000B;

		public static void Suspend(Control control)
		{
			Message msgSuspendUpdate = Message.Create(control.Handle, WM_SETREDRAW, IntPtr.Zero,
				IntPtr.Zero);

			NativeWindow window = NativeWindow.FromHandle(control.Handle);
			window.DefWndProc(ref msgSuspendUpdate);
		}

		public static void Resume(Control control)
		{
			// Create a C "true" boolean as an IntPtr
			IntPtr wparam = new IntPtr(1);
			Message msgResumeUpdate = Message.Create(control.Handle, WM_SETREDRAW, wparam,
				IntPtr.Zero);

			NativeWindow window = NativeWindow.FromHandle(control.Handle);
			window.DefWndProc(ref msgResumeUpdate);

			control.Invalidate();
		}
	}
}
