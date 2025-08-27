using PlantDiaganoseDisease.Helper;
using PlantDiaganoseDisease.IServices;
using PlantDiaganoseDisease.Models;
using PlantDiaganoseDisease.Models.RequestModels;
using System.Data;
using System.Data.SqlClient;

namespace PlantDiaganoseDisease.Services
{
    public class UserManagerService : IUserManagerService
    {
        private readonly IConfiguration _configuration;
        private readonly IAccountService _accountService;
        EncDcService _encDcService = new EncDcService();
        public UserManagerService(IAccountService accountService, IConfiguration configuration)
        {
            _accountService = accountService;
            _configuration = configuration;
        }

        public async Task<ResponseModel> GetAllUsers()
        {
            ResponseModel response = new ResponseModel();

            var users = new List<UserMasterReqModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("ConnStr")))
                using (SqlCommand cmd = new SqlCommand("sp_ManageUsers", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Input parameters
                    cmd.Parameters.AddWithValue("@Flag", "G");

                    // Output parameters
                    SqlParameter errorMsgParam = new SqlParameter("@ErrorMsg", SqlDbType.VarChar, 90)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(errorMsgParam);

                    SqlParameter retParam = new SqlParameter("@Ret", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(retParam);

                    await conn.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var user = new UserMasterReqModel
                            {
                                UserId = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                                UserName = reader["Username"]?.ToString(),
                                Email = reader["Email"]?.ToString(),
                                Password = reader["Password"]?.ToString(),
                                FirstName = reader["FirstName"]?.ToString(),
                                LastName = reader["LastName"]?.ToString(),
                                Mobile = reader["Mobile"].ToString(),
                                Address = reader["Address"]?.ToString(),
                                IsActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"]),
                                RoleId = reader["RoleId"] != DBNull.Value ? Convert.ToInt32(reader["RoleId"]) : 0,
                                CreatedBy = reader["CreatedBy"] != DBNull.Value ? Convert.ToInt32(reader["CreatedBy"]) : 0,

                            };

                            users.Add(user);
                        }
                    }

                    response.msg = errorMsgParam.Value?.ToString();
                    response.code = (int)(retParam.Value ?? -1);
                    response.data = users;
                }
            }
            catch (Exception ex)
            {
                response.msg = ex.Message;
                response.code = -1;
                response.data = null;
            }

            return response;
        }

        public async Task<ResponseModel> GetUserById(int userId)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("ConnStr")))
                using (SqlCommand cmd = new SqlCommand("sp_ManageUsers", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Input parameters
                    cmd.Parameters.AddWithValue("@Flag", "G"); // For getting a specific user
                    cmd.Parameters.AddWithValue("@Id", userId);

                    // Output parameters
                    SqlParameter errorMsgParam = new SqlParameter("@ErrorMsg", SqlDbType.VarChar, 90)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(errorMsgParam);

                    SqlParameter retParam = new SqlParameter("@Ret", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(retParam);

                    await conn.OpenAsync();

                    UserMasterReqModel user = null;

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            user = new UserMasterReqModel
                            {
                                UserId = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                                UserName = reader["Username"]?.ToString(),
                                Email = reader["Email"]?.ToString(),
                                Password = reader["Password"]?.ToString(),
                                FirstName = reader["FirstName"]?.ToString(),
                                LastName = reader["LastName"]?.ToString(),
                                Mobile = reader["Mobile"].ToString(),
                                Address = reader["Address"]?.ToString(),
                                IsActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"]),
                                RoleId = reader["RoleId"] != DBNull.Value ? Convert.ToInt32(reader["RoleId"]) : 0,
                                CreatedBy = reader["CreatedBy"] != DBNull.Value ? Convert.ToInt32(reader["CreatedBy"]) : 0,
                            };
                        }

                      
                    }

                    response.msg = errorMsgParam.Value?.ToString();
                    response.code = (int)(retParam.Value ?? -1);
                    response.data = user;
                }
            }
            catch (Exception ex)
            {
                response.msg = ex.Message;
                response.code = -1;
                response.data = null;
            }

            return response;
        }


        public async Task<ResponseModel> CreateOrSetUser(UserMasterReqModel rq)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("ConnStr")))
                using (SqlCommand cmd = new SqlCommand("sp_ManageUsers", conn))
                {
                    string encPassword = await _encDcService.Encrypt(rq.Password);

                    cmd.CommandType = CommandType.StoredProcedure;

                    // Required input parameters
                    cmd.Parameters.AddWithValue("@Flag", rq.Flag); // 'I' for Insert (or based on your logic)
                    cmd.Parameters.AddWithValue("@Id", rq.UserId);
                    cmd.Parameters.AddWithValue("@Username", rq.UserName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", rq.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Password", encPassword ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FirstName", rq.FirstName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastName", rq.LastName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Mobile", rq.Mobile);
                    cmd.Parameters.AddWithValue("@Address", rq.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsActive", rq.IsActive);
                    cmd.Parameters.AddWithValue("@RoleId", rq.RoleId);
                    cmd.Parameters.AddWithValue("@CreatedBy", rq.CreatedBy);


                    // Output parameters
                    SqlParameter errorMsgParam = new SqlParameter("@ErrorMsg", SqlDbType.NVarChar, 20)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(errorMsgParam);

                    SqlParameter retParam = new SqlParameter("@Ret", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(retParam);

                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();

                    // Read output values
                    response.msg = errorMsgParam.Value?.ToString();
                    response.code = (int)(retParam.Value ?? -1);

                }
            }
            catch (Exception ex)
            {
                response.msg = ex.Message;
                response.code = -1;
            }

            return response;
        }

    }
}
