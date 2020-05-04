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

		public static Color GetRandomPastelColor()
		{
			int r = Main.rnd.Next(70, 200);
			int g = Main.rnd.Next(100, 225);
			int b = Main.rnd.Next(100, 230);

			Color c = Color.FromArgb(r, g, b);

			c = c.ChangeColorBrightness(0.35f);

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
