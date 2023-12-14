namespace Server.Models
{
	public class ReferenceModel
	{
		public int? idx { get; set; }
		public double top1 { get; set; }
		public double top2 { get; set; }
		public double top3 { get; set; }
		public double top4 { get; set; }
		public double mid1 { get; set; }
		public double mid2 { get; set; }
		public double mid3 { get; set; }
		public double mid4 { get; set; }
		public double bottom1 { get; set; }
		public double bottom2 { get; set; }
		public double bottom3 { get; set; }
		public double bottom4 { get; set; }
		public double A_final { get; set; }
		public double B_final { get; set; }
		public double C_final { get; set; }
		public string A_direction { get; set; }
		public string B_direction { get; set; }
		public string C_direction { get; set; }
	}
}
