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
	public class RenameService : IDisposable
	{
		public static RenameService Instance { get; } = new RenameService();

		private readonly SerialDisposable _renameWindow = new SerialDisposable();

		private RenameService()
		{
			VirtualDesktopService.RenameCurrent += this.VirtualDesktopServiceOnRenameCurrent;
		}

		private static IDisposable ShowRenameWindow()
		{
			var current = VirtualDesktop.Current;
			int index = DesktopHelper.GetIndex(current);

			var source = new CancellationTokenSource();

			var vmodel = new RenameWindowViewModel
			{
				Index = index,
			};
			var window = new RenameWindow(MonitorService.GetCurrentArea().WorkArea)
			{
				DataContext = vmodel,
			};
			window.Show();
			window.Activate();

			Task.Delay(TimeSpan.FromMilliseconds(30000), source.Token)
			.ContinueWith(_ => window.Close(), TaskScheduler.FromCurrentSynchronizationContext());

			return Disposable.Create(() => source.Cancel());
		}

		private void VirtualDesktopServiceOnRenameCurrent(object sender, RenameEventArgs e)
		{
			VisualHelper.InvokeOnUIDispatcher(() =>
			{
				this._renameWindow.Disposable = ShowRenameWindow();
			});
		}

		public void Dispose()
		{
			VirtualDesktopService.RenameCurrent -= this.VirtualDesktopServiceOnRenameCurrent;
			this._renameWindow.Dispose();
		}
	}
}
