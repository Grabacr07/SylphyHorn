using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MetroTrilithon.Lifetime;
using VDMHelperCLR.Common;

namespace SylphyHorn.Services
{
	public class HookService : IDisposable
	{
		private readonly ShortcutKeyDetector _detector = new ShortcutKeyDetector();
		private readonly List<HookAction> _hookActions = new List<HookAction>();
		private readonly IVdmHelper _helper;
		private int _suspendRequestCount;

		public HookService(IVdmHelper helper)
		{
			this._detector.Pressed += this.KeyHookOnPressed;
			this._detector.Start();
			this._helper = helper;
		}

		public IDisposable Suspend()
		{
			this._suspendRequestCount++;
			this._detector.Stop();

			return Disposable.Create(() =>
			{
				this._suspendRequestCount--;
				if (this._suspendRequestCount == 0)
				{
					this._detector.Start();
				}
			});
		}

		public IDisposable Register(ShortcutKey shortcutKey, Action<IntPtr> action)
		{
			return this.Register(shortcutKey, action, () => true);
		}

		public IDisposable Register(ShortcutKey shortcutKey, Action<IntPtr> action, Func<bool> canExecute)
		{
			var hook = new HookAction(shortcutKey, action, canExecute);
			this._hookActions.Add(hook);

			return Disposable.Create(() => this._hookActions.Remove(hook));
		}

		private void KeyHookOnPressed(object sender, ShortcutKeyPressedEventArgs args)
		{
			if (args.ShortcutKey == ShortcutKey.None) return;

			var target = this._hookActions.FirstOrDefault(x => x.ShortcutKey == args.ShortcutKey);
			if (target != null && target.CanExecute())
			{
				VisualHelper.InvokeOnUIDispatcher(() => target.Action(InteropHelper.GetForegroundWindowEx()));
				args.Handled = true;
			}
		}

		public void Dispose()
		{
			this._detector.Stop();
			this._helper?.Dispose();
		}

		private class HookAction
		{
			public ShortcutKey ShortcutKey { get; }

			public Action<IntPtr> Action { get; }

			public Func<bool> CanExecute { get; }

			public HookAction(ShortcutKey shortcutKey, Action<IntPtr> action, Func<bool> canExecute)
			{
				this.ShortcutKey = shortcutKey;
				this.Action = action;
				this.CanExecute = canExecute;
			}
		}
	}
}
