using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace SylphyHorn.Bootstrapper.UI.Controls
{
	public static class ContentControlHelper
	{
		public static readonly DependencyProperty ContentChangedAnimationProperty = DependencyProperty.RegisterAttached(
			"ContentChangedAnimation", typeof(Storyboard), typeof(ContentControlHelper), new PropertyMetadata(default(Storyboard), ContentChangedAnimationPropertyChangedCallback));

		public static void SetContentChangedAnimation(ContentControl element, Storyboard value)
		{
			element.SetValue(ContentChangedAnimationProperty, value);
		}

		public static Storyboard GetContentChangedAnimation(ContentControl element)
		{
			return (Storyboard)element.GetValue(ContentChangedAnimationProperty);
		}

		private static void ContentChangedAnimationPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var contentControl = d as ContentControl;
			if (contentControl == null) return;

			var propertyDescriptor = DependencyPropertyDescriptor.FromProperty(ContentControl.ContentProperty, typeof(ContentControl));

			propertyDescriptor.RemoveValueChanged(contentControl, ContentChangedHandler);
			propertyDescriptor.AddValueChanged(contentControl, ContentChangedHandler);
		}

		private static void ContentChangedHandler(object sender, EventArgs eventArgs)
		{
			var animateObject = (ContentControl)sender;
			var storyboard = GetContentChangedAnimation(animateObject);
			storyboard.Begin(animateObject);
		}
	}
}
