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
            StickyWindowsManager.ToggleStickyWindowEvent += this.ShowToggleStickyWindow;
        }

		private void VirtualDesktopOnCurrentChanged(object sender, VirtualDesktopChangedEventArgs e)
		{
			if (!GeneralSettings.NotificationWhenSwitchedDesktop) return;

			VisualHelper.InvokeOnUIDispatcher(() =>
			{
				var desktops = VirtualDesktop.GetDesktops();
				var newIndex = Array.IndexOf(desktops, e.NewDesktop) + 1;

				this.currentNotificationWindow?.Dispose();

                var vmodel = new NotificationWindowViewModel
                {
                    Title = ProductInfo.Title,
                    Header = "Virtual Desktop Switched",
                    Body = "Current Desktop: Desktop " + newIndex,
                };
                this.currentNotificationWindow = ShowWindow(vmodel);
			});
		}


        private void ShowToggleStickyWindow(object sender, string body)
        {
            this.currentNotificationWindow?.Dispose();

            var vmodel = new NotificationWindowViewModel
            {
                Title = ProductInfo.Title,
                Header = "Window Pin Toggled",
                Body = body
            };
            this.currentNotificationWindow = ShowWindow(vmodel);
        }

		private static IDisposable ShowWindow(NotificationWindowViewModel vm)
		{
			
			var source = new CancellationTokenSource();
			var window = new NotificationWindow
			{
				DataContext = vm,
			};
			window.Show();

			Task.Delay(TimeSpan.FromSeconds(2.5), source.Token)
				.ContinueWith(_ => window.Close(), TaskScheduler.FromCurrentSynchronizationContext());

			return Disposable.Create(() => source.Cancel());
		}

		public void Dispose()
		{
			VirtualDesktop.CurrentChanged -= this.VirtualDesktopOnCurrentChanged;
            StickyWindowsManager.ToggleStickyWindowEvent -= this.ShowToggleStickyWindow;
        }
	}
}
