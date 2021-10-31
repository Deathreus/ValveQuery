using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;

using Newtonsoft.Json;

namespace ValveQuery
{
	/// <summary>
	/// Wrapper on ReadOnlyCollection that returns its json representation on calling ToString().
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class QueryCollection<T> : ReadOnlyCollection<T>
	{
		/// <inheritdoc />
		public QueryCollection(IList<T> collection)
			: base(collection)
		{
		}

		/// <summary>
		/// Returns Json string.
		/// </summary>
		/// <returns>Json string.</returns>
		public override string ToString()
		{
			if (typeof(T) == typeof(IPEndPoint))
			{
				return JsonConvert.SerializeObject(this, Formatting.Indented, new StringIpEndPointConverter());
			}

			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
	}
}
