﻿using MetroRadiance.Interop.Win32;
using System;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace SylphyHorn.Interop
{
	[StructLayout(LayoutKind.Sequential)]
	public struct COLORREF
	{
		public byte R;
		public byte G;
		public byte B;
	}

	public enum DesktopSlideshowOptions
	{
		DSO_SHUFFLEIMAGES = 0x01,
	}

	public enum DesktopSlideshowState
	{
		DSS_ENABLED = 0x01,
		DSS_SLIDESHOW = 0x02,
		DSS_DISABLED_BY_REMOTE_SESSION = 0x04,
	}

	public enum DesktopSlideshowDirection
	{
		DSD_FORWARD = 0,
		DSD_BACKWARD,
	}

	public enum DesktopWallpaperPosition
	{
		DWPOS_CENTER = 0,
		DWPOS_TILE,
		DWPOS_STRETCH,
		DWPOS_FIT,
		DWPOS_FILL,
		DWPOS_SPAN,
	}

	[ComImport]
	[Guid("B92B56A9-8B55-4E14-9A89-0199BBB6F93B")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IDesktopWallpaper
	{
		void SetWallpaper([MarshalAs(UnmanagedType.LPWStr)] string monitorID, [MarshalAs(UnmanagedType.LPWStr)] string wallpaper);

		[return: MarshalAs(UnmanagedType.LPWStr)]
		string GetWallpaper([MarshalAs(UnmanagedType.LPWStr)] string monitorID);

		[return: MarshalAs(UnmanagedType.LPWStr)]
		string GetMonitorDevicePathAt(uint monitorIndex);

		uint GetMonitorDevicePathCount();

		RECT GetMonitorRECT([MarshalAs(UnmanagedType.LPWStr)] string monitorID);

		void SetBackgroundColor([MarshalAs(UnmanagedType.U4)] COLORREF color);
		
		COLORREF GetBackgroundColor();

		void SetPosition([MarshalAs(UnmanagedType.I4)] DesktopWallpaperPosition position);

		[return: MarshalAs(UnmanagedType.I4)]
		DesktopWallpaperPosition GetPosition();

		void SetSlideshow(IntPtr /* IShellItemArray* */ items);

		IntPtr /* IShellItemArray* */ GetSlideshow();

		void SetSlideshowOptions(DesktopSlideshowOptions options, uint slideshowTick);

		void GetSlideshowOptions(out DesktopSlideshowOptions options, out uint slideshowTick);

		void AdvanceSlideshow([MarshalAs(UnmanagedType.LPWStr)] string monitorID, [MarshalAs(UnmanagedType.I4)] DesktopSlideshowDirection direction);

		DesktopSlideshowState GetStatus();

		void Enable([MarshalAs(UnmanagedType.Bool)] bool enable);
	}

	public static class DesktopWallpaperFactory
	{

		[ComImport]
		[Guid("C2CF3110-460E-4fc1-B9D0-8A1C0C9CC4BD")]
		class DesktopWallpaperCoclass { }

		public static IDesktopWallpaper Create()
		{
			return (IDesktopWallpaper)new DesktopWallpaperCoclass();
		}
	}
}