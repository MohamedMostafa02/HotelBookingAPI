namespace HotelBookingAPI.DTOs.UserDTOs
{
    public class UpdateUserResponseDTO
    {
        public int UserID { get; set; }
        public string Message { get; set; }
        public bool IsUpdated { get; set; }
    }
}
