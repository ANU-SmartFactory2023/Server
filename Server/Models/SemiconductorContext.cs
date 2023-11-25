using Microsoft.EntityFrameworkCore;

namespace Server.Models
{
    public class SemiconductorContext : DbContext
    {
        //생성자, DB 연결 초기화
        public SemiconductorContext(DbContextOptions options) : base(options)
        {

        }
        //DB연결작업 --> appsettings.json
        //테이블 만들기
        public DbSet<SemiconductorModel> SemiconductorModel { get; set; }
        public DbSet<LogModel> LogModels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LogModel>().HasKey(x => x.log_No);  // 기본키 정의
            modelBuilder.Entity<SemiconductorModel>().HasKey(x => x.lot_id);    // 기본키 정의

            modelBuilder.Entity<SemiconductorModel>().ToTable("Semiconductor");
            modelBuilder.Entity<LogModel>().ToTable("Log");


        }
    }
}
