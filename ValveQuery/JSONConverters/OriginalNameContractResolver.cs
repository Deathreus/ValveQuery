using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ValveQuery
{
	internal class OriginalNameContractResolver : DefaultContractResolver
	{
		protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			IList<JsonProperty> list = base.CreateProperties(type, memberSerialization);
			foreach (JsonProperty item in list)
			{
				item.PropertyName = item.UnderlyingName;
			}

			return list;
		}
	}
}
