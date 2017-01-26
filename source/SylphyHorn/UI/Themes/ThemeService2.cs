using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using MetroRadiance.Utilities;
using SylphyHorn.Interop;

namespace SylphyHorn.UI.Themes
{
	// Source from https://github.com/Grabacr07/MetroRadiance/blob/master/source/MetroRadiance.Core/Platform/ThemeValue.cs
	public sealed class ThemeService2
	{
		#region singleton members

		public static ThemeService2 Current { get; } = new ThemeService2();

		#endregion

		private IDisposable _transparencyListener;
		private IDisposable _colorPrevalenceListener;
		private IDisposable _highContrastListener;

		private readonly List<ResourceDictionary> _resources = new List<ResourceDictionary>();

		public IDisposable Register(Application app)
		{
			var disposable = this.Register(app.Resources);

			this.UpdateListener();

			return disposable;
		}

		private IDisposable Register(ResourceDictionary rd)
		{
			var accentDic = GetDefaultAccentResource();
			rd.MergedDictionaries.Add(accentDic);
			this._resources.Add(accentDic);

			return Disposable.Create(() =>
			{
				this._resources.Remove(accentDic);
			});
		}


		private static ResourceDictionary GetDefaultAccentResource()
			=> GetAccentResource(
				WindowsTheme2.Transparency.Current,
				WindowsTheme2.ColorPrevalence.Current,
				WindowsTheme2.HighContrast.Current);

		private static ResourceDictionary GetAccentResource(bool transparency, bool colorPreserve, bool highContrast)
		{
			string backgroundName;
			if (highContrast)
			{
				backgroundName = ImmersiveColorNames.ApplicationBackground;
				transparency = false;
			}
			else if (colorPreserve)
			{
				backgroundName = ImmersiveColorNames.SystemAccentDark1;
			}
			else
			{
				backgroundName = ImmersiveColorNames.DarkChromeMedium;
			}

			var background = ImmersiveColorHelper.GetColorByTypeName(backgroundName);
			if (transparency) ChangeAlpha(ref background, 0.8f);
			var headerBackground = ImmersiveColorHelper.GetColorByTypeName(ImmersiveColorNames.SystemAccent);
			if (transparency) ChangeAlpha(ref headerBackground, 0.8f);
			var foreground = ImmersiveColorHelper.GetColorByTypeName(ImmersiveColorNames.SystemTextDarkTheme);
			var activeForeground = ImmersiveColorHelper.GetColorByTypeName(ImmersiveColorNames.SystemAccent);
			var border = highContrast ? SystemColors.WindowFrameColor : Colors.Transparent;
			var borderThickness = highContrast ? new Thickness(2) : new Thickness();
			var dic = new ResourceDictionary
			{
				["AccentBackgroundColorKey"] = background,
				["AccentBackgroundBrushKey"] = new SolidColorBrush(background),
				["AccentHeaderBackgroundColorKey"] = headerBackground,
				["AccentHeaderBackgroundBrushKey"] = new SolidColorBrush(headerBackground),
				["AccentForegroundColorKey"] = foreground,
				["AccentForegroundBrushKey"] = new SolidColorBrush(foreground),
				["AccentActiveForegroundColorKey"] = activeForeground,
				["AccentActiveForegroundBrushKey"] = new SolidColorBrush(activeForeground),
				["AccentBorderColorKey"] = border,
				["AccentBorderBrushKey"] = new SolidColorBrush(border),
				["AccentBorderThickness"] = borderThickness,
			};

			return dic;
		}

		private static void ChangeAlpha(ref Color color, float alpha) => color.A = (byte)(alpha * color.A);

		private void ChangeTranceparencyCore(bool tranceparency)
		{
			this.ChangeCore(GetAccentResource(
				tranceparency,
				WindowsTheme2.ColorPrevalence.Current,
				WindowsTheme2.HighContrast.Current));
		}

		private void ChangeColorPrevalenceCore(bool preserve)
		{
			this.ChangeCore(GetAccentResource(
				WindowsTheme2.Transparency.Current,
				preserve,
				WindowsTheme2.HighContrast.Current));
		}

		private void ChangeHighContrastCore(bool highContrast)
		{
			this.ChangeCore(GetAccentResource(
				WindowsTheme2.Transparency.Current,
				WindowsTheme2.ColorPrevalence.Current,
				highContrast));
		}

		private void ChangeCore(ResourceDictionary dic)
		{
			foreach (var key in dic.Keys.OfType<string>())
			{
				foreach (var resource in this._resources.Where(x => x.Contains(key)))
				{
					resource[key] = dic[key];
				}
			}
		}

		private void UpdateListener()
		{
			if (this._transparencyListener == null)
			{
				this._transparencyListener = WindowsTheme2.Transparency.RegisterListener(x => this.ChangeTranceparencyCore(x));
			}
			if (this._colorPrevalenceListener == null)
			{
				this._colorPrevalenceListener = WindowsTheme2.ColorPrevalence.RegisterListener(x => this.ChangeColorPrevalenceCore(x));
			}
			if (this._highContrastListener == null)
			{
				this._highContrastListener = WindowsTheme2.HighContrast.RegisterListener(x => this.ChangeHighContrastCore(x));
			}
		}
	}
}