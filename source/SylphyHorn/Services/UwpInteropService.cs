using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SylphyHorn.Serialization;

namespace SylphyHorn.Services
{
	internal class UwpInteropService : IDisposable
	{
		private readonly HookService _hookService;

		private bool _current;
		private IDisposable _suspended;

		public UwpInteropService(HookService hookService, GeneralSettings settings)
		{
			this._hookService = hookService;

			if (settings.SuspendKeyDetection)
			{
				this._suspended = hookService.Suspend();
			}

			settings.SuspendKeyDetection.Subscribe(x => this.HandleSuspendedChanged(x));
		}

		private void HandleSuspendedChanged(bool suspended)
		{
			// suspend: false -> true
			if (!this._current && suspended)
			{
				var prev = this._suspended;
				this._suspended = this._hookService.Suspend();
				prev?.Dispose();
			}

			// suspend: true -> false
			else if (this._current && !suspended)
			{
				this._suspended?.Dispose();
				this._suspended = null;
			}

			this._current = suspended;
		}

		public void Dispose()
		{
			this._suspended?.Dispose();
		}
	}
}
