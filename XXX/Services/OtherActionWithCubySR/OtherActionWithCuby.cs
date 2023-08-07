namespace Services.OtherActionWithCubySR
{
    public class OtherActionWithCuby : IOtherActionWithCuby
    {
        public string LeftArrowView { get; set; } = "";  //Переменные Малых стрелок
        public string RightArrowView { get; set; } = "";  //Переменные Малых стрелок

        //Method: Отрисовываю стрелки для просмотра фото
        public void LoadingImgArrowAsync(byte quantityImgs)
        {
            //отрисовка стрелок  если фото одно
            if (quantityImgs == 1)
            {
                RightArrowView = "border-left: 9vw  solid #1863c0e3;";
                LeftArrowView = "border-right: 9vw solid #1863c0e3;";
            }
            else
            {
                RightArrowView = "border-left: 9vw  solid #27d913e3;";
                LeftArrowView = "border-right: 9vw solid #1863c0e3;";
            }
        }
        //Mthod:Просмотр фотографий в Право + изменения вида стрелок
        public void ViewingPhotosRight(byte numberPhoto, byte quantityImgs)
        {
            if (numberPhoto > 0 && numberPhoto == quantityImgs - 1) //Проверка на смену вида стрелки
            {
                RightArrowView = "border-left: 9vw solid #1863c0e3;";
                LeftArrowView = "border-right: 9vw solid #27d913e3;";
            }
            else if (numberPhoto > 0 && numberPhoto < quantityImgs - 1)
            {
                LeftArrowView = "border-right: 9vw solid #27d913e3;";
                RightArrowView = "border-left: 9vw solid #27d913e3;";
            }
            else if (numberPhoto == 0 && numberPhoto < quantityImgs - 1)
            {
                LeftArrowView = "border-right: 9vw solid #1863c0e3;";
                RightArrowView = "border-left: 9vw solid #27d913e3;";
            }
        }

        //Mthod:Просмотр фотографий в В лево
        public void ViewingPhotosLeft(byte numberPhoto, byte quantityImgs)
        {
            if (numberPhoto > 0 && numberPhoto == quantityImgs - 1)
            {
                RightArrowView = "border-left: 9vw solid #27d913e3;";
                LeftArrowView = "border-right: 9vw solid #1863c0e3;";
            }
            else if (numberPhoto > 0 && numberPhoto < quantityImgs - 1)
            {
                LeftArrowView = "border-right: 9vw solid #27d913e3;";
                RightArrowView = "border-left: 9vw solid #27d913e3;";
            }
            else if (numberPhoto == 0 && numberPhoto < quantityImgs - 1)
            {
                LeftArrowView = "border-right: 9vw solid #1863c0e3;";
                RightArrowView = "border-left: 9vw solid #27d913e3;";
            }
        }
    }
}
