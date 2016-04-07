using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SylphyHorn.Interop;
using SylphyHorn.Serialization;
using WindowsDesktop;

namespace SylphyHorn.Services
{
	public class WallpaperService : IDisposable
	{
		public static WallpaperService Instance { get; } = new WallpaperService();

		private WallpaperService()
		{
			VirtualDesktop.CurrentChanged += VirtualDesktopOnCurrentChanged;
		}

		private static void VirtualDesktopOnCurrentChanged(object sender, VirtualDesktopChangedEventArgs e)
		{
			VisualHelper.InvokeOnUIDispatcher(() =>
			{
				var desktops = VirtualDesktop.GetDesktops();
				var newIndex = Array.IndexOf(desktops, e.NewDesktop) + 1;

				var imgDirectoryPath = Settings.General.DesktopBackgroundFolderPath.Value ?? "";
				var imgPath = Path.Combine(imgDirectoryPath, newIndex + ".bmp");
				if (File.Exists(imgPath))
				{
					Set(imgPath);
				}
			});
		}

		public void Dispose()
		{
			VirtualDesktop.CurrentChanged -= VirtualDesktopOnCurrentChanged;
		}

		public static void Set(string path)
		{
			NativeMethods.SystemParametersInfo(
				SystemParametersInfo.SPI_SETDESKWALLPAPER,
				0,
				path,
				SystemParametersInfoFlag.SPIF_UPDATEINIFILE | SystemParametersInfoFlag.SPIF_SENDWININICHANGE);
		}
	}
}
