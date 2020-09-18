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
            this.Duration = (this.UnixTime - lastEnergyLog.UnixTime);
        }
        
        internal static SensorEnergyLog Parse(Sensor sensor, Func<long, long> GetSensorDimTimeId, string contentLogItem)
        {
            var contenValues = contentLogItem.Split(";");
            var unixTime = long.Parse(contenValues[0]);
            var watts1 = long.Parse(contenValues[1]);
            var watts2 = long.Parse(contenValues[2]);
            var watts3 = long.Parse(contenValues[3]);
            var wattsTotal = watts1 + watts2 + watts3;
            var sensorEnergyLog = new SensorEnergyLog()
            {
                SensorId = sensor.Id,
                SensorDimTimeId = GetSensorDimTimeId(unixTime),
                UnixTime = unixTime,
                Duration = sensor.LogDurationMode,
                Watts1 = watts1 * sensor.DefaultToConvert,
                Watts2 = watts2 * sensor.DefaultToConvert,
                Watts3 = watts3 * sensor.DefaultToConvert,
                WattsTotal = wattsTotal * sensor.DefaultToConvert,
                ConvertToUnits = sensor.DefaultToConvert
            };
            return sensorEnergyLog;
        }

        internal void Update(SensorEnergyLog newEnergyLog)
        {
            this.Watts1 = newEnergyLog.Watts1;
            this.Watts2 = newEnergyLog.Watts2;
            this.Watts3 = newEnergyLog.Watts3;
            this.WattsTotal = newEnergyLog.WattsTotal;
        }
    }
}
