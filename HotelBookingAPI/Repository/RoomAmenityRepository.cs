using HotelBookingAPI.Connection;
using HotelBookingAPI.DTOs.AmenityDTOs;
using HotelBookingAPI.DTOs.RoomAmenityDTOs;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Data.SqlClient;
using System.Data;
namespace HotelBookingAPI.Repository
{
    public class RoomAmenityRepository
    {
        private SqlConnectionFactory _sqlConnectionFactory;

        public RoomAmenityRepository(SqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<List<AmenityResponseDTO>> FetchRoomAmenitiesByRoomTypeIdAsync(int roomTypeId)
        {
            var response = new List<AmenityResponseDTO>();
            using var connection = _sqlConnectionFactory.CreateConnection();
            using var command = new SqlCommand("spFetchRoomAmenitiesByRoomTypeID", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@RoomTypeID", roomTypeId);

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while(await reader.ReadAsync())
            {
                response.Add(new AmenityResponseDTO
                {
                    AmenityID = reader.GetInt32(reader.GetOrdinal("AmenityID")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Description = reader.GetString(reader.GetOrdinal("Description")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))

                });
            }

            return response;

        }

        public async Task<List<FetchRoomAmenityResponseDTO>> FetchRoomTypesByAmenityIdAsync(int amenityId)
        {
            var response = new List<FetchRoomAmenityResponseDTO>();

            using var connection = _sqlConnectionFactory.CreateConnection();
            using var command = new SqlCommand("spFetchRoomTypesByAmenityID", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@AmenityID", amenityId);

            await connection.OpenAsync();

            var reader = await command.ExecuteReaderAsync();

            while(await reader.ReadAsync())
            {
                response.Add(new FetchRoomAmenityResponseDTO
                {
                    RoomTypeID = reader.GetInt32(reader.GetOrdinal("RoomTypeID")),
                    TypeName = reader.GetString(reader.GetOrdinal("TypeName")),
                    Description = reader.GetString(reader.GetOrdinal("Description")),
                    AccessibilityFeatures = reader.GetString(reader.GetOrdinal("AccessibilityFeatures")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                });
            }
            return response;

        }

        public async Task<RoomAmenityResponseDTO> AddRoomAmenityAsync(RoomAmenityDTO input)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();
            using var command = new SqlCommand("spAddRoomAmenity", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@RoomTypeID", input.RoomTypeID);
            command.Parameters.AddWithValue("@AmenityID", input.AmenityID);

            var statusParam = new SqlParameter("@Status", SqlDbType.Bit) 
            { 
                Direction = ParameterDirection.Output 
            };
            var messageParam = new SqlParameter("@Message", SqlDbType.NVarChar, 255) 
            { 
                Direction = ParameterDirection.Output
            };

            command.Parameters.Add(statusParam);
            command.Parameters.Add(messageParam);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return new RoomAmenityResponseDTO
            {
                IsSuccess = (bool)statusParam.Value,
                Message = (string)messageParam.Value
            };
        }

        public async Task<RoomAmenityResponseDTO> DeleteRoomAmenityAsync(RoomAmenityDTO input)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();
            using var command = new SqlCommand("spDeleteSingleRoomAmenity", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@RoomTypeID", input.RoomTypeID);
            command.Parameters.AddWithValue("@AmenityID", input.AmenityID);

            var statusParam = new SqlParameter("@Status", SqlDbType.Bit)
            {
                Direction = ParameterDirection.Output
            };

            var messageParam = new SqlParameter("@Message", SqlDbType.NVarChar, 255)
            {
                Direction = ParameterDirection.Output
            };

            command.Parameters.Add(statusParam);
            command.Parameters.Add(messageParam);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return new RoomAmenityResponseDTO
            {
                IsSuccess = (bool)statusParam.Value,
                Message = (string)messageParam.Value
            };
        }
        public async Task<RoomAmenityResponseDTO> BulkInsertRoomAmenitiesAsync(RoomAmenitiesBulkInsertUpdateDTO input)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();
            using var command = new SqlCommand("spBulkInsertRoomAmenities", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@RoomTypeID", input.RoomTypeID);
            command.Parameters.Add(CreateAmenityIDTableParameter(input.AmenityIDs));
            var statusParam = new SqlParameter("@Status", SqlDbType.Bit) { Direction = ParameterDirection.Output };
            var messageParam = new SqlParameter("@Message", SqlDbType.NVarChar, 255) { Direction = ParameterDirection.Output };
            command.Parameters.Add(statusParam);
            command.Parameters.Add(messageParam);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
            return new RoomAmenityResponseDTO
            {
                IsSuccess = (bool)statusParam.Value,
                Message = (string)messageParam.Value
            };
        }

        public async Task<RoomAmenityResponseDTO> BulkUpdateRoomAmenitiesAsync(RoomAmenitiesBulkInsertUpdateDTO input)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();
            using var command = new SqlCommand("spBulkUpdateRoomAmenities", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@RoomTypeID", input.RoomTypeID);
            command.Parameters.Add(CreateAmenityIDTableParameter(input.AmenityIDs));
            var statusParam = new SqlParameter("@Status", SqlDbType.Bit) { Direction = ParameterDirection.Output };
            var messageParam = new SqlParameter("@Message", SqlDbType.NVarChar, 255) { Direction = ParameterDirection.Output };
            command.Parameters.Add(statusParam);
            command.Parameters.Add(messageParam);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
            return new RoomAmenityResponseDTO
            {
                IsSuccess = (bool)statusParam.Value,
                Message = (string)messageParam.Value
            };
        }

        public async Task<RoomAmenityResponseDTO> DeleteAllRoomAmenitiesByRoomTypeIDAsync(int roomTypeId)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();
            using var command = new SqlCommand("spDeleteAllRoomAmenitiesByRoomTypeID", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@RoomTypeID", roomTypeId);
            var statusParam = new SqlParameter("@Status", SqlDbType.Bit) { Direction = ParameterDirection.Output };
            var messageParam = new SqlParameter("@Message", SqlDbType.NVarChar, 255) { Direction = ParameterDirection.Output };
            command.Parameters.Add(statusParam);
            command.Parameters.Add(messageParam);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
            return new RoomAmenityResponseDTO
            {
                IsSuccess = (bool)statusParam.Value,
                Message = (string)messageParam.Value
            };
        }

        public async Task<RoomAmenityResponseDTO> DeleteAllRoomAmenitiesByAmenityIDAsync(int amenityId)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();
            using var command = new SqlCommand("spDeleteAllRoomAmenitiesByAmenityID", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@AmenityID", amenityId);
            var statusParam = new SqlParameter("@Status", SqlDbType.Bit) { Direction = ParameterDirection.Output };
            var messageParam = new SqlParameter("@Message", SqlDbType.NVarChar, 255) { Direction = ParameterDirection.Output };
            command.Parameters.Add(statusParam);
            command.Parameters.Add(messageParam);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
            return new RoomAmenityResponseDTO
            {
                IsSuccess = (bool)statusParam.Value,
                Message = (string)messageParam.Value
            };
        }
        private SqlParameter CreateAmenityIDTableParameter(IEnumerable<int> amenityIds)
        {
            var table = new DataTable();
            table.Columns.Add("AmenityID", typeof(int));
            foreach (var id in amenityIds)
            {
                table.Rows.Add(id);
            }
            var param = new SqlParameter
            {
                ParameterName = "@AmenityIDs",
                SqlDbType = SqlDbType.Structured,
                Value = table,
                TypeName = "AmenityIDTableType"
            };
            
            return param;
        }
    }
}
