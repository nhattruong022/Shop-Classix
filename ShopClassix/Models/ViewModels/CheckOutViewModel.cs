using System.ComponentModel.DataAnnotations;

namespace Shop_Classix.Models.ViewModels
{
    public class CheckOutViewModel
    {
        [Required(ErrorMessage = "Receiver name is required")]
        public string Receiver { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }



        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^(\+84|0)\d{9,10}$", ErrorMessage = "Invalid Phone Number")]
        public string Phone { get; set; }



        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[a-zA-Z0-9]+@gmail\.com$", ErrorMessage = "Invalid email format ")]
        public string Email { get; set; }




        public double deposit {  get; set; }



        public string? OrderNotes { get; set; }




        public double Total { get; set; } // Calculated from the cart

        public List<CartItemViewModel> Items { get; set; } // Representing cart items


        

    }
}

