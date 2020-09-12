using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class SensorEnergyLog
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        public string SensorId { get; set; }
        [Required]
        public long SensorDimTimeId { get; set; }
        [Required]
        public long UnixTime { get; set; }
        [Required, Range(0.00, float.MaxValue)]
        public float Duration { get; set; }
        [Required, Range(0.00, float.MaxValue)]
        public float Watts1 { get; set; }
        [Required, Range(0.00, float.MaxValue)]
        public float Watts2 { get; set; }
        [Required, Range(0.00, float.MaxValue)]
        public float Watts3 { get; set; }
        [Required, Range(0.00, float.MaxValue)]
        public float WattsTotal { get; set; }
        [Required]
        public float ConvertToUnits { get; set; }
        internal void CalculateDuration(SensorEnergyLog lastEnergyLog)
        {
            if (lastEnergyLog == null ) return;
        }

        internal static SensorEnergyLog Parse(Sensor sensor, Func<long,long> GetSensorDimTimeId, string contentLogItem)
        {
            var sensorEnergyLog = new SensorEnergyLog()
            {
                SensorId = sensor.Id,
                SensorDimTimeId = GetSensorDimTimeId(1574608324),
                UnixTime = 1574608324,
                Duration = sensor.LogDurationMode,
                Watts1 = 1 * sensor.DefaultToConvert,
                Watts2 = 2 * sensor.DefaultToConvert,
                Watts3 = 3 * sensor.DefaultToConvert,
                WattsTotal = 6 * sensor.DefaultToConvert,
                ConvertToUnits = sensor.DefaultToConvert
            };
            return sensorEnergyLog;
        }
    }
}
