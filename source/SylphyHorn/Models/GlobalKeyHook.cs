using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using MetroTrilithon.Linq;
using Open.WinKeyboardHook;

namespace SylphyHorn.Models
{
	public class GlobalKeyHook
	{
		private readonly IKeyboardInterceptor interceptor = new KeyboardInterceptor();
		private readonly HashSet<Key> pressedModifiers = new HashSet<Key>();

		public event EventHandler<ShortcutKey> Pressed;

		public GlobalKeyHook()
		{
			this.interceptor.KeyDown += this.InterceptorOnKeyDown;
			this.interceptor.KeyUp += this.InterceptorOnKeyUp;
		}

		public void Start()
		{
			this.interceptor.StartCapturing();
		}

		public void Stop()
		{
			this.interceptor.StopCapturing();
		}

		private void InterceptorOnKeyDown(object sender, System.Windows.Forms.KeyEventArgs args)
		{
			var key = KeyInterop.KeyFromVirtualKey((int)args.KeyCode);
			if (key.IsModifyKey())
			{
				this.pressedModifiers.Add(key);
			}
			else
			{
				System.Diagnostics.Debug.WriteLine("KeyDown: " + this.pressedModifiers.Concat(EnumerableEx.Return(key)).JoinString(" + "));
				this.Pressed?.Invoke(this, ShortcutKey.Create(key, this.pressedModifiers));
			}
		}

		private void InterceptorOnKeyUp(object sender, System.Windows.Forms.KeyEventArgs args)
		{
			if (this.pressedModifiers.Count == 0) return;

			var key = KeyInterop.KeyFromVirtualKey((int)args.KeyCode);
			if (key.IsModifyKey())
			{
				this.pressedModifiers.Remove(key);
			}
		}

	}

	public static class KeyHelper
	{
		public static bool IsModifyKey(this Key key)
		{
			// System.Windows.Forms.KeyEventArgs.Modifiers が LWin と RWin を含めてくれないので…
			// あと Left と Right 区別できたらいいなって…
			// _:(´ཀ`」 ∠):_

			return key == Key.LeftAlt
				|| key == Key.LeftCtrl
				|| key == Key.LeftShift
				|| key == Key.LWin
				|| key == Key.RightAlt
				|| key == Key.RightCtrl
				|| key == Key.RightShift
				|| key == Key.RWin;
		}
	}
}
