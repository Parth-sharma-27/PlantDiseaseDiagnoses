using PlantDiaganoseDisease.Models;
using PlantDiaganoseDisease.Models.RequestModels;

namespace PlantDiaganoseDisease.IServices
{
    public interface IUserManagerService
    {
        public Task<ResponseModel> GetUserById(int userId);

        public Task<ResponseModel> GetAllUsers();
        public Task<ResponseModel> CreateOrSetUser(UserMasterReqModel rq);
    }
}
