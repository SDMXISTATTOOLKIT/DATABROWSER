using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataBrowser.Interfaces.Mail
{
    public interface IEmailService
    {
        Task SendAsync(string from, string to, string subject, string html);
        Task RecoveryPasswordAsync(string to, string token);
    }
}
