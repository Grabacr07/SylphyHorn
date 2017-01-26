using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using MetroTrilithon.Lifetime;

namespace SylphyHorn.Services
{
	public class HookService : IDisposable
	{
		public readonly ShortcutKeyDetector KeyDetector = new ShortcutKeyDetector();

		private readonly List<HookAction> _hookActions = new List<HookAction>();
		private int _suspendRequestCount;

		public HookService()
		{
			this.KeyDetector.Pressed += this.KeyHookOnPressed;
			this.KeyDetector.Start();
		}

		public IDisposable Suspend()
		{
			this._suspendRequestCount++;
			this.KeyDetector.Stop();

			return Disposable.Create(() =>
			{
				this._suspendRequestCount--;
				if (this._suspendRequestCount == 0)
				{
					this.KeyDetector.Start();
				}
			});
		}

		public IDisposable Register(Func<ShortcutKey> getShortcutKey, Action<IntPtr, ShortcutKey> action)
		{
			return this.Register(getShortcutKey, action, () => true);
		}

	    public IDisposable Register(Func<ShortcutKey> getShortcutKey, Action<IntPtr, ShortcutKey> action, Func<bool> canExecute)
	    {
	        return this.Register(getShortcutKey, action, canExecute, () => true);
	    }

        public IDisposable Register(Func<ShortcutKey> getShortcutKey, Action<IntPtr, ShortcutKey> action, Func<bool> canExecute, Func<bool> isHandled)
		{
			var hook = new HookAction(getShortcutKey, action, canExecute, isHandled);
			this._hookActions.Add(hook);

			return Disposable.Create(() => this._hookActions.Remove(hook));
		}

	    private DispatcherOperation _activeDispatcherOperation;

		private void KeyHookOnPressed(object sender, ShortcutKeyPressedEventArgs args)
		{
			if (args.ShortcutKey == ShortcutKey.None) return;

			var target = this._hookActions.FirstOrDefault(x => x.GetShortcutKey() == args.ShortcutKey);
			if (target != null && target.CanExecute())
			{
			    if (this._activeDispatcherOperation == null || this._activeDispatcherOperation.Task.IsCompleted)
			    {
			        Console.WriteLine();
                    Console.WriteLine($"Received shortcut key: {target.GetShortcutKey()}");
			        this._activeDispatcherOperation = VisualHelper.InvokeOnUIDispatcher(() => target.Action(InteropHelper.GetForegroundWindowEx(), target.GetShortcutKey()));
			    }
			    args.Handled = target.IsHandled();
			}
		}

		public void Dispose()
		{
			this.KeyDetector.Stop();
		}

		private class HookAction
		{
			public Func<ShortcutKey> GetShortcutKey { get; }

			public Action<IntPtr, ShortcutKey> Action { get; }

			public Func<bool> CanExecute { get; }

            public Func<bool> IsHandled { get; }

			public HookAction(Func<ShortcutKey> getShortcutKey, Action<IntPtr, ShortcutKey> action, Func<bool> canExecute, Func<bool> isHandled)
			{
				this.GetShortcutKey = getShortcutKey;
				this.Action = action;
				this.CanExecute = canExecute;
			    this.IsHandled = isHandled;
			}
		}
	}
}
