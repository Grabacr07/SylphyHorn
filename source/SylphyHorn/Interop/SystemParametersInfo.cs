using System;
// ReSharper disable InconsistentNaming

namespace SylphyHorn.Interop
{
	public enum SystemParametersInfo
	{
		SPI_SETDESKWALLPAPER = 20,
	}

	[Flags]
	public enum SystemParametersInfoFlag
	{
		SPIF_UPDATEINIFILE = 0x01,
		SPIF_SENDWININICHANGE = 0x02,
	}
}
