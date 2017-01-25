using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private readonly HashSet<Keys> _pressedRegularKeys = new HashSet<Keys>();
        private readonly IKeyboardInterceptor _interceptor = new KeyboardInterceptor();

        private bool _started;
        private bool _suspended;

        private bool _suspendUntilKey;
        private ShortcutKey _keyToSuspendUntil;
        private int _keyCountToIgnore;
        private int _keyCountSeen;
        private readonly HashSet<Keys> _suspendedPressedModifiers = new HashSet<Keys>();
        private Keys _suspendedRegularKey;

        private readonly ManualResetEvent _noKeysPressedEvent = new ManualResetEvent(true);
        private readonly ManualResetEvent _notSuspendedEvent = new ManualResetEvent(true);

        public bool IsSuspendedUntilKeyPress => this._suspendUntilKey;

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

        public void SuspendUntil(IShortcutKey key, int keyCountToIgnore)
        {
            Console.WriteLine($"Ignoring {key} exactly {keyCountToIgnore} times");
            this._suspended = true;
            this._suspendUntilKey = true;
            this._keyToSuspendUntil = (ShortcutKey)key;
            this._keyCountToIgnore = keyCountToIgnore;
            this._keyCountSeen = 0;

            this._notSuspendedEvent.Reset();
        }

        public bool WaitForNoKeysPressed()
        {
            if (this._suspended) return false;

            return this._noKeysPressedEvent.WaitOne();
        }


        private void InterceptorOnKeyDown(object sender, KeyEventArgs args)
        {
            if (this._keyCountToIgnore > 0 && this._keyCountSeen >= this._keyCountToIgnore)
            {
                this._suspended = false;
                this._suspendUntilKey = false;
                this._keyCountSeen = 0;
                this._keyCountToIgnore = 0;

                this._suspendedPressedModifiers.Clear();
                this._suspendedRegularKey = Keys.None;

                this._notSuspendedEvent.Set();
            }
            else if (this._suspendUntilKey)
            {
                if (args.KeyCode.IsModifyKey())
                {
                    this._suspendedPressedModifiers.Add(args.KeyCode);
                }
                else
                {
                    this._suspendedRegularKey = args.KeyCode;
                }

                var currentKey = new ShortcutKey(this._suspendedRegularKey, this._suspendedPressedModifiers);
                if (currentKey == this._keyToSuspendUntil)
                {
                    this._keyCountSeen++;
                }
            }

            if (this._suspended) return;

            this._noKeysPressedEvent.Reset();

            if (args.KeyCode.IsModifyKey())
            {
                this._pressedModifiers.Add(args.KeyCode);
            }
            else
            {
                this._pressedRegularKeys.Add(args.KeyCode);

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

            if (this._pressedRegularKeys.Count != 0 && !args.KeyCode.IsModifyKey())
            {
                this._pressedRegularKeys.Remove(args.KeyCode);
            }

            if (!this._pressedModifiers.Any() && !this._pressedRegularKeys.Any())
            {
                this._noKeysPressedEvent.Set();
            }
        }
    }
}
