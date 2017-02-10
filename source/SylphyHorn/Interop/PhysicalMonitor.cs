using System;
using System.Runtime.InteropServices;

namespace SylphyHorn.Interop
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct PHYSICAL_MONITOR
	{
		public IntPtr hPhysicalMonitor;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string szPhysicalMonitorDescription;
	}
}