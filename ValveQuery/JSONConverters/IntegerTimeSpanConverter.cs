using System;

using Newtonsoft.Json;

namespace ValveQuery
{
	internal class IntegerTimeSpanConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType) => objectType == typeof(TimeSpan);

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			TimeSpan timeSpan = TimeSpan.Zero;
			if (Double.TryParse(reader.Value.ToString(), out double result))
			{
				timeSpan = TimeSpan.FromMinutes(result);
			}

			return timeSpan;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => writer.WriteValue(((TimeSpan)value).ToString());
	}
}
