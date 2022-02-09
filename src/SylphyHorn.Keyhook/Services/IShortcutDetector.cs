using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace SylphyHorn.Services
{
	public interface IShortcutDetector
	{
		IDisposable Register(IShortcut[] shortcuts);
	}

	public interface IShortcut
	{
		IKeyCombination[] Keys { get; }

		void Action();
	}

	public interface IKeyCombination
	{
		public ModifierKeys Modifiers { get; }

		public Key Trigger { get; }
	}
}
