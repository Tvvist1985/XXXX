using Models.ModelVM;

namespace ApiServices.WorkingWithImage
{
    public struct WorkingWithIMG
    {
        //Метод: Создание директории
        public static async Task<string> GetDirectoryAsync(params string[] directoryName)
        {
            string path = default;
            await Task.Run(() =>
            {
                path = Directory.GetCurrentDirectory();
                //создаю путь
                foreach (var item in directoryName) path = Path.Combine(path, item);               
            });
            return path;
        }
        //Метод: Создание директории в коорневой папке
        public static async Task<string> CreateGetDirectoryAsync(params string[] directoryName)
        {
            string path = default;
            await Task.Run(() =>
            {
                path = Directory.GetCurrentDirectory();
                //создаю путь
                foreach (var item in directoryName) path = Path.Combine(path, item);
                //создание директории(папок) если их нет
                if (!Directory.Exists(path) && !string.IsNullOrEmpty(path)) Directory.CreateDirectory(path);
            });
            return path;
        }

        //Метод сохраняет фаил из СТРОКИ 64  в указонную директорию
        public static async Task CreateFileFromBase64StringAsync(string directory, string fileName, string fileBase64)
        {
            await Task.Run(async () =>
            {
                //Создание пути файла
                directory = Path.Combine(directory, fileName);
                //Сохраняем изображени          
                using (var fs = new FileStream(directory, FileMode.Create))
                {
                    //Декодировка из строки BASE64
                    var enTextBytes = Convert.FromBase64String(fileBase64);
                    
                    //запись массива байтов в файл               
                    await fs.WriteAsync(enTextBytes, 0, enTextBytes.Length);
                }
            });
        }

        //Method: Получение из файла  СТроку в ФОРМАТЕ BASE64;      
        public static async Task<string> GetStringBase64FromFileAsync(string directory, string fileName)
        {
            string stringBase64 = default;
            await Task.Run(async () =>
            {
                var fullFileName = Path.Combine(directory, fileName);

                using (FileStream fs = File.OpenRead(fullFileName))
                {
                    byte[] buffer = new byte[fs.Length];
                    await fs.ReadAsync(buffer, 0, buffer.Length);

                    //конвертирую байты в строку для вывода на html
                    stringBase64 = Convert.ToBase64String(buffer);
                }
            });
            return stringBase64;
        }

        //Method: Получение из всех файлов  СТроку в ФОРМАТЕ BASE64;      
        public static async Task<string[]> GetAllBase64FromFileAsync(string directory)
        {     
            string[] stringBase64 = new string[6];

            await Task.Run(() =>
            {                
                List<Task> tasks = new List<Task>(6);//создаю Список Задач
               
                var fileNames = GetFileNameAsync(directory).Result; //получаю имена файлов

                byte j = 0;// счётчик для конкретного еста в массиве
                foreach (var name in fileNames) //заполняю массив в виде base64  
                    tasks.Add(Task.Run(() => AddIMG(j++, stringBase64, directory, name)));

                 Task.WhenAll(tasks).Wait();
            });

            void AddIMG(byte count, string[] stringBase64, string sirectory, string name) =>            
                stringBase64[count] = GetStringBase64FromFileAsync(directory, name).Result;
                                  
            return stringBase64.Where(p => p != null).ToArray();
        }

        //Method: Получение ВСЕ чистые имена файлов ОНИс РАСШИРЕНИЕМ
        public static async Task<string[]> GetFileNameAsync(string directory) =>
           await Task.Run(() => Directory.EnumerateFiles(directory).Select(fn => Path.GetFileName(fn)).ToArray());

        //Method: Удаляю папку 
        public static async Task DeleteDirectoryAsync(string directory)
        {
            await Task.Run(() =>
            {                
                //Проверка обязательна   иначе эксепшн
                if (Directory.Exists(directory))
                    Directory.Delete(directory, true);
            });
        }

        //Method: Удаляю фаил
        public static async Task DeleteFileAsync(string directory)
        {
            await Task.Run(() =>
            {
                //Проверка обязательна   иначе эксепшн
                if (File.Exists(directory))
                    File.Delete(directory);
            });
        }
    }
}
