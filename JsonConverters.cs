using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace Ephemera.NBagOfTricks
{
    /// <summary>Serialize prettier.</summary>
    public class JsonColorConverter : JsonConverter<Color>
    {
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? s = reader.GetString();
            if(s is not null)
            {
                return Color.FromName(s);
            }
            else
            {
                throw new InvalidOperationException("Invalid color string");
            }
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
            string? s = reader.GetString();
            if (s is not null)
            {
                var parts = StringUtils.SplitByToken(s, ",");
                PointF pt = new(float.Parse(parts[0]), float.Parse(parts[1]));
                return pt;
            }
            else
            {
                throw new InvalidOperationException("Invalid PointF");
            }
        }

        public override void Write(Utf8JsonWriter writer, PointF pt, JsonSerializerOptions options)
        {
            writer.WriteStringValue($"{pt.X},{pt.Y}");
        }
    }

    /// <summary>Serialize prettier.</summary>
    public class JsonRectangleConverter : JsonConverter<Rectangle>
    {
        public override Rectangle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? s = reader.GetString();
            if (s is not null)
            {
                var parts = StringUtils.SplitByToken(s, ",");
                Rectangle rect = new(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]), int.Parse(parts[3]));
                return rect;
            }
            else
            {
                throw new InvalidOperationException("Invalid color string");
            }
        }

        public override void Write(Utf8JsonWriter writer, Rectangle rect, JsonSerializerOptions options)
        {
            writer.WriteStringValue($"{rect.X},{rect.Y},{rect.Width},{rect.Height}");
        }
    }

    /// <summary>Serialize prettier.</summary>
    public class JsonFontConverter : JsonConverter<Font>
    {
        public override Font Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? s = reader.GetString();
            if (s is not null)
            {
                var parts = StringUtils.SplitByToken(s, ",");
                Font font = new(parts[0], float.Parse(parts[1]));
                return font;
            }
            else
            {
                throw new InvalidOperationException("Invalid color string");
            }
        }

        public override void Write(Utf8JsonWriter writer, Font font, JsonSerializerOptions options)
        {
            writer.WriteStringValue($"{font.FontFamily.Name},{font.Size}");
        }
    }
}
