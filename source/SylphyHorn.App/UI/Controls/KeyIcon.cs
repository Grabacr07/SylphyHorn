using System;
using System.Collections.Generic;
using System.Linq;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SylphyHorn.UI.Controls
{
	[TemplatePart(Name = PART_KeyText, Type = typeof(TextBlock))]
	[TemplatePart(Name = PART_RightOrLeftText, Type = typeof(TextBlock))]
	[TemplatePart(Name = PART_Symbol, Type = typeof(SymbolIcon))]
	public sealed class KeyIcon : Control
	{
		private const string PART_KeyText = "PART_KeyText";
		private const string PART_RightOrLeftText = "PART_RightOrLeftText";
		private const string PART_Symbol = "PART_Symbol";

		private TextBlock _keyText;
		private TextBlock _rightOrLeftText;
		private SymbolIcon _symbol;

		public KeyIcon()
		{
			this.DefaultStyleKey = typeof(KeyIcon);
		}

		#region VirtualKey dependency property

		public static readonly DependencyProperty VirtualKeyProperty = DependencyProperty.Register(
			nameof(VirtualKey), typeof(VirtualKey), typeof(KeyIcon), new PropertyMetadata(default(VirtualKey), VirtualKeyPropertyChangedCallback));

		public VirtualKey VirtualKey
		{
			get { return (VirtualKey)this.GetValue(VirtualKeyProperty); }
			set { this.SetValue(VirtualKeyProperty, value); }
		}

		private static void VirtualKeyPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var instance = (KeyIcon)d;
			var newValue = (VirtualKey)e.NewValue;

			instance.ChangeText(newValue);
		}

		#endregion

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			this._keyText = this.GetTemplateChild(PART_KeyText) as TextBlock;
			this._rightOrLeftText = this.GetTemplateChild(PART_RightOrLeftText) as TextBlock;
			this._symbol = this.GetTemplateChild(PART_Symbol) as SymbolIcon;

			this.ChangeText(this.VirtualKey);
		}

		private void ChangeText(VirtualKey key)
		{
			if (this._keyText == null) return;

			string text, side;
			Symbol? symbol = null;

			switch (key)
			{
				case VirtualKey.LeftShift:
					text = "Shift";
					side = "Left";
					break;
				case VirtualKey.RightShift:
					text = "Shift";
					side = "Right";
					break;

				case VirtualKey.LeftMenu:
					text = "Alt";
					side = "Left";
					break;
				case VirtualKey.RightMenu:
					text = "Alt";
					side = "Right";
					break;

				case VirtualKey.LeftControl:
					text = "Ctrl";
					side = "Left";
					break;
				case VirtualKey.RightControl:
					text = "Ctrl";
					side = "Right";
					break;

				case VirtualKey.LeftWindows:
					text = "Win";
					side = "Left";
					break;
				case VirtualKey.RightWindows:
					text = "Win";
					side = "Right";
					break;

				case VirtualKey.Up:
					text = "";
					side = "";
					symbol = Symbol.Up;
					break;

				case VirtualKey.Left:
					text = "";
					side = "";
					symbol = Symbol.Back;
					break;

				case VirtualKey.Right:
					text = "";
					side = "";
					symbol = Symbol.Forward;
					break;

				default:
					text = key.ToString();
					side = "";
					break;
			}

			if (this._keyText != null)
			{
				this._keyText.Text = text;
				this._keyText.Visibility = string.IsNullOrEmpty(text) ? Visibility.Collapsed : Visibility.Visible;
			}

			if (this._rightOrLeftText != null)
			{
				this._rightOrLeftText.Text = side;
				this._rightOrLeftText.Visibility = string.IsNullOrEmpty(side) ? Visibility.Collapsed : Visibility.Visible;
			}

			if (this._symbol != null)
			{
				if (symbol.HasValue)
				{
					this._symbol.Symbol = symbol.Value;
					this._symbol.Visibility = Visibility.Visible;
				}
				else
				{
					this._symbol.Visibility = Visibility.Collapsed;
				}
			}
		}
	}
}
