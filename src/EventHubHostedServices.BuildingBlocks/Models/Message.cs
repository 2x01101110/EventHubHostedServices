using System;
using System.Runtime.Serialization;

namespace EventHubHostedServices.BuildingBlocks.Models
{
    public class Message
    {
        [DataMember(Name = "id")]
        public int DeviceId { get; set; }

        [DataMember(Name = "time")]
        public DateTime Timestamp { get; set; }

        [DataMember(Name = "readings")]
        public Reading[] Readings { get; set; }
    }

    public class Reading
    {
        [DataMember(Name = "sensorId")]
        public int SensorId { get; set; }

        [DataMember(Name = "value")]
        public double? Value { get; set; }
    }
}
