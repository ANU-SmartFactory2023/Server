﻿namespace Server.Models
{
    public class Process4Model
    {
        public int? idx { get; set; }
        public string lot_id { get; set; }
        public int serial { get; set; }
        public DateTime start_time { get; set; }
        public DateTime? end_time { get; set; }
        public DateTime? spend_time { get; set; }
        public double? value { get; set; }
    }
}
