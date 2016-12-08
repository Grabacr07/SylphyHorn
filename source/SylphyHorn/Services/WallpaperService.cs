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
			VirtualDesktop.CurrentChanged += this.VirtualDesktopOnCurrentChanged;
		}

		private void VirtualDesktopOnCurrentChanged(object sender, VirtualDesktopChangedEventArgs e)
		{
			Task.Run(() =>
			{
				if (!Settings.General.ChangeBackgroundEachDesktop) return;

				var desktops = VirtualDesktop.GetDesktops();
				var newIndex = Array.IndexOf(desktops, e.NewDesktop) + 1;

				var file = this.GetWallpaperFiles(Settings.General.DesktopBackgroundFolderPath).FirstOrDefault(x => x.Number == newIndex);
				if (file != null) this.Set(file.Filepath);
			});
		}

		public WallpaperFile[] GetWallpaperFiles(string directoryPath)
		{
			try
			{
				var directoryInfo = new DirectoryInfo(string.IsNullOrEmpty(directoryPath) ? "dummy" : directoryPath);
				if (directoryInfo.Exists)
				{
					var dic = new Dictionary<ushort, WallpaperFile>();
					foreach (var file in directoryInfo.GetFiles())
					{
						ushort number;
						if (ushort.TryParse(Path.GetFileNameWithoutExtension(file.Name), out number)
							&& _supportedExtensions.Any(x => x == file.Extension)
							&& !dic.ContainsKey(number))
						{
							dic[number] = new WallpaperFile(number, file.FullName);
						}
					}

					return dic
						.OrderBy(kvp => kvp.Key)
						.Select(x => x.Value)
						.ToArray();
				}
			}
			catch (Exception ex)
			{
				LoggingService.Instance.Register(ex);
			}

			return Array.Empty<WallpaperFile>();
		}

		public void Dispose()
		{
			VirtualDesktop.CurrentChanged -= this.VirtualDesktopOnCurrentChanged;
		}

		private void Set(string path)
		{
			NativeMethods.SystemParametersInfo(
				SystemParametersInfo.SPI_SETDESKWALLPAPER,
				0,
				path,
				SystemParametersInfoFlag.SPIF_UPDATEINIFILE | SystemParametersInfoFlag.SPIF_SENDWININICHANGE);
		}
	}

	public class WallpaperFile
	{
		public ushort Number { get; }
		public string Filepath { get; }

		public WallpaperFile(ushort number, string path)
		{
			this.Number = number;
			this.Filepath = path;
		}
	}
}
