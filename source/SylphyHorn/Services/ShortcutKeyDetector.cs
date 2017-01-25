﻿using System;
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
        private readonly ShortcutKeyAccumulator _accumulator = new ShortcutKeyAccumulator();
        private readonly IKeyboardInterceptor _interceptor = new KeyboardInterceptor();

        private bool _started;
        private bool _suspended;

        private bool _suspendUntilKey;
        private ShortcutKey _keyToSuspendUntil;
        private int _keyCountToIgnore;
        private int _keyCountSeen;

        private readonly ManualResetEvent _noKeysPressedEvent = new ManualResetEvent(true);

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
            //this._pressedModifiers.Clear();
            this._accumulator.Clear();
        }

        public void SuspendUntil(IShortcutKey key, int keyCountToIgnore)
        {
            Console.WriteLine($"Ignoring {key} exactly {keyCountToIgnore} times");
            this._suspended = true;
            this._suspendUntilKey = true;
            this._keyToSuspendUntil = (ShortcutKey)key;
            this._keyCountToIgnore = keyCountToIgnore;
            this._keyCountSeen = 0;
        }

        public bool WaitForNoKeysPressed()
        {
            if (this._suspended) return false;

            return this._noKeysPressedEvent.WaitOne();
        }

        private void UpdateSuspendUntilKeyCount(IEnumerable<ShortcutKey> pressedKeys)
        {
            if (this._keyCountToIgnore > 0 && this._keyCountSeen >= this._keyCountToIgnore)
            {
                this._suspended = false;
                this._suspendUntilKey = false;
                this._keyCountSeen = 0;
                this._keyCountToIgnore = 0;
            }
            else if (this._suspendUntilKey)
            {
                foreach (var currentKey in pressedKeys)
                {
                    if (currentKey == this._keyToSuspendUntil)
                    {
                        this._keyCountSeen++;
                    }
                }
            }
        }

        private void InterceptorOnKeyDown(object sender, KeyEventArgs args)
        {
            this._accumulator.Add(args.KeyCode);
            var pressedKeys = this._accumulator.GetShortcutKeys();

            if (this._suspendUntilKey)
            {
                this.UpdateSuspendUntilKeyCount(pressedKeys);
            }

            if (this._suspended) return;

            this._noKeysPressedEvent.Reset();

            if (!args.KeyCode.IsModifyKey())
            {
                var pressedEventArgs = new ShortcutKeyPressedEventArgs(args.KeyCode, this._accumulator.Modifiers);
                this.Pressed?.Invoke(this, pressedEventArgs);
                if (pressedEventArgs.Handled) args.SuppressKeyPress = true;
            }
        }

        private void InterceptorOnKeyUp(object sender, KeyEventArgs args)
        {
            this._accumulator.Remove(args.KeyCode);

            if (this._suspended) return;

            if (!this._accumulator.Modifiers.Any() && !this._accumulator.Keys.Any())
            {
                this._noKeysPressedEvent.Set();
            }
        }
    }
}
