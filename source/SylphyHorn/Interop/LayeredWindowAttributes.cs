using System;

namespace SylphyHorn.Interop
{
	[Flags]
	public enum LayeredWindowAttributes
	{
		/// <summary>LWA_COLORKEY</summary>
		ColorKey = 0x00000001,

		/// <summary>LWA_ALPHA</summary>
		Alpha = 0x00000002,
	}
}
