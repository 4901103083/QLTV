using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QLThuVien
{
    public static class SendMail
    {
        public static string tentaikhoan = "huynhmyduc123@gmail.com";
        public static string matkhau = "qfydmkcugemjnaxm"; // tsss bwjk fsdj pdyd
        public static bool sendMailTo(string emailto, string content)
        {
            var fromAddress = new MailAddress(tentaikhoan, "Admin QLTV");
            var toAddress = new MailAddress(emailto, emailto);
            string fromPassword = matkhau;
            string subject = "Thông báo từ hệ thống quản lý thư viện";
            string body = content;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            try
            {
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
