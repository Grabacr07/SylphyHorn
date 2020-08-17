using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MetroTrilithon.Lifetime;
using SylphyHorn.Properties;
using SylphyHorn.Serialization;
using SylphyHorn.UI;
using SylphyHorn.UI.Bindings;
using WindowsDesktop;

namespace SylphyHorn.Services
{
	public class NotificationService : IDisposable
	{
		public static NotificationService Instance { get; } = new NotificationService();

		private readonly SerialDisposable _notificationWindow = new SerialDisposable();

		private NotificationService()
		{
			VirtualDesktop.CurrentChanged += this.VirtualDesktopOnCurrentChanged;
			VirtualDesktopService.WindowPinned += this.VirtualDesktopServiceOnWindowPinned;
		}

		private void VirtualDesktopOnCurrentChanged(object sender, VirtualDesktopChangedEventArgs e)
		{
			if (!Settings.General.NotificationWhenSwitchedDesktop) return;

			VisualHelper.InvokeOnUIDispatcher(() =>
			{
				var desktops = VirtualDesktop.GetDesktops();
				var newIndex = Array.IndexOf(desktops, e.NewDesktop) + 1;

				this._notificationWindow.Disposable = ShowDesktopWindow(newIndex);
			});
		}

		private void VirtualDesktopServiceOnWindowPinned(object sender, WindowPinnedEventArgs e)
		{
			VisualHelper.InvokeOnUIDispatcher(() =>
			{
				this._notificationWindow.Disposable = ShowPinWindow(e.Target, e.PinOperation);
			});
		}

		private static IDisposable ShowDesktopWindow(int index)
		{
			var vmodel = new NotificationWindowViewModel
			{
				Title = ProductInfo.Title,
				Header = "Virtual Desktop Switched",
				Body = "Current Desktop: Desktop " + index,
			};
			var source = new CancellationTokenSource();

			var settings = Settings.General.Display.Value;
			IntPtr[] targets;
			if (settings == 0)
			{
				targets = new[] { MonitorHelper.GetCurrentHmonitor() };
			}
			else if (settings == uint.MaxValue)
			{
				targets = MonitorHelper.GetHmonitors();
			}
			else
			{
				targets = new[] { MonitorHelper.GetHmonitorFromIndex((int)settings - 1) };
			}
			var placement = (WindowPlacement)Settings.General.Placement.Value;
			var windows = targets.Select(hMonitor =>
			{
				var window = new SwitchWindow(hMonitor, placement)
				{
					DataContext = vmodel,
				};
				window.Show();
				return window;
			}).ToArray();

			Task.Delay(TimeSpan.FromMilliseconds(Settings.General.NotificationDuration), source.Token)
				.ContinueWith(_ => Array.ForEach(windows, window => window.Close()), TaskScheduler.FromCurrentSynchronizationContext());

			return Disposable.Create(() => source.Cancel());
		}

		private static IDisposable ShowPinWindow(IntPtr hWnd, PinOperations operation)
		{
			var vmodel = new NotificationWindowViewModel
			{
				Title = ProductInfo.Title,
				Header = "Virtual Desktop",
				Body = $"{(operation.HasFlag(PinOperations.Pin) ? "Pinned" : "Unpinned")} this {(operation.HasFlag(PinOperations.Window) ? "window" : "application")}",
			};
			var source = new CancellationTokenSource();
			var placement = (WindowPlacement)Settings.General.PinPlacement.Value;
			if (placement == WindowPlacement.Default)
			{
				placement = (WindowPlacement)Settings.General.Placement.Value;
			}
			var window = new PinWindow(hWnd, placement)
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
			VirtualDesktopService.WindowPinned -= this.VirtualDesktopServiceOnWindowPinned;

			this._notificationWindow.Dispose();
		}
	}
}
