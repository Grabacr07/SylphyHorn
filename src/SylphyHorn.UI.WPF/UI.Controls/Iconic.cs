using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SylphyHorn.UI.Controls;

/// <remarks>
/// see also: https://docs.microsoft.com/en-us/windows/apps/design/style/segoe-ui-symbol-font
/// </remarks>
public static class Iconic
{
	#region Content attached property

	public static readonly DependencyProperty ContentProperty
		= DependencyProperty.RegisterAttached(
			nameof(ContentProperty).GetPropertyName(),
			typeof(string),
			typeof(Iconic),
			new PropertyMetadata(default(string), HandleContentChanged));

	private static void HandleContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is not ContentControl button) return;
		if (e.NewValue is not string s) return;

		var split = s.Split("|", StringSplitOptions.RemoveEmptyEntries);
		if (split.Length < 1) return;

		var panel = new StackPanel()
		{
			Orientation = Orientation.Horizontal,
		};
		var icon = new TextBlock()
		{
			Text = split[0],
			FontFamily = new FontFamily("Segoe MDL2 Assets"),
			Margin = new Thickness(0, 3, 0, 0),
		};
		panel.Children.Add(icon);

		if (split.Length >= 2)
		{
			var text = new TextBlock()
			{
				Text = split[1],
				Margin = new Thickness(12, 0, 0, 0),
			};
			panel.Children.Add(text);
		}

		button.Content = panel;
	}

	public static void SetContent(ContentControl element, string value)
		=> element.SetValue(ContentProperty, value);

	public static string GetContent(ContentControl element)
		=> (string)element.GetValue(ContentProperty);

	#endregion
}
