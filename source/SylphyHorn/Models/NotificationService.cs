using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WindowsDesktop;
using MetroTrilithon.Lifetime;
using SylphyHorn.ViewModels;
using SylphyHorn.Views;

namespace SylphyHorn.Models
{
	public class NotificationService : IDisposable
	{
		private IDisposable currentNotificationWindow;

		public NotificationService()
		{
			VirtualDesktop.CurrentChanged += this.VirtualDesktopOnCurrentChanged;
		}

		private void VirtualDesktopOnCurrentChanged(object sender, VirtualDesktopChangedEventArgs e)
		{
			if (!GeneralSettings.NotificationWhenSwitchedDesktop) return;

			VisualHelper.InvokeOnUIDispatcher(() =>
			{
				var desktops = VirtualDesktop.GetDesktops();
				var newIndex = Array.IndexOf(desktops, e.NewDesktop) + 1;

				this.currentNotificationWindow?.Dispose();
				this.currentNotificationWindow = ShowWindow(newIndex);

                // var imgDirectoryPath = @"D:\User\Pictures\vd-bg\";
                var imgDirectoryPath = @"";
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

			Task.Delay(TimeSpan.FromSeconds(2.5), source.Token)
				.ContinueWith(_ => window.Close(), TaskScheduler.FromCurrentSynchronizationContext());

			return Disposable.Create(() => source.Cancel());
		}

		public void Dispose()
		{
			VirtualDesktop.CurrentChanged -= this.VirtualDesktopOnCurrentChanged;
		}
	}
}
