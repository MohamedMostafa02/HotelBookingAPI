namespace HotelBookingAPI.DTOs.UserDTOs
{
    public class LoginUserResponseDTO
    {
        public int UserID { get; set; }
        public string Message { get; set; }
        public bool IsLogin { get; set; }
    }
}
