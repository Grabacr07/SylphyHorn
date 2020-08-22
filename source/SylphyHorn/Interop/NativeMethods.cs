using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using MetroRadiance.Interop.Win32;

namespace SylphyHorn.Interop
{
	public static class NativeMethods
	{
		[DllImport("user32.dll")]
		public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

		[DllImport("user32.dll")]
		public static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);
		
		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetCursorPos(out POINT lpPoint);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, EnumMonitorsDelegate lpfnEnum, IntPtr dwData);

		[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFOEX lpmi);

		[DllImport("user32.dll", ExactSpelling = true)]
		public static extern IntPtr MonitorFromPoint(POINT pt, MonitorDefaultTo dwFlags);

		[DllImport("user32.dll", ExactSpelling = true)]
		public static extern IntPtr MonitorFromWindow(IntPtr hWnd, MonitorDefaultTo dwFlags);

		[DllImport("User32.dll", ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, int crKey, byte bAlpha, LayeredWindowAttributes dwFlags);

		[DllImport("dxva2.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, ref int pdwNumberOfPhysicalMonitors);

		[DllImport("dxva2.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, uint dwPhysicalMonitorArraySize, [Out] PHYSICAL_MONITOR[] pPhysicalMonitorArray);

		[DllImport("dxva2.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool DestroyPhysicalMonitors(uint dwPhysicalMonitorArraySize, PHYSICAL_MONITOR[] pPhysicalMonitorArray);

		[DllImport("user32.dll")]
		internal static extern uint MapVirtualKey(uint uCode, MapVirtualKey uMapType);
	}

	public delegate bool EnumMonitorsDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);
}
