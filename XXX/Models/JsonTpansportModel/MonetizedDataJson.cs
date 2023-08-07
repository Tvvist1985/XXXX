using Models.DataModel.MonetizedDataModel;

namespace Models.JsonTpansportModel
{
    public class MonetizedDataJson : MonetizedDataDTO
    {
        //Method:Инициализация манетизацию данного пользователя
        public async Task InicializedLikesAsync(MonetizedDataDTO mData) => await Task.Run(() => Likes = mData.Likes);
    }
}
