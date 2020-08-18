using System;

namespace SylphyHorn.UI.Bindings
{
	[Flags]
	public enum WindowPlacement : byte
	{
		Default,
		TopLeft = 1,
		TopCenter,
		TopRight,
		Center = 5,
		BottomLeft = 7,
		BottomCenter,
		BottomRight,

		OutsideY = 0b0010_0000,
	}
}