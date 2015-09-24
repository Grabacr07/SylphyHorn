using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using MetroTrilithon.Lifetime;

namespace SylphyHorn.Services
{
	public static class Helpers
	{
		public static IDisposable GetDeferral(this SuspendingEventArgs args)
		{
			var deferral = args.SuspendingOperation.GetDeferral();
			return Disposable.Create(() => deferral.Complete());
		}
	}
}
