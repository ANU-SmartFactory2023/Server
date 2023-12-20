namespace Server.Models
{
    public class ProcessBaseModel
    {
        public int? idx { get; set; }
        public string lot_id { get; set; }
        public int serial { get; set; }
        public DateTime start_time { get; set; }
        public DateTime? end_time { get; set; }
        public string? spend_time { get; set; }
        public double? value { get; set; }
		public string? grade { get; set; }

		public ProcessBaseModel()
        {
            lot_id = "0";
            serial = 0;
            start_time = DateTime.Now;
            end_time = DateTime.Now;
            spend_time = "0";
            value = 0;
            grade = "0";
        }
    }
}
