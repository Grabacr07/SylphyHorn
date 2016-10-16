using System;
using System.Collections.Generic;
using System.Linq;

namespace SylphyHorn
{
	public class CommandLineArgs : CommandLineArgsBase
	{
		[CommandLineOption(Key = nameof(Setup))]
		public bool Setup { get; private set; }

		[CommandLineOption(Key = nameof(CanSettings))]
		public bool CanSettings { get; private set; } = true;

		[CommandLineOption(Key = nameof(Restarted))]
		public int? Restarted { get; private set; }

		public CommandLineArgs()
			: base(new string[0])
		{
		}

		public CommandLineArgs(string[] args)
			: base(args)
		{
		}
	}

	public abstract class CommandLineArgsBase
	{
		public string[] OriginalArgs { get; }

		public CommandLineOption[] Options { get; }

		protected CommandLineArgsBase(string[] args, string keyPrefixOverride = null, string[] separatorsOverride = null)
		{
			var options = new List<CommandLineOption>();

			foreach (var property in this.GetType().GetProperties())
			{
				var attr = property.GetCustomAttributes(typeof(CommandLineOptionAttribute), true)
					.OfType<CommandLineOptionAttribute>()
					.FirstOrDefault();
				if (attr == null) continue;

				var dic = args
					.Select(x => x.Split(separatorsOverride ?? attr.Separators, StringSplitOptions.RemoveEmptyEntries))
					.GroupBy(xs => xs[0], (k, ys) => ys.Last()) // 重複の場合は後ろの引数を優先
					.ToDictionary(xs => xs[0].ToLower(), xs => xs.Length == 1 ? null : xs[1]);

				var prefix = keyPrefixOverride ?? attr.KeyPrefix;
				string valueString;

				if ((!string.IsNullOrEmpty(attr.Key) && dic.TryGetValue(prefix + attr.Key.ToLower(), out valueString)) ||
					(!string.IsNullOrEmpty(attr.ShortKey) && dic.TryGetValue(prefix + attr.ShortKey.ToLower(), out valueString)))
				{
					var option = new CommandLineOption(attr.Key, property.PropertyType, valueString, attr.KeyPrefix, attr.Separators.First());
					if (option.ConvertException != null)
					{
						System.Diagnostics.Debug.WriteLine(option.ConvertException);
						continue;
					}

					property.SetValue(this, option.Value);
					options.Add(option);
				}
			}

			this.OriginalArgs = args;
			this.Options = options.ToArray();
		}

		public string GetKey(string propertyName)
		{
			var property = this.GetType().GetProperty(propertyName);
			var attr = property.GetCustomAttributes(typeof(CommandLineOptionAttribute), true)
				.OfType<CommandLineOptionAttribute>()
				.First();

			return attr.Key;
		}

		public CommandLineOption CreateOption(string propertyName, string value)
		{
			var property = this.GetType().GetProperty(propertyName);
			var attr = property.GetCustomAttributes(typeof(CommandLineOptionAttribute), true)
				.OfType<CommandLineOptionAttribute>()
				.First();

			return new CommandLineOption(attr.Key, property.PropertyType, value, attr.KeyPrefix, attr.Separators.First());
		}

		public class CommandLineOption
		{
			public string Key { get; }

			public Type Type { get; }

			public string ValueString { get; }

			public object Value { get; }

			public string KeyPrefix { get; }

			public string Separator { get; }

			public Exception ConvertException { get; }

			public CommandLineOption(string key, Type type, string valueString, string keyPrefix, string separator)
			{
				if (valueString == null && type != typeof(bool)) throw new NotSupportedException();

				this.Key = key;
				this.Type = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)
					? type.GetGenericArguments().First()
					: type;
				this.ValueString = valueString;

				try
				{
					this.Value = Convert.ChangeType(this.ValueString ?? true.ToString(), this.Type);
				}
				catch (Exception ex)
				{
					this.ConvertException = ex;
				}

				this.KeyPrefix = keyPrefix;
				this.Separator = separator;
			}

			public override string ToString()
			{
				return $"{this.KeyPrefix}{this.Key}{(this.ValueString == null ? "" : $"{this.Separator}{this.ValueString}")}";
			}
		}
	}

	public class CommandLineOptionAttribute : Attribute
	{
		public string Key { get; set; }

		public string ShortKey { get; set; }

		public string KeyPrefix { get; set; } = "-";

		public string[] Separators { get; set; } = { "=" };
	}
}
