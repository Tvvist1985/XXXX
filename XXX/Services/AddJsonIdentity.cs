using System.Text.Json;

namespace Services
{
    public struct AddJsonIdentity
    {
        public AddJsonIdentity() { }
        
        private readonly string path = @"\identity.json";   //Фаил храница на диске C: а не в папке wwwroot

        //Method: содание файлика JSON для аунтефикации
        public void SerializeJsonFile<Entity>(Entity model)
        {
            //Удаляю JSON  иначе при замене косячит
            if (File.Exists(path)) File.Delete(path);

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            { JsonSerializer.Serialize(fs, model); }
        }

        //Method: получаю данные из JSON для аунтефикации
        public Entity DeserializeJsonFileAsync<Entity>()
        {         
            string jsonText = default;
            if (File.Exists(path))
                using (StreamReader reader = new StreamReader(path))
                    //также можно получить через бинарный формат
                    jsonText = reader.ReadToEnd();

            return string.IsNullOrWhiteSpace(jsonText) ? default : JsonSerializer.Deserialize<Entity>(jsonText);
        }

        //Method: Delete identity file.
        public void DeleteFileIndentiy() => File.Delete(path);
    }
}
