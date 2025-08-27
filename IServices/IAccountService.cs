using PlantDiaganoseDisease.Models;
using PlantDiaganoseDisease.Models.RequestModels;

namespace PlantDiaganoseDisease.IServices
{
    public interface IAccountService
    {
        public Task<ResponseModel> AuthenticateUser(LoginReqModel rq);
    }
}
