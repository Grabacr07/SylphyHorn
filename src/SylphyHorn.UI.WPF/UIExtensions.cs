using System;
using System.Collections.Generic;
using System.Linq;

namespace SylphyHorn;

internal static class UIExtensions
{
	private const string _propertyKeyword = "Property";

	/// <summary>
	/// 'XxxProperty' -> 'Xxx'
	/// </summary>
	public static string GetPropertyName(this string dependencyPropertyName)
		=> dependencyPropertyName.Equals(_propertyKeyword, StringComparison.OrdinalIgnoreCase) == false
			&& dependencyPropertyName.EndsWith(_propertyKeyword, StringComparison.Ordinal)
				? dependencyPropertyName[..dependencyPropertyName.LastIndexOf(_propertyKeyword, StringComparison.Ordinal)]
				: dependencyPropertyName;
}
