using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlantDiaganoseDisease.IServices;
using PlantDiaganoseDisease.Models.RequestModels;

namespace PlantDiaganoseDisease.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagerController : ControllerBase
    {
        private readonly IUserManagerService _userManagerService;
        private readonly IAccountService _accountService;
        public UserManagerController(IUserManagerService userManagerService, IAccountService accountService)
        {
            _userManagerService = userManagerService;
            _accountService = accountService;
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var res = await _userManagerService.GetAllUsers();
            return res == null ? NotFound() : Ok(res);
        }

        [HttpGet("GetUserById")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            var res = await _userManagerService.GetUserById(userId);
            return res == null ? NotFound() : Ok(res);
        }
        [HttpPost("AuthenticateUser")]
        public async Task<IActionResult> AuthenticateUser(LoginReqModel user)
        {
            var res = await _accountService.AuthenticateUser(user);
            return res == null ? Ok(res) : Ok(res);
        }
        [HttpPost("CreateOrSetUser")]
        public async Task<IActionResult> CreateOrSetUser([FromBody] UserMasterReqModel user)
        {
            var res = await _userManagerService.CreateOrSetUser(user);
            return res == null ? NotFound() : Ok(res);
        }
    }
}
