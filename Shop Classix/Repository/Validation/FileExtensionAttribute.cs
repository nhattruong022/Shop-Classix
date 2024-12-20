using System.ComponentModel.DataAnnotations;
using System.Net.WebSockets;

namespace Shop_Classix.Repository.Validation
{
    public class FileExtensionAttribute:ValidationAttribute
    {
        protected override ValidationResult IsValid(object value,ValidationContext validationContext)
        {
            if(value is IFormFile file)
            {
                var extenssion = Path.GetExtension(file.FileName);
                string[] extensions = { "jpg", "png", "jpeg" };

                bool result=extenssion.Any(x=>extenssion.EndsWith(x));

                if(!result)
                {
                    return new ValidationResult("Allowed extensions are jpg or png or jpeg");
                }
            }
            return ValidationResult.Success; 
        }
    }
}
