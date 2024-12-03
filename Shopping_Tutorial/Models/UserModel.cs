using System.ComponentModel.DataAnnotations;

namespace Shopping_Tutorial.Models
{
	public class UserModel
	{
		public int Id { get; set; }
		[Required(ErrorMessage ="Làm ơn nhập Username")]
		public string Username { get; set; }
		[Required(ErrorMessage = "Làm ơn nhập Email"),EmailAddress]

		public string Email { get; set; }
		[DataType(DataType.Password),Required(ErrorMessage ="Làm ơn nhập pasword")]
		public string Password { get; set; }
	}
}
