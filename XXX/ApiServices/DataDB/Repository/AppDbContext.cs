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
            //Наследование таблиц стратегия TPC
            modelBuilder.Entity<MainUserDTO>().UseTpcMappingStrategy();
            modelBuilder.Entity<EventUserDTO>().UseTpcMappingStrategy();
            modelBuilder.Entity<AponentDTO>().UseTpcMappingStrategy();
            modelBuilder.Entity<MonetizedDataDTO>().UseTpcMappingStrategy();
            modelBuilder.Entity<DeleteSympathyDTO>().UseTpcMappingStrategy();

            //Add initial Map
            Guid manId = Guid.NewGuid();
            Guid womanId = Guid.NewGuid();
            modelBuilder.Entity<UsersMapDTO>().HasData(
                new UsersMapDTO { Id = manId, Gender = "Man" },
                new UsersMapDTO { Id = womanId, Gender = "Woman" });

            //Add initial data
            AddInitialMan(modelBuilder, manId, 1);
            AddInitialMan(modelBuilder, manId, 2);

            AddInitialWoman(modelBuilder, womanId, 3);
            AddInitialWoman(modelBuilder, womanId, 4);
        }

        //Method: Add initial data
        private void AddInitialMan(ModelBuilder modelBuilder, in Guid mapId, byte endId)
        {


            //Abb users            
            modelBuilder.Entity<UserManDTOTBL1>().HasData(
               new UserManDTOTBL1
               {
                   Id = new Guid($"00000000-0000-0000-0000-00000000000" + endId),
                   UsersMapDTOId = mapId,
                   FirstName = $"Demo"+ endId,
                   DateOfBirth = new(2000, 1, 1),
                   Gender = "Man",
                   Сountry = "Russia",
                   City = "Moscow",
                   EmailAdress = $"11{endId}@111.com",
                   Telephone = 123,
                   Password = "11111",
                   ConfirmPassword = "11111"
               });

            //add apponent data
            modelBuilder.Entity<AponentForManDTOTbl1>().HasData(
               new AponentForManDTOTbl1
               {
                   Id = new Guid($"00000000-0000-0000-0000-00000000000" + endId),
                   MyGender = "Man",
                   Сountry = "Russia",
                   City = "Moscow",
                   MainUserDTOId = new Guid($"00000000-0000-0000-0000-00000000000" + endId)
               }
               );

            //Add monitized data
            modelBuilder.Entity<MonetizedForManTBL1>().HasData(
              new MonetizedForManTBL1
              {
                  Id = new Guid("00000000-0000-0000-0000-00000000000" + endId),
                  TimeLastSession = DateTime.Now,
                  MainUserDTOId = new Guid($"00000000-0000-0000-0000-00000000000" + endId)
              });
        }
        //Method: Add initial data
        private void AddInitialWoman(ModelBuilder modelBuilder, in Guid mapId, byte endId)
        {
            //Abb users            
            modelBuilder.Entity<UserWomanDTOTBL1>().HasData(
               new UserWomanDTOTBL1
               {
                   Id = new Guid($"00000000-0000-0000-0000-00000000000" + endId),
                   UsersMapDTOId = mapId,
                   FirstName = $"Demo" + endId,
                   DateOfBirth = new(2000, 1, 1),
                   Gender = "Woman",
                   Сountry = "Russia",
                   City = "Moscow",
                   EmailAdress = $"11{endId}@111.com",
                   Telephone = 123,
                   Password = "11111",
                   ConfirmPassword = "11111"
               });

            //add apponent data
            modelBuilder.Entity<AponentForWomanDTOTbl1>().HasData(
               new AponentForWomanDTOTbl1
               {
                   Id = new Guid($"00000000-0000-0000-0000-00000000000" + endId),
                   MyGender = "Woman",
                   Сountry = "Russia",
                   City = "Moscow",
                   MainUserDTOId = new Guid($"00000000-0000-0000-0000-00000000000" + endId)
               }
               );

            //Add monitized data
            modelBuilder.Entity<MonetizedForWomanTBL1>().HasData(
              new MonetizedForWomanTBL1
              {
                  Id = new Guid("00000000-0000-0000-0000-00000000000" + endId),
                  TimeLastSession = DateTime.Now,
                  MainUserDTOId = new Guid($"00000000-0000-0000-0000-00000000000" + endId)
              });
        }
    }
}
