namespace HotelBookingAPI.DTOs.UserDTOs
{
    public class CreateUserResponseDTO
    {
        public int UserID { get; set; }
        public string Message { get; set; }
        public bool IsCreated { get; set; }
    }
}
