using System;

namespace Backend.Models.Dtos
{
    public class SensorDto
    {
        public string Id { get; set; }
        public float LogDurationMode { get; set; }
        public string SecretApiToken { get; set; }
    }
}
