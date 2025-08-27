using PlantDiaganoseDisease.Helper;
using PlantDiaganoseDisease.IServices;
using PlantDiaganoseDisease.Models;
using PlantDiaganoseDisease.Models.RequestModels;
using System.Data;
using System.Data.SqlClient;

namespace PlantDiaganoseDisease.Services
{
    public class AccountService : IAccountService
    {
        private readonly IConfiguration _configuration;
        private readonly IJwtAuthService _jwtAuthService;
        public AccountService(IConfiguration configuration, IJwtAuthService jwtAuthService)
        {
            _configuration = configuration;
            _jwtAuthService = jwtAuthService;
        }

        public async Task<ResponseModel> AuthenticateUser(LoginReqModel rq)
        {
            ResponseModel response = new ResponseModel();
            EncDcService encDcService = new EncDcService();

            try
            {
                rq.Password = await encDcService.Encrypt(rq.Password);

                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("ConnStr")))
                {
                    await conn.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_GetAuthenticatedUser", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@MobileOrEmailId", rq.MobileOrEmail ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Password", rq.Password ?? (object)DBNull.Value);

                        SqlParameter retParam = new SqlParameter("@Ret", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(retParam);

                        SqlParameter errParam = new SqlParameter("@ErrorMsg", SqlDbType.VarChar, 500)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(errParam);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                int userId = Convert.ToInt32(reader["Id"]);
                                string role = reader["RoleName"].ToString();
                                string mobileOrEmail = rq.MobileOrEmail;

                                if (userId != 0)
                                {
                                    string jwtToken = await _jwtAuthService.GenerateJwtToken(userId, mobileOrEmail, role);

                                    response.code = userId;
                                    response.data = jwtToken;
                                    response.msg = "Success";
                                }
                                else
                                {
                                    response.code = -1;
                                    response.msg = "Username or Password Incorrect.";
                                }
                            }
                            else
                            {
                                response.code = -2;
                                response.msg = "Invalid User";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.code = -3;
                response.msg = ex.Message;
            }

            return response;
        }

    }
}
