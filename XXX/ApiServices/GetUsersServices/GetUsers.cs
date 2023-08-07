using Microsoft.EntityFrameworkCore;
using Models.DataModel.MainDataUserModel;
using Models.ModelVM;
using Services.DataDB.Repository;

namespace APIService.GetUsersServices
{
    public class GetUsers : IGetUsers
    {
        private readonly IStoreDbRepository db;
        public GetUsers(IStoreDbRepository db) { this.db = db; }

        //Method:Инициализирую контейнеры из базы данных
        public void OnInitializedListsUsers(DataForCuby data, ref List<MainUserDTO> ManList, ref List<MainUserDTO> WomanList)
        {
            if (!string.IsNullOrEmpty(data.JwtToken))
            {
                //Загрузка Man + проверка  есть хоть ктото в базе 
                if (data.ApponentDataJson.Man && !data.ApponentDataJson.Woman)
                {
                    LoadManFromDb(data, ref ManList);
                    if (ManList.Count() < 2) LoadRandomManFromDb(data, ref ManList);
                }
                //Загрузкк Woman
                else if (data.ApponentDataJson.Woman && !data.ApponentDataJson.Man)
                {
                    LoadWomanFromDb(data, ref WomanList);
                    if (WomanList.Count() < 2) LoadRandomWomanFromDb(data, ref WomanList);
                }
                else
                {
                    LoadManFromDb(data, ref ManList);
                    if (ManList.Count() < 2) LoadRandomManFromDb(data, ref ManList);

                    LoadWomanFromDb(data, ref WomanList);
                    if (WomanList.Count() < 2) LoadRandomWomanFromDb(data, ref WomanList);
                }
            }
            else
            {
                LoadRandomManFromDb(data, ref ManList);
                LoadRandomWomanFromDb(data, ref WomanList);
            }
        }

        //Method: Load Man from Db
        private void LoadManFromDb(DataForCuby data, ref List<MainUserDTO> ManList)
        {
            //Получаю минимальную и максимальную дату
            var date = DateInitialized(data);
            //Выбираю Man
            ManList = db.DirectRequestToDatabase(p => p.UsersMapDTO.AsNoTracking().Include(p => p.MainUserDTO.Where(p => p.City == data.ApponentDataJson.City && p.DateOfBirth >= date[1] && p.DateOfBirth <= date[0] && p.Id != data.ApponentDataJson.Id))
            .ThenInclude(p => p.ChatDTO.Where(p => p.MainUserDTO
           .Any(p => p.Id == data.ApponentDataJson.Id))).ThenInclude(p => p.ChatMessages)
           .Include(p => p.MainUserDTO)
           .ThenInclude(p => p.EventUserDTO.Where(p => p.ApponentID == data.ApponentDataJson.Id))
           .Include(p => p.MainUserDTO)
           .ThenInclude(p => p.AponentDTO)           
           .FirstOrDefault(p => p.Gender == "Man").MainUserDTO);
        }

        //Method: Load WOman from Db
        private void LoadWomanFromDb(DataForCuby data, ref List<MainUserDTO> WomanList)
        {
            var date = DateInitialized(data);
            //Выбираю Woman
            WomanList = db.DirectRequestToDatabase(p => p.UsersMapDTO.AsNoTracking().Include(p => p.MainUserDTO.Where(p => p.City == data.ApponentDataJson.City && p.DateOfBirth >= date[1] && p.DateOfBirth <= date[0] && p.Id != data.ApponentDataJson.Id))
            .ThenInclude(p => p.ChatDTO.Where(p => p.MainUserDTO
           .Any(p => p.Id == data.ApponentDataJson.Id))).ThenInclude(p => p.ChatMessages)
           .Include(p => p.MainUserDTO)
           .ThenInclude(p => p.EventUserDTO.Where(p => p.ApponentID == data.ApponentDataJson.Id))
           .Include(p => p.MainUserDTO)
           .ThenInclude(p => p.AponentDTO)           
           .FirstOrDefault(p => p.Gender == "Woman").MainUserDTO);
        }

        //Method: Load Random from Db
        private void LoadRandomManFromDb(DataForCuby data, ref List<MainUserDTO> ManList)
        {
            if (string.IsNullOrEmpty(data.JwtToken))
                ManList = db.DirectRequestToDatabase(p => p.UsersMapDTO.AsNoTracking().Include(p => p.MainUserDTO)                
                .FirstOrDefault(p => p.Gender == "Man").MainUserDTO);
            else
                ManList = db.DirectRequestToDatabase(p => p.UsersMapDTO.AsNoTracking().Include(p => p.MainUserDTO.Where(p => p.Id != data.ApponentDataJson.Id))
                .ThenInclude(p => p.ChatDTO.Where(p => p.MainUserDTO
               .Any(p => p.Id == data.ApponentDataJson.Id))).ThenInclude(p => p.ChatMessages)
               .Include(p => p.MainUserDTO)
               .ThenInclude(p => p.EventUserDTO.Where(p => p.ApponentID == data.ApponentDataJson.Id))
               .Include(p => p.MainUserDTO)
               .ThenInclude(p => p.AponentDTO)              
               .FirstOrDefault(p => p.Gender == "Man").MainUserDTO);
        }
        //Method: Load Random from Db
        private void LoadRandomWomanFromDb(DataForCuby data, ref List<MainUserDTO> WomanList)
        {
            if (string.IsNullOrEmpty(data.JwtToken))
                WomanList = db.DirectRequestToDatabase(p => p.UsersMapDTO.AsNoTracking().Include(p => p.MainUserDTO)
                .FirstOrDefault(p => p.Gender == "Woman").MainUserDTO);
            else
                WomanList = db.DirectRequestToDatabase(p => p.UsersMapDTO.AsNoTracking().Include(p => p.MainUserDTO.Where(p => p.Id != data.ApponentDataJson.Id))
               .ThenInclude(p => p.ChatDTO.Where(p => p.MainUserDTO
               .Any(p => p.Id == data.ApponentDataJson.Id))).ThenInclude(p => p.ChatMessages)
               .Include(p => p.MainUserDTO)
               .ThenInclude(p => p.EventUserDTO.Where(p => p.ApponentID == data.ApponentDataJson.Id))
               .Include(p => p.MainUserDTO)
              .ThenInclude(p => p.AponentDTO)              
              .FirstOrDefault(p => p.Gender == "Woman").MainUserDTO);
        }

        //Method: Инициализирует даты рождения для поисковика
        private DateTime[] DateInitialized(DataForCuby data)
        {
            DateTime[] date = new DateTime[2];
            //DateMax
            date[0] = DateTime.Now.AddYears((short)-data.ApponentDataJson.InitialAge);
            //DateMin
            date[1] = DateTime.Now.AddYears((short)-data.ApponentDataJson.FinalAge);

            return date;
        }
    }
}
