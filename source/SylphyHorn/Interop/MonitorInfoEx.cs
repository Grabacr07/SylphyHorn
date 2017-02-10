using MetroRadiance.Interop.Win32;
using System.Runtime.InteropServices;

namespace SylphyHorn.Interop
{
	public enum MONITORINFOF : uint
	{
		MONITORINFOF_PRIMARY = 1
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct MONITORINFOEX
	{
		public int cbSize;
		public RECT rcMonitor;
		public RECT rcWork;
		public MONITORINFOF dwFlags;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string szDevice;
	}
}