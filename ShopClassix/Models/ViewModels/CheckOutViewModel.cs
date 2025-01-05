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
        [Phone(ErrorMessage = "Invalid phone number")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }


        public decimal deposit {  get; set; }



        public string OrderNotes { get; set; }

        [Required(ErrorMessage = "Payment method is required")]
        public int PaymentMethod { get; set; } // Example: 1 = Check Payment, 2 = COD

        public decimal Total { get; set; } // Calculated from the cart

        public List<CartItemViewModel> Items { get; set; } // Representing cart items


        

    }
}

