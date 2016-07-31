using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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
				return new Icon(stream, new Size(16, 16));
			}
		}
	}
}
