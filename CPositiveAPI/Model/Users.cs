using System.ComponentModel.DataAnnotations;

namespace CPositiveAPI.Model
{
    public class Users
    {
        [Key]
        public int UserId {  get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword {  get; set; }
        public string EmailId { get; set;}
        public string Mobileno {  get; set; }
    }
}
