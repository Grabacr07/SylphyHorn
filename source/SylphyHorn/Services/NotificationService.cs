using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MetroTrilithon.Lifetime;
using SylphyHorn.Properties;
using SylphyHorn.Serialization;
using SylphyHorn.ViewModels;
using SylphyHorn.Views;
using WindowsDesktop;

namespace SylphyHorn.Services
{
	public class NotificationService : IDisposable
	{
		private readonly SerialDisposable _notificationWindow = new SerialDisposable();

		public NotificationService()
		{
			VirtualDesktop.CurrentChanged += this.VirtualDesktopOnCurrentChanged;
		}

		private void VirtualDesktopOnCurrentChanged(object sender, VirtualDesktopChangedEventArgs e)
		{
			if (!Settings.General.NotificationWhenSwitchedDesktop) return;

			VisualHelper.InvokeOnUIDispatcher(() =>
			{
				var desktops = VirtualDesktop.GetDesktops();
				var newIndex = Array.IndexOf(desktops, e.NewDesktop) + 1;

				this._notificationWindow.Disposable = ShowWindow(newIndex);

				// var imgDirectoryPath = @"D:\User\Pictures\vd-bg\";
				var imgDirectoryPath = Settings.General.DesktopBackgroundFolderPath;
				var bmpPath = imgDirectoryPath + newIndex.ToString() + ".bmp";
				if (System.IO.File.Exists(bmpPath))
				{
					WallpaperService.Set(bmpPath);
				}
			});
		}

		private static IDisposable ShowWindow(int index)
		{
			var vmodel = new NotificationWindowViewModel
			{
				Title = ProductInfo.Title,
				Header = "Virtual Desktop Switched",
				Body = "Current Desktop: Desktop " + index,
			};
			var source = new CancellationTokenSource();
			var window = new NotificationWindow
			{
				DataContext = vmodel,
			};
			window.Show();

			Task.Delay(TimeSpan.FromMilliseconds(Settings.General.NotificationDuration), source.Token)
				.ContinueWith(_ => window.Close(), TaskScheduler.FromCurrentSynchronizationContext());

			return Disposable.Create(() => source.Cancel());
		}

		public void Dispose()
		{
			VirtualDesktop.CurrentChanged -= this.VirtualDesktopOnCurrentChanged;
			this._notificationWindow.Dispose();
		}
	}
}
