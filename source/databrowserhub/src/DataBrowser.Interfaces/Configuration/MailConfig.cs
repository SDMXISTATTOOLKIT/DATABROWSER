using System;
using System.Collections.Generic;
using System.Text;

namespace DataBrowser.Interfaces.Configuration
{
    public class MailConfig
    {
        public SmtpConfig Smtp { get; set; }
        public string DefaultMail { get; set; }
        public Dictionary<string, TemplateConfig> Templates { get; set; }

        public class SmtpConfig
        {
            public string Host { get; set; }
            public int Port { get; set; }
            public bool Secure { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string From { get; set; }
        }

        public class TemplateConfig
        {
            public string Subject { get; set; }
            public string Sender { get; set; }
            public string Message { get; set; }
        }

    }
}
