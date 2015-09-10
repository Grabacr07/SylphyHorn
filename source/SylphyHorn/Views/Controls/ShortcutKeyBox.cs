using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MetroTrilithon.Serialization;
using SylphyHorn.Models;

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


		private readonly HashSet<Key> pressedModifiers = new HashSet<Key>();
		private Key pressedKey = Key.None;
		private ShortcutKey? current;


		#region SerializableProperty 依存関係プロパティ

		public SerializableProperty<ShortcutKey?> SerializableProperty
		{
			get { return (SerializableProperty<ShortcutKey?>)this.GetValue(SerializablePropertyProperty); }
			set { this.SetValue(SerializablePropertyProperty, value); }
		}
		public static readonly DependencyProperty SerializablePropertyProperty =
			DependencyProperty.Register(nameof(SerializableProperty), typeof(SerializableProperty<ShortcutKey?>), typeof(ShortcutKeyBox), new UIPropertyMetadata(null, SerializablePropertyChangedCallback));

		private static void SerializablePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs args)
		{
			var instance = (ShortcutKeyBox)d;
			var oldValue = (SerializableProperty<ShortcutKey?>)args.OldValue;
			var newValue = (SerializableProperty<ShortcutKey?>)args.NewValue;

			instance.current = newValue.Value;
			instance.UpdateText();
		}

		#endregion
		

		protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
		{
			base.OnGotKeyboardFocus(e);

			if (this.SerializableProperty != null) this.current = this.SerializableProperty.Value;
			this.UpdateText();
		}

		protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
		{
			base.OnLostKeyboardFocus(e);

			if (this.SerializableProperty != null) this.SerializableProperty.Value = this.current;
		}

		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			if (!e.IsRepeat)
			{
				var key = e.Key == Key.System ? e.SystemKey : e.Key;
				if (key == Key.Back)
				{
					this.pressedModifiers.Clear();
					this.pressedKey = Key.None;
				}
				else if (key.IsModifyKey())
				{
					this.pressedModifiers.Add(key);
				}
				else
				{
					this.pressedKey = key;
				}

				this.current = this.pressedModifiers.Any() && this.pressedKey != Key.None
					? new ShortcutKey(this.pressedKey, this.pressedModifiers.ToArray())
					: (ShortcutKey?)null;
				System.Diagnostics.Debug.WriteLine("Current: " + this.current);
				
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
					this.pressedModifiers.Remove(key);
				}

				this.pressedKey = Key.None;
				this.UpdateText();
			}

			e.Handled = true;
			base.OnPreviewKeyUp(e);
		}

		private void UpdateText()
		{
			var text = (this.current ?? new ShortcutKey(this.pressedKey, this.pressedModifiers)).ToString();

			this.Text = text;
			this.CaretIndex = text.Length;
		}
	}
}
