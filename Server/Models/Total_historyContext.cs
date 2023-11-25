using Microsoft.EntityFrameworkCore;

namespace Server.Models
{
    public class Total_historyContext : DbContext
    {

        //생성자, DB 연결 초기화
        public Total_historyContext(DbContextOptions options) : base(options)
        {

        }
        //DB연결작업 --> appsettings.json
        //테이블 만들기
        public DbSet<Total_historyModel> Total_historyModel { get; set; }
        public DbSet<Process1Model> Process1Model { get; set; }
        public DbSet<Process2Model> Process2Model { get; set; }
        public DbSet<Process3Model> Process3Model { get; set; }
        public DbSet<Process4Model> Process4Model { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Total_historyModel>().HasKey(x => x.idx);  // 기본키 정의
            modelBuilder.Entity<Process1Model>().HasKey(x => x.idx);       // 기본키 정의
            modelBuilder.Entity<Process2Model>().HasKey(x => x.idx);       // 기본키 정의
            modelBuilder.Entity<Process3Model>().HasKey(x => x.idx);       // 기본키 정의
            modelBuilder.Entity<Process4Model>().HasKey(x => x.idx);       // 기본키 정의


            modelBuilder.Entity<Total_historyModel>().ToTable("Total_history");
            modelBuilder.Entity<Process1Model>().ToTable("Process1");
            modelBuilder.Entity<Process2Model>().ToTable("Process2");
            modelBuilder.Entity<Process3Model>().ToTable("Process3");
            modelBuilder.Entity<Process4Model>().ToTable("Process4");
        }
    }
}
