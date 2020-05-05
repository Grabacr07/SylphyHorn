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

		private static string GetDesktopName(int index)
		{
			string setting = null;
			switch(index)
			{
				case 1: setting = Settings.General.DesktopName1.Value; break;
				case 2: setting = Settings.General.DesktopName2.Value; break;
				case 3: setting = Settings.General.DesktopName3.Value; break;
				case 4: setting = Settings.General.DesktopName4.Value; break;
				case 5: setting = Settings.General.DesktopName5.Value; break;
				case 6: setting = Settings.General.DesktopName6.Value; break;
				case 7: setting = Settings.General.DesktopName7.Value; break;
				case 8: setting = Settings.General.DesktopName8.Value; break;
				case 9: setting = Settings.General.DesktopName9.Value; break;
				case 10: setting = Settings.General.DesktopName10.Value; break;
			}
			if(setting != null && setting != "")
			{
				return $"{index}: {setting}";
			}
			return $"Desktop {index}";
		}

		private static IDisposable ShowDesktopWindow(int index)
		{
			var vmodel = new NotificationWindowViewModel
			{
				Title = ProductInfo.Title,
				Body = GetDesktopName(index),
			};
			var source = new CancellationTokenSource();

			var settings = Settings.General.Display.Value;
			Monitor[] targets;
			if (settings == 0)
			{
				targets = new[] { MonitorService.GetCurrentArea() };
			}
			else
			{
				var monitors = MonitorService.GetAreas();
				if (settings == uint.MaxValue)
				{
					targets = monitors;
				}
				else
				{
					targets = new[] { monitors[settings - 1] };
				}
			}
			var windows = targets.Select(area =>
			{
				var window = new SwitchWindow(area.WorkArea)
				{
					DataContext = vmodel,
				};
				window.Show();
				return window;
			}).ToList();

			Task.Delay(TimeSpan.FromMilliseconds(Settings.General.NotificationDuration), source.Token)
				.ContinueWith(_ => windows.ForEach(window => window.Close()), TaskScheduler.FromCurrentSynchronizationContext());

			return Disposable.Create(() => source.Cancel());
		}

		private static IDisposable ShowPinWindow(IntPtr hWnd, PinOperations operation)
		{
			var vmodel = new NotificationWindowViewModel
			{
				Title = ProductInfo.Title,
				Body = $"{(operation.HasFlag(PinOperations.Window) ? "Window" : "Application")} {(operation.HasFlag(PinOperations.Pin) ? "Pinned" : "Unpinned")}",
			};
			var source = new CancellationTokenSource();
			var window = new PinWindow(hWnd)
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
