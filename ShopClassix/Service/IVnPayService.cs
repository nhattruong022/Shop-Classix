using Shop_Classix.Models.ViewModels;

namespace Shop_Classix.Service
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context,VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);

    }
}
