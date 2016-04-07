using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SylphyHorn.Services;

namespace SylphyHorn.Views.Controls
{
	public class ShortcutKeyBox : TextBox
	{
		static ShortcutKeyBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(
				typeof(ShortcutKeyBox),
				new FrameworkPropertyMetadata(typeof(ShortcutKeyBox)));
		}


		private readonly HashSet<Key> _pressedModifiers = new HashSet<Key>();
		private Key _pressedKey = Key.None;


		#region Current 依存関係プロパティ

		public ShortcutKey? Current
		{
			get { return (ShortcutKey?)this.GetValue(CurrentProperty); }
			set { this.SetValue(CurrentProperty, value); }
		}
		public static readonly DependencyProperty CurrentProperty =
			DependencyProperty.Register(nameof(Current), typeof(ShortcutKey?), typeof(ShortcutKeyBox), new UIPropertyMetadata(null, CurrentPropertyChangedCallback));

		private static void CurrentPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs args)
		{
			var instance = (ShortcutKeyBox)d;
			instance.UpdateText();
		}

		#endregion


		protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
		{
			base.OnGotKeyboardFocus(e);

			this.UpdateText();
		}

		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			if (!e.IsRepeat)
			{
				var key = e.Key == Key.System ? e.SystemKey : e.Key;
				if (key == Key.Back)
				{
					this._pressedModifiers.Clear();
					this._pressedKey = Key.None;
				}
				else if (key.IsModifyKey())
				{
					this._pressedModifiers.Add(key);
				}
				else
				{
					this._pressedKey = key;
				}

				this.Current = this._pressedModifiers.Any() && this._pressedKey != Key.None
					? new ShortcutKey(this._pressedKey.ToVirtualKey(), this._pressedModifiers.Select(x => x.ToVirtualKey()).ToArray())
					: (ShortcutKey?)null;

				this.UpdateText();
			}

			e.Handled = true;
			base.OnPreviewKeyDown(e);
		}

		protected override void OnPreviewKeyUp(KeyEventArgs e)
		{
			if (!e.IsRepeat)
			{
				var key = e.Key == Key.System ? e.SystemKey : e.Key;
				if (key.IsModifyKey())
				{
					this._pressedModifiers.Remove(key);
				}

				this._pressedKey = Key.None;
				this.UpdateText();
			}

			e.Handled = true;
			base.OnPreviewKeyUp(e);
		}

		private void UpdateText()
		{
			var text = (this.Current ?? new ShortcutKey(this._pressedKey.ToVirtualKey(), this._pressedModifiers.Select(x => x.ToVirtualKey()).ToArray())).ToString();

			this.Text = text;
			this.CaretIndex = text.Length;
		}
	}
}
