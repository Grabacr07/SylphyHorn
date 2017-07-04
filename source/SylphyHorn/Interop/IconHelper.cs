using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MetroRadiance.Interop;
using SylphyHorn.Services;
using System.Windows;

namespace SylphyHorn.Interop
{
    public static class IconHelper
	{
		public static Icon GetIconFromResource(Uri uri)
		{
			var streamResourceInfo = System.Windows.Application.GetResourceStream(uri);
			if (streamResourceInfo == null) throw new ArgumentException("Resource not found.", nameof(uri));

			using (var stream = streamResourceInfo.Stream)
			{
                var icon = new Icon(stream);

                return ScaleIconToDpi(icon);
			}
		}

        public static Icon ToIcon(this Bitmap bitmap)
        {
            IntPtr iconHandle = bitmap.GetHicon();
            var icon = Icon.FromHandle(iconHandle);

            return icon;
        }

        private static Icon ScaleIconToDpi(Icon targetIcon)
        {
            var dpi = GetMonitorDpi();

            return new Icon(targetIcon, new System.Drawing.Size((int)(16 * dpi.ScaleX), (int)(16 * dpi.ScaleY)));
        }

        public static System.Windows.Size GetIconSize()
        {
            var dpi = GetMonitorDpi();

            return new System.Windows.Size(SystemParameters.SmallIconWidth * dpi.ScaleX, SystemParameters.SmallIconHeight * dpi.ScaleY);
        }
        
        private static Dpi GetMonitorDpi()
        {
            return PerMonitorDpi.GetDpi(IntPtr.Zero);
        }
	}
}
