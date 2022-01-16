using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using YouiToolkit.Design;

namespace YouiToolkit.Models
{
    internal class AvoidObstacleDragPointSerializeObject
    {
        [JsonIgnore]
        public const string Decelerate = "decelerate";
        [JsonIgnore]
        public const string EmergencyStop = "emergency_stop";
        [JsonIgnore]
        public const string Stop = "stop";

        [JsonProperty("area")]
        public Dictionary<string, Dictionary<string, float[][]>> Area = new();

        [JsonProperty("sensitivity")]
        public float Sensitivity { get; set; } = 0.5f;

        [JsonProperty("delay_start")]
        public int DelayStart { get; set; } = default;

        [JsonProperty("delay_end")]
        public int DelayEnd { get; set; } = default;

        public AvoidObstacleDragPointSerializeObject()
        {
            DebugCreate();
        }

        public static AvoidObstacleDragPointSerializeObject Create(AvoidObstacleStock stock)
        {
            AvoidObstacleDragPointSerializeObject jsonObject = new();

            for (int i = default; i < stock.Length; i++)
            {
                AvoidObstacleDragPointSetBlock setBlock = stock[i];
                var d = new Dictionary<string, float[][]>();

                foreach (var set in setBlock.DragPointSets)
                {
                    PolarF[] polars = set.ToPolars();
                    string typeAs = set.TypeAs switch
                    {
                        AvoidObstacleAreaTypeAs.Decelerate => Decelerate,
                        AvoidObstacleAreaTypeAs.Stop => Stop,
                        AvoidObstacleAreaTypeAs.EmergencyStop => EmergencyStop,
                        _ => throw new ArgumentException(),
                    };
                    
                    float[][] v = new float[polars.Length][];

                    for (int j = default; j < v.Length; j++)
                    {
                        v[j] = polars[j].ToFloatArray();
                    }
                    d.Add(typeAs, v);
                }
                jsonObject.Area.Add($"{setBlock.Index + 1}", d);
            }

            return jsonObject;
        }

        [Conditional("DEBUG_CREATE_JSON_OBJECT")]
        public void DebugCreate()
        {
            for (int no = 1; no <= 16; no++)
            {
                var d = new Dictionary<string, float[][]>();

                foreach (var typeAs in new string[] { Decelerate, EmergencyStop, Stop })
                {
                    float[][] v = new float[32][];

                    for (int i = default; i < v.Length; i++)
                    {
                        v[i] = new float[] { -3.1415926535897931f, 2f };
                    }

                    d.Add(typeAs, v);
                }
                Area.Add(no.ToString(), d);
            }
        }
    }
}
