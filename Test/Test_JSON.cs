using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json.Serialization;
using System.Text.Json;
using NBagOfTricks;
using NBagOfTricks.PNUT;

namespace NBagOfTricks.Test
{
    public class JSON_CONVERTERS : TestSuite
    {
        public override void RunSuite()
        {
            UT_INFO("Tests json converters.");

            ConverterTarget ct1 = new();

            JsonSerializerOptions opts = new() { WriteIndented = true };
            string json = JsonSerializer.Serialize(ct1, opts);

            UT_EQUAL(json, "{\r\n  \"ControlColor\": \"MediumOrchid\",\r\n  \"Position\": \"101.1,532.22\",\r\n  \"SomeRect\": \"34,55,890,75\",\r\n  \"PrettyFace\": \"Cooper Black,9\"\r\n}");

            var ser = "{\"ControlColor\": \"Pink\", \"Position\": \"45.88,502.01\", \"SomeRect\": \"505,80,75,66\", \"PrettyFace\": \"Arial, 14\"}";

            ConverterTarget? ct2 = JsonSerializer.Deserialize<ConverterTarget>(ser);

            UT_NULL(ct2);
            if(ct2 is not null)
            {
                UT_EQUAL(ct2.ControlColor.ToString(), "Pink");
                UT_EQUAL(ct2.Position.X, 45.88);
                UT_EQUAL(ct2.SomeRect.Width, 75);
                UT_EQUAL(ct2.PrettyFace.Size, 14);
            }
        }
    }

    public class ConverterTarget
    {
        [JsonConverter(typeof(JsonColorConverter))]
        public Color ControlColor { get; set; } = Color.MediumOrchid;

        [JsonConverter(typeof(JsonPointFConverter))]
        public PointF Position { get; set; } = new(101.1f, 532.22f);

        [JsonConverter(typeof(JsonRectangleConverter))]
        public Rectangle SomeRect { get; set; } = new(34, 55, 890, 75);

        [JsonConverter(typeof(JsonFontConverter))]
        public Font PrettyFace { get; set; } = new Font("Cooper Black", 9F);
    }
}
