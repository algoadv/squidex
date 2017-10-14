// ==========================================================================
//  AppConverter.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System;
using Newtonsoft.Json;
using Squidex.Domain.Apps.Core.Apps.Json.V1;

namespace Squidex.Domain.Apps.Core.Apps.Json
{
    public sealed class AppConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is App app)
            {
                writer.WriteStartObject();

                writer.WritePropertyName("version");
                writer.WriteValue("1.0");

                writer.WritePropertyName("value");

                serializer.Serialize(writer, V1JsonApp.Create(app));

                writer.WriteEndObject();
            }
            else
            {
                writer.WriteNull();
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            App app = null;

            if (reader.TokenType == JsonToken.StartObject)
            {
                reader.Read();
                reader.Read();

                var version = reader.ReadAsString();

                if (version != "1.0")
                {
                    throw new JsonException($"Invalid version, expected 1.0, got {version}");
                }

                reader.Read();

                app = serializer.Deserialize<V1JsonApp>(reader).ToApp();

                reader.Read();
            }
            else
            {
                reader.Read();
            }

            return app;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(App);
        }
    }
}
