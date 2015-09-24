using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MetroRadiance.Chrome;
using MetroRadiance.Platform;
using SylphyHorn.Views;
using VDMHelperCLR.Common;
using WindowsDesktop;

namespace SylphyHorn.Services
{
	public class PinService : IDisposable
	{
		private readonly object _sync = new object();
		private readonly Dictionary<IntPtr, Tuple<ExternalWindow, WindowChrome>> _pinnedWindows = new Dictionary<IntPtr, Tuple<ExternalWindow, WindowChrome>>();
		private readonly IVdmHelper _helper;

		public PinService(IVdmHelper helper)
		{
			this._helper = helper;
			VirtualDesktop.CurrentChanged += this.VirtualDesktopOnCurrentChanged;
		}

		public void Register(IntPtr hWnd)
		{
			lock (this._sync)
			{
				if (this._pinnedWindows.ContainsKey(hWnd)) return;

				var external = new ExternalWindow(hWnd);
				var chrome = new WindowChrome
				{
					BorderThickness = new Thickness(4.0),
					Top = new PinMarker(),
				};
				chrome.Attach(external);

				this._pinnedWindows[hWnd] = Tuple.Create(external, chrome);
			}
		}

		public void Unregister(IntPtr hWnd)
		{
			lock (this._sync)
			{
				if (!this._pinnedWindows.ContainsKey(hWnd)) return;

				var tuple = this._pinnedWindows[hWnd];
				tuple.Item2.Detach();
				tuple.Item1.Dispose();

				this._pinnedWindows.Remove(hWnd);
			}
		}

		public void UnregisterAll()
		{
			IntPtr[] targets;
			lock (this._sync)
			{
				targets = this._pinnedWindows.Keys.ToArray();
			}

			foreach (var hWnd in targets)
			{
				this.Unregister(hWnd);
			}
		}

		private void VirtualDesktopOnCurrentChanged(object sender, VirtualDesktopChangedEventArgs e)
		{
			IntPtr[] targets;
			lock (this._sync)
			{
				targets = this._pinnedWindows.Keys.ToArray();
			}

			VisualHelper.InvokeOnUIDispatcher(() =>
			{
				foreach (var hWnd in targets.Where(x => !VirtualDesktopHelper.MoveToDesktop(x, e.NewDesktop)))
				{
					this._helper.MoveWindowToDesktop(hWnd, e.NewDesktop.Id);
				}
			});
		}

		public void Dispose()
		{
			VirtualDesktop.CurrentChanged -= this.VirtualDesktopOnCurrentChanged;
			this.UnregisterAll();
		}
	}
}
