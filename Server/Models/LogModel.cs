using System.ComponentModel.DataAnnotations;

namespace Server.Models
{
    public class LogModel
    {
        public int log_No { get; set; }
        public String lot_id { get; set; }
        public int defective { get; set; }
        public int process_start { get; set; }
        public int process_end { get; set; }
        public int process_time { get; set; }
        public int process1_time { get; set; }
        public int process2_time { get; set; }
        public int process3_time { get; set; }
        public int process4_time { get; set; }

    }
}
