using System.ComponentModel.DataAnnotations;

namespace Server.Models
{
    public class SemiconductorModel
    {
        public String lot_id { get; set; }
        public int sensor_1 { get; set; }
        public int sensor_2 { get; set; }
        public int sensor_3 { get; set; }
        public int sensor_4 { get; set; }
        public int grade { get; set; }

    }
}
