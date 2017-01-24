﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MetroTrilithon.Lifetime;

namespace SylphyHorn.Services
{
	public class HookService : IDisposable
	{
		private readonly ShortcutKeyDetector _detector = new ShortcutKeyDetector();
		private readonly List<HookAction> _hookActions = new List<HookAction>();
		private int _suspendRequestCount;

		public HookService()
		{
			this._detector.Pressed += this.KeyHookOnPressed;
			this._detector.Start();
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

		public IDisposable Register(Func<ShortcutKey> getShortcutKey, Action<IntPtr, ShortcutKeyDetector, ShortcutKey> action)
		{
			return this.Register(getShortcutKey, action, () => true);
		}

	    public IDisposable Register(Func<ShortcutKey> getShortcutKey, Action<IntPtr, ShortcutKeyDetector, ShortcutKey> action, Func<bool> canExecute)
	    {
	        return this.Register(getShortcutKey, action, canExecute, () => true);
	    }

        public IDisposable Register(Func<ShortcutKey> getShortcutKey, Action<IntPtr, ShortcutKeyDetector, ShortcutKey> action, Func<bool> canExecute, Func<bool> isHandled)
		{
			var hook = new HookAction(getShortcutKey, action, canExecute, isHandled);
			this._hookActions.Add(hook);

			return Disposable.Create(() => this._hookActions.Remove(hook));
		}

		private void KeyHookOnPressed(object sender, ShortcutKeyPressedEventArgs args)
		{
			if (args.ShortcutKey == ShortcutKey.None) return;

			var target = this._hookActions.FirstOrDefault(x => x.GetShortcutKey() == args.ShortcutKey);
			if (target != null && target.CanExecute())
			{
				VisualHelper.InvokeOnUIDispatcher(() => target.Action(InteropHelper.GetForegroundWindowEx(), this._detector, target.GetShortcutKey()));
				args.Handled = target.IsHandled();
			}
		}

		public void Dispose()
		{
			this._detector.Stop();
		}

		private class HookAction
		{
			public Func<ShortcutKey> GetShortcutKey { get; }

			public Action<IntPtr, ShortcutKeyDetector, ShortcutKey> Action { get; }

			public Func<bool> CanExecute { get; }

            public Func<bool> IsHandled { get; }

			public HookAction(Func<ShortcutKey> getShortcutKey, Action<IntPtr, ShortcutKeyDetector, ShortcutKey> action, Func<bool> canExecute, Func<bool> isHandled)
			{
				this.GetShortcutKey = getShortcutKey;
				this.Action = action;
				this.CanExecute = canExecute;
			    this.IsHandled = isHandled;
			}
		}
	}
}
