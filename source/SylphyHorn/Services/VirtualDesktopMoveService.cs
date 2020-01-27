using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using WindowsDesktop;
using SylphyHorn.Interop;

namespace SylphyHorn.Services
{
	public static class VirtualDesktopMoveService
	{
		public static void MoveLeft()
		{
			var target = VirtualDesktop.Current.GetLeft();
			if (target == null)
				return;
			SwapDesktops(VirtualDesktop.Current, target);
			target.Switch();
		}

		public static void MoveRight()
		{
			var target = VirtualDesktop.Current.GetRight();
			if (target == null)
				return;
			SwapDesktops(VirtualDesktop.Current, target);
			target.Switch();
		}

		private static void SwapDesktops(VirtualDesktop source, VirtualDesktop target)
		{
			var srcWindows = new List<IntPtr>();
			var dstWindows = new List<IntPtr>();
			foreach (var window in GetAllWindows())
			{
				var desktop = VirtualDesktop.FromHwnd(window);
				if (desktop == source)
					srcWindows.Add(window);
				else if (desktop == target)
					dstWindows.Add(window);
			}

			foreach (var window in srcWindows)
				VirtualDesktopHelper.MoveToDesktop(window, target);
			foreach (var window in dstWindows)
				VirtualDesktopHelper.MoveToDesktop(window, source);
		}

		private static IEnumerable<IntPtr> GetAllWindows()
		{
			return GetChildWindows(IntPtr.Zero).ToList();
		}

		private static IEnumerable<IntPtr> GetChildWindows(IntPtr parent)
		{
			var result = new List<IntPtr>();
			var listHandle = GCHandle.Alloc(result);
			try
			{
				var childProc = new NativeMethods.Win32Callback(EnumWindow);
				NativeMethods.EnumChildWindows(parent, childProc, GCHandle.ToIntPtr(listHandle));
			}
			finally
			{
				if (listHandle.IsAllocated)
					listHandle.Free();
			}
			return result;
		}

		private static bool EnumWindow(IntPtr handle, IntPtr pointer)
		{
			var gch = GCHandle.FromIntPtr(pointer);
			if (!(gch.Target is List<IntPtr> list))
			{
				throw new InvalidCastException("GCHandle Target could not be cast as List<IntPtr>");
			}
			list.Add(handle);
			//  return null to cancel?
			return true;
		}
	}
}
