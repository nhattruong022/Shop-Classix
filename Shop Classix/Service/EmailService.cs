using System.Net;
using System.Net.Mail;

//gửi thông tin liên hệ bằng email

namespace Shop_Classix.Service
{
    public interface IEmailService
    {
        Task SendEmail(string repceptor, string subject, string body);
    }
    public class EmailService:IEmailService
    {
        private readonly IConfiguration configuration;

        public EmailService(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public async Task SendEmail(string repceptor, string subject, string body)
        {
            //nhận thông tin cấu hình từ appsettings.json
            var email = configuration.GetValue<string> ("EMAIL_CONFIGURATION:EMAIL");
            var password=configuration.GetValue<string>("EMAIL_CONFIGURATION:PASSWORD");
            var host = configuration.GetValue<string>("EMAIL_CONFIGURATION:HOST");
            var port=configuration.GetValue<int>("EMAIL_CONFIGURATION:PORT");


            var smtpClient = new SmtpClient(host, port) //gửi email qua giao thức SMTP
            {
                EnableSsl = true,  //bật SSL/TLS mã hóa kết nối giữa ứng dụng và máy chủ SMTP-> bảo mật thông tin(email và password)
                UseDefaultCredentials = false, //không sử dụng thông tin đăng nhập của hệ thống mặc định
                Credentials = new NetworkCredential(email, password)
            };


            //tạo đối tượng MailMessage và bật hỗ trợ html
            var message = new MailMessage(email!, repceptor, subject, body)
            {
                IsBodyHtml = true, //cấu hình hỗ trợ HTML
            };

            //gửi email
            await smtpClient.SendMailAsync(message);
        }

    }
}
