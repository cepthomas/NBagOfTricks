using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;
using System.Windows.Forms;
using NBagOfTricks;

namespace NBagOfTricks
{
    /// <summary>Serialize prettier.</summary>
    public class JsonColorConverter : JsonConverter<Color>
    {
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return Color.FromName(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, Color color, JsonSerializerOptions options)
        {
            writer.WriteStringValue(color.Name);
        }
    }

    /// <summary>Serialize prettier.</summary>
    public class JsonPointFConverter : JsonConverter<PointF>
    {
        public override PointF Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var parts = StringUtils.SplitByToken(reader.GetString(), ",");
            PointF pt = new(float.Parse(parts[0]), float.Parse(parts[1]));
            return pt;
        }

        public override void Write(Utf8JsonWriter writer, PointF pt, JsonSerializerOptions options)
        {
            writer.WriteStringValue($"{pt.X},{pt.Y}");
        }
    }

    /// <summary>Serialize prettier.</summary>
    public class JsonFontConverter : JsonConverter<Font>
    {
        public override Font Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var parts = StringUtils.SplitByToken(reader.GetString(), ",");
            Font font = new(parts[0], float.Parse(parts[1]));
            return font;
        }

        public override void Write(Utf8JsonWriter writer, Font font, JsonSerializerOptions options)
        {
            writer.WriteStringValue($"{font.FontFamily.Name},{font.Size}");
        }
    }
}
