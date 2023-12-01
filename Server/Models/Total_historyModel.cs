namespace Server.Models
{
    public class Total_historyModel
    {
        public int? idx { get; set; }
        public string lot_id { get; set; }
        public int serial { get; set; }
        public DateTime? start_time { get; set; }
        public DateTime? end_time { get; set;}
        public DateTime? spend_time { get; set; }
        public int? process1_idx { get; set; }
        public int? process2_idx { get; set;}
        public int? process3_idx { get; set;}
        public int? process4_idx { get; set;}
        public string? grade { get; set; }
    }
}
