using System;

namespace WampClient
{
    public class HeartbeatDto
    {
        public string Type { get; set; }
        public string Instance { get; set; }
        public DateTime Timestamp { get; set; }
        public double Load { get; set; }
    }
}