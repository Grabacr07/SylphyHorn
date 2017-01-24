using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using WindowsDesktop;
using Open.WinKeyboardHook;

namespace SylphyHorn.Services
{
    /// <summary>
    /// Provides the function to detect a shortcut key ([modifier key(s)] + [key] style) by use of global key hook.
    /// </summary>
    public class ShortcutKeyDetector : IShortcutKeyDetector
    {
        private readonly HashSet<Keys> _pressedModifiers = new HashSet<Keys>();
        private readonly HashSet<Keys> _regularKeys = new HashSet<Keys>();
        private readonly IKeyboardInterceptor _interceptor = new KeyboardInterceptor();

        private bool _started;
        private bool _suspended;

        private readonly ManualResetEvent _noKeysPressedEvent = new ManualResetEvent(true);

        /// <summary>
        /// Occurs when detects a shortcut key.
        /// </summary>
        public event EventHandler<ShortcutKeyPressedEventArgs> Pressed;

        public ShortcutKeyDetector()
        {
            this._interceptor.KeyDown += this.InterceptorOnKeyDown;
            this._interceptor.KeyUp += this.InterceptorOnKeyUp;
        }

        public void Start()
        {
            if (!this._started)
            {
                this._interceptor.StartCapturing();
                this._started = true;
            }

            this._suspended = false;
        }

        public void Stop()
        {
            this._suspended = true;
            this._pressedModifiers.Clear();
        }

        public bool WaitForNoKeysPressed()
        {
            if (this._suspended) return false;

            return this._noKeysPressedEvent.WaitOne();
        }

        private void InterceptorOnKeyDown(object sender, KeyEventArgs args)
        {
            if (this._suspended) return;

            this._noKeysPressedEvent.Reset();

            if (args.KeyCode.IsModifyKey())
            {
                this._pressedModifiers.Add(args.KeyCode);
            }
            else
            {
                this._regularKeys.Add(args.KeyCode);

                var pressedEventArgs = new ShortcutKeyPressedEventArgs(args.KeyCode, this._pressedModifiers);
                this.Pressed?.Invoke(this, pressedEventArgs);
                if (pressedEventArgs.Handled) args.SuppressKeyPress = true;
            }
        }

        private void InterceptorOnKeyUp(object sender, KeyEventArgs args)
        {
            if (this._suspended) return;

            if (this._pressedModifiers.Count != 0 && args.KeyCode.IsModifyKey())
            {
                this._pressedModifiers.Remove(args.KeyCode);
            }

            if (this._regularKeys.Count != 0 && !args.KeyCode.IsModifyKey())
            {
                this._regularKeys.Remove(args.KeyCode);
            }

            if (!this._pressedModifiers.Any() && !this._regularKeys.Any())
            {
                this._noKeysPressedEvent.Set();
            }
        }
    }
}
