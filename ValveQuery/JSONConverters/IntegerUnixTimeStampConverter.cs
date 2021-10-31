using System;

using Newtonsoft.Json;

namespace ValveQuery
{
	internal class IntegerUnixTimeStampConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType) => objectType == typeof(DateTime);

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			if (Double.TryParse(reader.Value.ToString(), out double result))
			{
				dateTime = dateTime.AddSeconds(result).ToLocalTime();
			}

			return dateTime;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => writer.WriteValue(((DateTime)value).ToString());
	}
}
