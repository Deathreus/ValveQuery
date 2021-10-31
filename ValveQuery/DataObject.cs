using System;

using Newtonsoft.Json;

namespace ValveQuery
{
	/// <summary>
	/// Base of all data objects in this library.
	/// </summary>
	[Serializable]
	public class DataObject
	{
		[NonSerialized]
		internal JsonConverter[] Converters;

		/// <summary>
		/// Returns Json string.
		/// </summary>
		/// <returns>Json string.</returns>
		public override string ToString()
		{
			var settings = new JsonSerializerSettings
			{
				ContractResolver = new OriginalNameContractResolver(),
				Formatting = Formatting.Indented
			};

			if (Converters != null && Converters.Length != 0)
			{
				settings.Converters = Converters;
			}

			return JsonConvert.SerializeObject(this, settings);
		}
	}
}
