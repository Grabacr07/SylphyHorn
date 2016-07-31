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
		private static readonly string[] _supportedExtensions = { ".png", ".jpg", ".jpeg", ".bmp", };

		public static WallpaperService Instance { get; } = new WallpaperService();

		private WallpaperService()
		{
			VirtualDesktop.CurrentChanged += VirtualDesktopOnCurrentChanged;
		}

		private static void VirtualDesktopOnCurrentChanged(object sender, VirtualDesktopChangedEventArgs e)
		{
			Task.Run(() => 
			//VisualHelper.InvokeOnUIDispatcher(() =>
			{
				if (!Settings.General.ChangeBackgroundEachDesktop) return;

				var desktops = VirtualDesktop.GetDesktops();
				var newIndex = Array.IndexOf(desktops, e.NewDesktop) + 1;

				var dirPath = Settings.General.DesktopBackgroundFolderPath.Value ?? "";
				if (Directory.Exists(dirPath))
				{
					foreach (var extension in _supportedExtensions)
					{
						var filePath = Path.Combine(dirPath, newIndex + extension);
						var wallpaper = new FileInfo(filePath);
						if (wallpaper.Exists)
						{
							Set(wallpaper.FullName);
							break;
						}
					}
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
