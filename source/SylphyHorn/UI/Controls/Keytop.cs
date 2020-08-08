using SylphyHorn.Interop;
using SylphyHorn.Serialization;
using SylphyHorn.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using VirtualKey = System.Windows.Forms.Keys;

namespace SylphyHorn.UI.Controls
{
	public enum KeyKind
	{
		Normal,
		Left,
		Right,
		NumPad,
	}

	public class Keytop : Control
	{
		static Keytop()
		{
			DefaultStyleKeyProperty.OverrideMetadata(
				typeof(Keytop),
				new FrameworkPropertyMetadata(typeof(Keytop)));
		}
		
		#region Key dependency property

		public static readonly DependencyProperty KeyProperty = DependencyProperty.Register(
			nameof(Key), typeof(VirtualKey), typeof(Keytop), new PropertyMetadata(default(VirtualKey), HandleKeyPropertyChanged));

		private static void HandleKeyPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			var instance = (Keytop)sender;
			var newValue = (VirtualKey)args.NewValue;

			var tuple = GetKeyString(newValue);
			instance.KeyText = tuple.Item1;
			instance.KeyKind = tuple.Item2;
		}

		public VirtualKey Key
		{
			get { return (VirtualKey)this.GetValue(KeyProperty); }
			set { this.SetValue(KeyProperty, value); }
		}

		#endregion

		#region KeyKind dependency property

		public static readonly DependencyProperty KeyKindProperty = DependencyProperty.Register(
			nameof(KeyKind), typeof(KeyKind), typeof(Keytop), new PropertyMetadata(default(KeyKind)));

		public KeyKind KeyKind
		{
			get { return (KeyKind)this.GetValue(KeyKindProperty); }
			private set { this.SetValue(KeyKindProperty, value); }
		}

		#endregion

		#region KeyText dependency property

		public static readonly DependencyProperty KeyTextProperty = DependencyProperty.Register(
			nameof(KeyText), typeof(string), typeof(Keytop), new PropertyMetadata(default(string)));

		public string KeyText
		{
			get { return (string)this.GetValue(KeyTextProperty); }
			private set { this.SetValue(KeyTextProperty, value); }
		}

		#endregion
		
		private static Tuple<string, KeyKind> GetKeyString(VirtualKey key)
		{
			string text;
			KeyKind kind;

			switch (key)
			{
				case VirtualKey.LShiftKey:
					text = "Shift";
					kind = KeyKind.Left;
					break;
				case VirtualKey.RShiftKey:
					text = "Shift";
					kind = KeyKind.Right;
					break;

				case VirtualKey.LMenu:
					text = "Alt";
					kind = KeyKind.Left;
					break;
				case VirtualKey.RMenu:
					text = "Alt";
					kind = KeyKind.Right;
					break;

				case VirtualKey.LControlKey:
					text = "Ctrl";
					kind = KeyKind.Left;
					break;
				case VirtualKey.RControlKey:
					text = "Ctrl";
					kind = KeyKind.Right;
					break;

				case VirtualKey.LWin:
					text = "Win";
					kind = KeyKind.Left;
					break;
				case VirtualKey.RWin:
					text = "Win";
					kind = KeyKind.Right;
					break;

				case VirtualKey.Control:
					text = "Ctrl";
					kind = KeyKind.Normal;
					break;
				
				case VirtualKey.Return:
					text = "Enter";
					kind = KeyKind.Normal;
					break;
				case VirtualKey.NoName:
					// shoddy construction :(
					text = "Win";
					kind = KeyKind.Normal;
					break;

				case VirtualKey numKey when numKey >= VirtualKey.D0 && numKey <= VirtualKey.D9:
					text = (numKey - VirtualKey.D0).ToString();
					kind = KeyKind.Normal;
					break;

				case VirtualKey numpadKey when numpadKey >= VirtualKey.NumPad0 && numpadKey <= VirtualKey.NumPad9:
					text = (numpadKey - VirtualKey.NumPad0).ToString();
					kind = KeyKind.NumPad;
					break;

				case VirtualKey.Multiply:
					text = "*";
					kind = KeyKind.NumPad;
					break;
				case VirtualKey.Add:
					text = "+";
					kind = KeyKind.NumPad;
					break;
				case VirtualKey.Subtract:
					text = "-";
					kind = KeyKind.NumPad;
					break;
				case VirtualKey.Decimal:
					text = ".";
					kind = KeyKind.NumPad;
					break;
				case VirtualKey.Divide:
					text = "/";
					kind = KeyKind.NumPad;
					break;

				case VirtualKey.Prior:
					text = "Page Up";
					kind = KeyKind.Normal;
					break;
				case VirtualKey.Next:
					text = "Page Down";
					kind = KeyKind.Normal;
					break;

				case VirtualKey.Oem1:
				case VirtualKey.Oemplus:
				case VirtualKey.Oemcomma:
				case VirtualKey.OemMinus:
				case VirtualKey.OemPeriod:
				case VirtualKey.Oem2:
				case VirtualKey.Oem3:
				case VirtualKey.Oem4:
				case VirtualKey.Oem5:
				case VirtualKey.Oem6:
				case VirtualKey.Oem7:
				case VirtualKey.Oem8:
				case VirtualKey.Oem102:
					text = ((VirtualKeys)key).ToChar().ToString();
					if (Thread.CurrentThread.CurrentUICulture.Name == "ja" && text == "\\")
					{
						text = "¥";
					}
					kind = KeyKind.Normal;
					break;

				default:
					text = key.ToString();
					kind = KeyKind.Normal;
					break;
			}

			return Tuple.Create(text, kind);
		}
	}
}
