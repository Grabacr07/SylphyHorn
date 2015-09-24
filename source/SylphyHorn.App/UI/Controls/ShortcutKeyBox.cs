using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using SylphyHorn.Serialization;
using SylphyHorn.Services;

namespace SylphyHorn.UI.Controls
{
	[TemplatePart(Name = PART_Modifiers, Type = typeof(ItemsControl))]
	[TemplatePart(Name = PART_KeyIcon, Type = typeof(KeyIcon))]
	[TemplatePart(Name = PART_Prompt, Type = typeof(TextBlock))]
	public class ShortcutKeyBox : TextBox
	{
		private const string PART_Modifiers = "PART_Modifiers";
		private const string PART_KeyIcon = "PART_KeyIcon";
		private const string PART_Prompt = "PART_Prompt";

		private static readonly IReadOnlyDictionary<VirtualKey, VirtualKey[]> _virtualModifiers = new Dictionary<VirtualKey, VirtualKey[]>
		{
			{ VirtualKey.Control, new[] { VirtualKey.LeftControl, VirtualKey.RightControl, } },
			{ VirtualKey.Shift, new[] { VirtualKey.LeftShift, VirtualKey.RightShift, } },
			{ VirtualKey.Menu, new[] { VirtualKey.LeftMenu, VirtualKey.RightMenu, } },
		};

		private readonly HashSet<VirtualKey> _pressedModifiers = new HashSet<VirtualKey>();
		private VirtualKey _pressedKey = VirtualKey.None;
		private ItemsControl _modifiersPresenter;
		private KeyIcon _keyPresenter;
		private TextBlock _prompt;

		#region Current dependency property

		public int[] Current
		{
			get { return (int[])this.GetValue(CurrentProperty); }
			set { this.SetValue(CurrentProperty, value); }
		}
		public static readonly DependencyProperty CurrentProperty =
			DependencyProperty.Register(nameof(Current), typeof(int[]), typeof(ShortcutKeyBox), new PropertyMetadata(null, CurrentPropertyChangedCallback));

		private static void CurrentPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var instance = (ShortcutKeyBox)d;
			instance.UpdateVisual();
		}

		private ShortcutKey? CurrentKey
		{
			get { return this.Current?.ToShortcutKey(); }
			set { this.Current = value?.ToSerializable(); }
		}

		#endregion


		public ShortcutKeyBox()
		{
			this.DefaultStyleKey = typeof(ShortcutKeyBox);
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			this._modifiersPresenter = this.GetTemplateChild(PART_Modifiers) as ItemsControl;
			this._keyPresenter = this.GetTemplateChild(PART_KeyIcon) as KeyIcon;
			this._prompt = this.GetTemplateChild(PART_Prompt) as TextBlock;

			this.UpdateVisual();
		}

		protected override void OnGotFocus(RoutedEventArgs e)
		{
			base.OnGotFocus(e);
			this.UpdateVisual();
		}

		protected override void OnKeyDown(KeyRoutedEventArgs e)
		{
			if (!this.IsReadOnly)
			{
				if (e.KeyStatus.RepeatCount == 1)
				{
					var key = e.Key;
					if (key == VirtualKey.Back)
					{
						this._pressedModifiers.Clear();
						this._pressedKey = VirtualKey.None;
					}
					else if (key.IsModifyKey())
					{
						this._pressedModifiers.Add(key);
					}
					else if (_virtualModifiers.ContainsKey(key))
					{
						foreach (var modifier in _virtualModifiers[key])
						{
							if (Window.Current.CoreWindow.GetKeyState(modifier).HasFlag(CoreVirtualKeyStates.Down))
							{
								this._pressedModifiers.Add(modifier);
								break;
							}
						}
					}
					else
					{
						this._pressedKey = key;
					}

					this.CurrentKey = this._pressedModifiers.Any() && this._pressedKey != VirtualKey.None
						? new ShortcutKey(this._pressedKey, this._pressedModifiers.ToArray())
						: (ShortcutKey?)null;

					this.UpdateVisual();
				}

				e.Handled = true;
			}

			base.OnKeyDown(e);
		}

		protected override void OnKeyUp(KeyRoutedEventArgs e)
		{
			if (!this.IsReadOnly)
			{
				if (e.KeyStatus.RepeatCount == 1)
				{
					var key = e.Key;
					if (key.IsModifyKey())
					{
						this._pressedModifiers.Remove(key);
					}
					else if (_virtualModifiers.ContainsKey(key))
					{
						foreach (var modifier in _virtualModifiers[key])
						{
							if (!Window.Current.CoreWindow.GetKeyState(modifier).HasFlag(CoreVirtualKeyStates.Down))
							{
								this._pressedModifiers.Remove(modifier);
							}
						}
					}

					this._pressedKey = VirtualKey.None;
					this.UpdateVisual();
				}

				e.Handled = true;
			}

			base.OnKeyUp(e);
		}

		private void UpdateVisual()
		{
			var key = this.CurrentKey ?? new ShortcutKey(this._pressedKey, this._pressedModifiers.ToArray());

			if (this._modifiersPresenter != null)
			{
				this._modifiersPresenter.ItemsSource = key.Modifiers;
				this._modifiersPresenter.Visibility = key.Modifiers.Length == 0 ? Visibility.Collapsed : Visibility.Visible;
			}
			if (this._keyPresenter != null)
			{
				this._keyPresenter.VirtualKey = key.Key;
				this._keyPresenter.Visibility = key.Key == VirtualKey.None ? Visibility.Collapsed : Visibility.Visible;
			}
			if (this._prompt != null)
			{
				this._prompt.Text = key == ShortcutKey.None ? "Press shortcut keys." : "";
			}
		}
	}
}
