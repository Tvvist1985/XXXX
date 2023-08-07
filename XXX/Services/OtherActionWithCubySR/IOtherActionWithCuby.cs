namespace Services.OtherActionWithCubySR
{
    public interface IOtherActionWithCuby
    {     
        public string LeftArrowView { get; set; } 
        public string RightArrowView { get; set; }

        public void LoadingImgArrowAsync(byte quantityImgs);
        public void ViewingPhotosRight(byte numberPhoto, byte quantityImgs);
        public void ViewingPhotosLeft(byte numberPhoto,byte quantityImgs);       
    }
}
