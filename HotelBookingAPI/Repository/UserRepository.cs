using HotelBookingAPI.Connection;
using HotelBookingAPI.DTOs.UserDTOs;
using HotelBookingAPI.Extensions;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics.Contracts;

namespace HotelBookingAPI.Repository
{
    public class UserRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public UserRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public async Task<CreateUserResponseDTO> AddUserAsync(CreateUserDTO user)
        {
            CreateUserResponseDTO createUserResponseDTO = new CreateUserResponseDTO();
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("spAddUser", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@PasswordHash",user.Password);
            command.Parameters.AddWithValue("@CreatedBy", "System");
            var userIdParam = new SqlParameter("@UserID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            var ErrorMessageParam = new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 255)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(userIdParam);
            command.Parameters.Add(ErrorMessageParam);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            var UserID = (int)userIdParam.Value;

            if(UserID != -1)
            {
                createUserResponseDTO.UserID = UserID;
                createUserResponseDTO.Message = "User Created Successfully..";
                createUserResponseDTO.IsCreated = true;
                return createUserResponseDTO;
            }
            var message = ErrorMessageParam.Value?.ToString();
            createUserResponseDTO.IsCreated= false;
            createUserResponseDTO.Message = "An known error occured while creating the user.";
            return createUserResponseDTO;

        }
        public async Task<UserRoleResponseDTO> AssignRoleToUserAsync(UserRoleDTO userRole)
        {
            UserRoleResponseDTO userRoleResponseDTO = new UserRoleResponseDTO();
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("spAssignUserRole", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@UserID", userRole.UserID);
            command.Parameters.AddWithValue("@RoleID", userRole.RoleID);
            var ErrorMessageParam = new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 255)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(ErrorMessageParam);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            var message = ErrorMessageParam.Value?.ToString();
            if(!string.IsNullOrEmpty(message))
            {
                userRoleResponseDTO.Message = message;
                userRoleResponseDTO.IsAssigned = false;
            }
            else
            {
                userRoleResponseDTO.IsAssigned = true;
                userRoleResponseDTO.Message = "User Role Assigned";
            }
            return userRoleResponseDTO;
        }
        public async Task<List<UserResponseDTO>> ListAllUsersAsync(bool? isActive)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("spListAllUsers", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@IsActive", (object)isActive ?? DBNull.Value);

            await connection.OpenAsync();
            var reader = await command.ExecuteReaderAsync();
            var users = new List<UserResponseDTO>();
            while (reader.Read())
            {
                users.Add(new UserResponseDTO()
                {
                    UserID = reader.GetInt32("UserID"),
                    Email = reader.GetString("Email"),
                    IsActive = reader.GetBoolean("IsActive"),
                    RoleID = reader.GetInt32("RoleID"),
                    LastLogin = reader.GetValueByColumn<DateTime?>("LastLogin")
                });
            }
            return users;
        }

        public async Task<UserResponseDTO> GetUserByIdAsync(int userId)
        {
            UserResponseDTO userResponseDTO = new UserResponseDTO();

           using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("spGetUserByID", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@UserID",userId);
            var ErrorMessageParam = new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 255)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(ErrorMessageParam);
            await connection.OpenAsync();
            var reader = await command.ExecuteReaderAsync();
            if(!reader.Read())
            {
                return null;
            }
            userResponseDTO.UserID = reader.GetInt32("UserID");
            userResponseDTO.Email = reader.GetString("Email");
            userResponseDTO.IsActive = reader.GetBoolean("IsActive");
            userResponseDTO.RoleID = reader.GetInt32("RoleID");
            userResponseDTO.LastLogin = reader.GetValueByColumn<DateTime?>("LastLgin");
            
            return userResponseDTO;
        }
        public async Task<UpdateUserResponseDTO> UpdateUserAsync(UpdateUserDTO user)
        {
            UpdateUserResponseDTO updateUserResponseDTO = new UpdateUserResponseDTO()
            {
                UserID = user.UserID
            };

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("spUpdateUserInformation", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@UserID",user.UserID);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@Password", user.Password);
            command.Parameters.AddWithValue("@ModifiedBy", "System");

            var errorMessageParam = new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 255)
            {
                Direction = ParameterDirection.Output
            };

            command.Parameters.Add(errorMessageParam);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
            var message = errorMessageParam.Value?.ToString();

            if(string.IsNullOrEmpty(message))
            {
                updateUserResponseDTO.IsUpdated = true;
                updateUserResponseDTO.Message = "User Information Updated";
            }
            else
            {
                updateUserResponseDTO.IsUpdated = false;
                updateUserResponseDTO.Message = message;
            }
            return updateUserResponseDTO;

        }
        public async Task<DeleteUserResponseDTO> DeleteUserAsync(int userId)
        {
            DeleteUserResponseDTO deleteUserResponseDTO = new DeleteUserResponseDTO();
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("spToggleUserActive", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@UserID", userId);
            command.Parameters.AddWithValue("@IsActive", false);
            var errorMessageParam = new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 255)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(errorMessageParam);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
            var message = errorMessageParam.Value?.ToString();
            if (!string.IsNullOrEmpty(message))
            {
                deleteUserResponseDTO.Message = message;
                deleteUserResponseDTO.IsDeleted = false;
            }
            else
            {
                deleteUserResponseDTO.Message = "User Deleted.";
                deleteUserResponseDTO.IsDeleted = true;
            }
            return deleteUserResponseDTO;
        }
        public async Task<LoginUserResponseDTO> LoginUserAsync(LoginUserDTO loginUser)
        {
            LoginUserResponseDTO loginUserResponseDTO = new LoginUserResponseDTO();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("spLoginUser", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Email", loginUser.Email);
            command.Parameters.AddWithValue("@PasswordHash",loginUser.Password);

            var userIDParam = new SqlParameter("@UserID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            var ErrorMessage = new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 255)
            {
                Direction = ParameterDirection.Output
            };

            command.Parameters.Add(userIDParam);
            command.Parameters.Add(ErrorMessage);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            var success = userIDParam.Value != DBNull.Value && (int)userIDParam.Value > 0;

            if (success)
            {
                var userId = Convert.ToInt32(userIDParam.Value);
               loginUserResponseDTO.IsLogin = true;
                loginUserResponseDTO.UserID = userId;
                loginUserResponseDTO.Message = "Login Successful";
                return loginUserResponseDTO;
            }
            var message = ErrorMessage.Value?.ToString();
            loginUserResponseDTO.IsLogin = false;
            loginUserResponseDTO.Message = message;
            return loginUserResponseDTO;
        }
        public async Task<(bool success, string message)> ToggleUserActiveAsync(int userId, bool isActive)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("spToggleUserActive", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@UserID", userId);
            command.Parameters.AddWithValue("@IsActive",isActive);

            var errorMessageParam = new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 255)
            {
                Direction = ParameterDirection.Output
            };

            command.Parameters.Add(errorMessageParam);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            var message = errorMessageParam.Value?.ToString();
            var success = string.IsNullOrEmpty(message);

            return (success, message);


        }
    }
}
