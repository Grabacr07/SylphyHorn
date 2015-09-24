using System;

namespace SylphyHorn.Bootstrapper
{
	public struct InstallResult : IEquatable<InstallResult>
	{
		public bool IsSucceeded { get; }

		public bool RestartRequired { get; }

		public InstallResult(bool isSucceeded, bool restartRequired)
		{
			this.IsSucceeded = isSucceeded;
			this.RestartRequired = restartRequired;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (this.IsSucceeded.GetHashCode() * 397) ^ this.RestartRequired.GetHashCode();
			}
		}

		public override bool Equals(object obj)
		{
			return obj is InstallResult && base.Equals((InstallResult)obj);
		}

		public bool Equals(InstallResult other)
		{
			return this.IsSucceeded == other.IsSucceeded && this.RestartRequired == other.RestartRequired;
		}

		public override string ToString()
		{
			return $"{(this.IsSucceeded ? "Succeeded" : "Failed")}{(this.RestartRequired ? ", Restart required" : "")}";
		}

		public static bool operator ==(InstallResult result1, InstallResult result2)
		{
			return result1.Equals(result2);
		}

		public static bool operator !=(InstallResult result1, InstallResult result2)
		{
			return !(result1 == result2);
		}
	}
}
