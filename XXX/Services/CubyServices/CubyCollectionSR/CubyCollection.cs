using Models.JsonTpansportModel;
using Models.ModelVM;
using Services.DateTimeService;

namespace Services.CubyServices.CubyCollectionSR
{
    public class CubyCollection : ICubyCollection
    {                     
        private Random random = new Random();         //счётчик для цикла заполнение контейнера пользователей   
        public DataForCuby DataForXXX { get; set; }  //Данные с сервера
        public List<MainDataJson> Re_cuttingContainer { get; set; } = new List<MainDataJson>(50); //Контейнер перезарядки пользователей
        public List<List<MainDataJson>> UsersContainers { get; set; } = new List<List<MainDataJson>>(2) { new List<MainDataJson>(50), new List<MainDataJson>(50)};
                
        private byte numberContainerForRe_cutting = 0;//Номер контейнера для перезарядки
        //Number container for select user
        public byte NumberContainerForSelectUser { get; set; } = 0;
        //Number for select user
        public byte NumberSelectUser { get; set; } = 49;
        public byte NumberPhoto { get; set; } = 0;

        private readonly BirthdayValidationService birthdayValidation;
        public CubyCollection(DataForCuby DataForXXX, BirthdayValidationService birthdayValidation)
        {
            this.DataForXXX = DataForXXX;
            this.birthdayValidation = birthdayValidation;                  
        }
        
        //Method: Выбо формат заполнение контейнеров
        public async Task OnInitializedContainerAsync()
        {
            await Task.Run(() =>
            {
                //размер контейнера перезарядки
                short userCounter = 50;   
                
                if (DataForXXX.ApponentDataJson == null || (DataForXXX.ApponentDataJson.Man == true && DataForXXX.ApponentDataJson.Woman == true) 
                || (DataForXXX.ApponentDataJson.Man == false && DataForXXX.ApponentDataJson.Woman == false))
                {
                    GetManContainer((short)(userCounter / 2), (short)(userCounter / 2));
                    GetWomanContainer((short)(userCounter / 2), (short)(userCounter / 2));                   
                }
                else if (DataForXXX.ApponentDataJson.Man == true && DataForXXX.ApponentDataJson.Woman == false) GetManContainer(userCounter, userCounter);               
                else if (DataForXXX.ApponentDataJson.Man == false && DataForXXX.ApponentDataJson.Woman == true) GetWomanContainer(userCounter, userCounter);
               
                //Очищаю контейнер перезарядки
                UsersContainers[numberContainerForRe_cutting] = new List<MainDataJson>(Re_cuttingContainer);
                Re_cuttingContainer.Clear();
               
                //Счётчик для двух контейнеров
                if (numberContainerForRe_cutting == 0) numberContainerForRe_cutting++;
                else numberContainerForRe_cutting--;
            });
        }
        
        //Method: Получаю  контейнер Man
        private void GetManContainer(short userCounter, short counterСycle)
        {           
            byte i = 0; //общий счётчик  50           
            while (i < counterСycle) //Заполняю пол контейнера Man пользователями
            {
                //Переменная для пропуска определённого количества пользователей в базе
                int count = random.Next(DataForXXX.ManListJson.Count());
                //Выбираю пользователей рандомно из коллекции
                List<MainDataJson> users = DataForXXX.ManListJson.Skip(count).Take(userCounter).ToList();

                if (users.Count() > 0)
                {
                    foreach (var user in users)
                    {
                        user.Age = (short)birthdayValidation.GetAge(user.DateOfBirth);                       
                        Re_cuttingContainer.Add(user);
                                                  
                        i++;
                    }
                }
                //Разность между найденными пользователями и общим счётчиком
                userCounter -= (short)users.Count();
            }
        }

        //Method: Получаю  контейнер Woman
        private void GetWomanContainer(short userCounter, short counterСycle)
        {
            byte i = 0;

            while (i < counterСycle)
            {
                int count = random.Next(DataForXXX.WomanListJson.Count());
                List<MainDataJson> users = DataForXXX.WomanListJson.Skip(count).Take(userCounter).ToList();
                
                if (users.Count() > 0)
                {
                    foreach (var user in users)
                    {
                        user.Age = (short)birthdayValidation.GetAge(user.DateOfBirth);                       
                        Re_cuttingContainer.Add(user);
                    
                        i++;
                    }
                }
                userCounter -= (short)users.Count();
            }
        }

        //Method: Get Next user
        public void GetNextUser()
        {
            //first number img
            NumberPhoto = 0;
            //Get next container
            if (NumberSelectUser == UsersContainers[NumberContainerForSelectUser].Count() -1)
            {
                OnInitializedContainerAsync();
                //Reset counter for getting users from container
                NumberSelectUser = 0;
                NumberContainerForSelectUser = (byte)(NumberContainerForSelectUser == 0 ? 1 : 0);

                return;
            }

            //Increment  NumberSelectUser
            ++NumberSelectUser;
        }                  
    }
}
