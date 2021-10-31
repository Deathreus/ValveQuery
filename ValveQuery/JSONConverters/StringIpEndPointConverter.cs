using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

namespace ValveQuery
{
	internal class StringIpEndPointConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType) => objectType == typeof(IPEndPoint);

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			string text = reader.ReadAsString();
			if (objectType == typeof(QueryCollection<IPEndPoint>))
			{
				var list = new List<IPEndPoint>();
				while (Regex.Match(text, "^(\\d{1,3}.\\d{1,3}.\\d{1,3}.\\d{1,3}):(\\d{1,5})$").Success)
				{
					list.Add(Util.ToIPEndPoint(text));
					text = reader.ReadAsString();
					if (String.IsNullOrEmpty(text))
					{
						break;
					}
				}

				return new QueryCollection<IPEndPoint>(list);
			}

			if (objectType == typeof(IPEndPoint) && Regex.Match(text, "^(\\d{1,3}.\\d{1,3}.\\d{1,3}.\\d{1,3}):(\\d{1,5})$").Success)
			{
				return Util.ToIPEndPoint(text);
			}

			return null;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value.GetType() == typeof(QueryCollection<IPEndPoint>))
			{
				var queryCollection = value as QueryCollection<IPEndPoint>;
				writer.WriteValue(queryCollection.ToString());
			}
			else if (value.GetType() == typeof(IPEndPoint))
			{
				var iPEndPoint = (IPEndPoint)value;
				writer.WriteValue(iPEndPoint.ToString());
			}
		}
	}
}
