using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WindowsDesktop;
using MetroTrilithon.Lifetime;
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
			VisualHelper.InvokeOnUIDispatcher(() =>
			{
				this.currentNotificationWindow?.Dispose();
				this.currentNotificationWindow = ShowWindow();
			});
		}

		private static IDisposable ShowWindow()
		{
			var source = new CancellationTokenSource();
			var window = new NotificationWindow
			{
				DataContext = null, // あとで
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
