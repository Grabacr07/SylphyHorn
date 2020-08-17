using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using MetroRadiance.Interop.Win32;
using SylphyHorn.Interop;

namespace SylphyHorn.Services
{
	internal struct Monitor
	{
		public string Name { get; }

		public RECT MonitorArea { get; }

		public RECT WorkArea { get; }

		public Monitor(string name, RECT monitorArea, RECT workArea)
		{
			this.Name = name;
			this.MonitorArea = monitorArea;
			this.WorkArea = workArea;
		}
	}

	internal static class MonitorHelper
	{
		public static Monitor[] GetMonitors() => GetMonitorsInternal(true);

		public static Monitor GetAreaFromHwnd(IntPtr hWnd) => GetMonitorInternal(GetHmonitorFromWindow(hWnd));

		public static Monitor GetAreaFromHmonitor(IntPtr hMonitor) => GetMonitorInternal(hMonitor);

		public static IntPtr[] GetHmonitors()
		{
			var collection = new Collection<IntPtr>();
			EnumHmonitors(hMonitor => collection.Add(hMonitor));
			return collection.ToArray();
		}

		public static IntPtr GetHmonitorFromIndex(int index)
		{
			var ret = IntPtr.Zero;
			EnumHmonitors(hMonitor => ret = hMonitor, index + 1);
			return ret;
		}

		public static IntPtr GetCurrentHmonitor()
		{
			POINT pt;
			if (!NativeMethods.GetCursorPos(out pt)) throw new Win32Exception(Marshal.GetLastWin32Error());

			var hWorkingMonitor = NativeMethods.MonitorFromPoint(pt, MonitorDefaultTo.MONITOR_DEFAULTTONEAREST);
			if (hWorkingMonitor == (IntPtr)MonitorDefaultTo.MONITOR_DEFAULTTONEAREST) throw new Win32Exception();

			return hWorkingMonitor;
		}

		public static IntPtr GetHmonitorFromWindow(IntPtr hWnd)
		{
			var hMonitor = User32.MonitorFromWindow(hWnd, MonitorDefaultTo.MONITOR_DEFAULTTONEAREST);
			if (hMonitor == (IntPtr)MonitorDefaultTo.MONITOR_DEFAULTTONEAREST) throw new Win32Exception();

			return hMonitor;
		}

		private static Monitor[] GetMonitorsInternal(bool additionalRetrive = false)
		{
			var collection = new Collection<Monitor>();
			EnumHmonitors(hMonitor =>
			{
				var monitor = GetMonitorInternal(hMonitor, additionalRetrive);
				collection.Add(monitor);
			});
			return collection.ToArray();
		}

		private static void EnumHmonitors(Action<IntPtr> callback, int maxCount = int.MaxValue)
		{
			var ret = NativeMethods.EnumDisplayMonitors(
				IntPtr.Zero,
				IntPtr.Zero,
				(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr _) =>
				{
					if (--maxCount > 0) callback(hMonitor);
					return true;
				},
				IntPtr.Zero);
			if (!ret) throw new Win32Exception(Marshal.GetLastWin32Error());
		}

		private static Monitor GetMonitorInternal(IntPtr hMonitor, bool additionalRetrive = false)
		{
			var info = new MONITORINFOEX();
			info.cbSize = Marshal.SizeOf(info);

			if (NativeMethods.GetMonitorInfo(hMonitor, ref info))
			{
				var name = info.szDevice;

				if (additionalRetrive)
				{
					var cPhysicalMonitors = 0;
					if (NativeMethods.GetNumberOfPhysicalMonitorsFromHMONITOR(hMonitor, ref cPhysicalMonitors)
						&& cPhysicalMonitors > 0)
					{
						var closing = false;
						var numberOfMonitors = 1u;
						var physicalMonitors = new PHYSICAL_MONITOR[numberOfMonitors];
						try
						{
							if (NativeMethods.GetPhysicalMonitorsFromHMONITOR(hMonitor, numberOfMonitors, physicalMonitors))
							{
								name = physicalMonitors[0].szPhysicalMonitorDescription;
								closing = true;
							}
						}
						finally
						{
							if (closing) NativeMethods.DestroyPhysicalMonitors(numberOfMonitors, physicalMonitors);
						}
					}
				}

				return new Monitor(name, info.rcMonitor, info.rcWork);
			}

			throw new Win32Exception(Marshal.GetLastWin32Error());
		}
	}
}
