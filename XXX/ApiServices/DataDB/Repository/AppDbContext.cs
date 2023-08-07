using Microsoft.EntityFrameworkCore;
using Models.DataModel.AponentDataModel;
using Models.DataModel.AponentDataModel.AponentForMan;
using Models.DataModel.AponentDataModel.AponentForWoman;
using Models.DataModel.ChatEventModel;
using Models.DataModel.DeleteSympathyModel;
using Models.DataModel.DeleteSympathyModel.Man;
using Models.DataModel.DeleteSympathyModel.Woman;
using Models.DataModel.EventDataUserModel;
using Models.DataModel.EventDataUserModel.Man;
using Models.DataModel.EventDataUserModel.Woman;
using Models.DataModel.MainDataUserModel;
using Models.DataModel.MainDataUserModel.Man;
using Models.DataModel.MainDataUserModel.Woman;
using Models.DataModel.MonetizedDataModel;
using Models.DataModel.MonetizedDataModel.Man;
using Models.DataModel.MonetizedDataModel.Woman;
using Models.DataModels;


namespace ApiServices.DataDB.Repository
{
    public class AppDbContext : DbContext
    {
        public DbSet<UsersMapDTO> UsersMapDTO { get; set; } = null!;

        public DbSet<MainUserDTO> MainUserDTO { get; set; } = null!;
        public DbSet<UserManDTOTBL1> UserManDTOTBL1 { get; set; } = null!;
        public DbSet<UserWomanDTOTBL1> UserWomanDTOTBL1 { get; set; } = null!;

        public DbSet<EventUserDTO> EventUserDTO { get; set; } = null!;
        public DbSet<EventManDTOTBL1> EventManDTOTBL1 { get; set; } = null!;
        public DbSet<EventWomanDTOTBL1> EventWomanDTOTBL1 { get; set; } = null!;

        public DbSet<AponentDTO> AponentDTO { get; set; } = null!;
        public DbSet<AponentForManDTOTbl1> AponentForManDTOTbl1 { get; set; } = null!;
        public DbSet<AponentForWomanDTOTbl1> AponentForWomanDTOTbl1 { get; set; } = null!;

        public DbSet<MonetizedDataDTO> MonetizedDataDTO { get; set; } = null!;
        public DbSet<MonetizedForManTBL1> MonetizedForManTBL1 { get; set; } = null!;
        public DbSet<MonetizedForWomanTBL1> MonetizedForWomanTBL1 { get; set; } = null!;

        public DbSet<ChatDTO> ChatDTO { get; set; } = null!;

        public DbSet<DeleteSympathyDTO> DeleteSympathyDTO { get; set; } = null!;
        public DbSet<DeleteSympathyForManTBL1> DeleteSympathyForManTBL1 { get; set; } = null!;
        public DbSet<DeleteSympathyForWomanTBL1> DeleteSympathyForWomanTBL1 { get; set; } = null!;
       
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //DONT DELETE!!!!!!!!!!!!
            //Инициализирую базу начальным MAP
            modelBuilder.Entity<UsersMapDTO>().HasData(
                new UsersMapDTO { Id = Guid.NewGuid(), Gender = "Man" },
                new UsersMapDTO { Id = Guid.NewGuid(), Gender = "Woman" }
            );

            //Наследование таблиц стратегия TPC
            modelBuilder.Entity<MainUserDTO>().UseTpcMappingStrategy();
            modelBuilder.Entity<EventUserDTO>().UseTpcMappingStrategy();
            modelBuilder.Entity<AponentDTO>().UseTpcMappingStrategy();
            modelBuilder.Entity<MonetizedDataDTO>().UseTpcMappingStrategy();
            modelBuilder.Entity<DeleteSympathyDTO>().UseTpcMappingStrategy();         
        }
    }
}
