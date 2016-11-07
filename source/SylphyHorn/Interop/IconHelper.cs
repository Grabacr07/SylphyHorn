using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MetroRadiance.Interop;

namespace SylphyHorn.Interop
{
	public static class IconHelper
	{
		public static Icon GetIconFromResource(Uri uri)
		{
			var streamResourceInfo = System.Windows.Application.GetResourceStream(uri);
			if (streamResourceInfo == null) throw new ArgumentException("Resource not found.", nameof(uri));

			var dpi = PerMonitorDpi.GetDpi(IntPtr.Zero); // get desktop dpi

			using (var stream = streamResourceInfo.Stream)
			{
				return new Icon(stream, new Size((int)(16 * dpi.ScaleX), (int)(16 * dpi.ScaleY)));
			}
		}
	}
}
