using System.ComponentModel.DataAnnotations;
namespace HotelBookingAPI.DTOs.UserDTOs
{
    public class UserRoleDTO
    {
        [Required(ErrorMessage ="User ID is Required")]
        public int UserID { get; set; }
        [Required(ErrorMessage ="Role ID is Required")]
        public int RoleID { get; set; }
    }
}
