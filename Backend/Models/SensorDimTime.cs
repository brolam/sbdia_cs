using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public enum PeriodOfDayTypes
    {
        EarlyMorning = 0,
        Morning = 1,
        Noon = 2,
        Eve = 3,
        Night = 4,
        LateNight = 5,
    }
    public class SensorDimTime
    {
        private DateTime dateTime;

        public SensorDimTime()
        {
        }
        public SensorDimTime(long unixTimeSeconds, Sensor sensor)
        {
            this.Sensor = sensor;
            DateTime dtUnixTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dtUnixTime = dtUnixTime.AddSeconds(unixTimeSeconds);
            TimeZoneInfo sensorTimeZone = TimeZoneInfo.FindSystemTimeZoneById(Sensor.TimeZone);
            this.DateTime = TimeZoneInfo.ConvertTimeFromUtc(dtUnixTime, sensorTimeZone);
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        public Sensor Sensor { get; set; }
        [Required]
        public DateTime DateTime { get => dateTime; set => SetDateTime(value); }
        [Required, Range(1900, int.MaxValue)]
        public int Year { get; private set; }
        [Required, Range(1, 12)]
        public int Month { get; private set; }
        [Required, Range(1, 31)]
        public int Day { get; private set; }
        [Required, Range(0, 23)]
        public int Hour { get; private set; }
        [Required, Range(0, 6)]
        public DayOfWeek DayOfWeek { get; private set; }
        [Required, Range(0, 6)]
        public PeriodOfDayTypes PeriodOfDay { get; private set; }
        private void SetDateTime(DateTime dateTime)
        {
            this.dateTime = dateTime;
            this.Year = DateTime.Year;
            this.Month = DateTime.Month;
            this.Day = DateTime.Day;
            this.Hour = DateTime.Hour;
            this.DayOfWeek = DateTime.DayOfWeek;
            this.PeriodOfDay = GetPeriodOfDayFromHour(this.Hour);
        }

        public static PeriodOfDayTypes GetPeriodOfDayFromHour(int hour)
        {
            if ((hour > 4) & (hour <= 8))
                return PeriodOfDayTypes.EarlyMorning; // 'Early Morning'
            else if ((hour > 8) & (hour <= 12))
                return PeriodOfDayTypes.Morning; //'Morning'
            else if ((hour > 12) & (hour <= 16))
                return PeriodOfDayTypes.Noon; //'Noon'
            else if ((hour > 16) & (hour <= 20))
                return PeriodOfDayTypes.Eve; //'Eve'
            else if ((hour > 20) & (hour <= 24))
                return PeriodOfDayTypes.Night;  //'Night'
            else //if (hour <= 4)
                return PeriodOfDayTypes.LateNight; //'Late Night' 
        }
    }
}
