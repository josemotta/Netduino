using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using Toolbox.NETMF.NET;

namespace SMTPClient
{
    public class Program
    {
        public static void Main()
        {
            // By defining the CORRECT! date, mail messages could get a lower spam score in spam filters
            //Utility.SetLocalTime(new DateTime(2011, 10, 16, 20, 43, 0, 0));

            // Defines the sender
            SMTP_Client.MailContact From = new SMTP_Client.MailContact("administrator@localhost", "Your name");
            // Defines the receiver
            SMTP_Client.MailContact Receiver = new SMTP_Client.MailContact("someone@else", "Recipients name");
            // Defines the mail message
            SMTP_Client.MailMessage Message = new SMTP_Client.MailMessage("Small test result");
            Message.Body = "This mail is sent by a Netduino :-)\r\n";
            Message.Body += "Good day!";

            // Initializes the mail sender class (When your ISP blocks port 25, try 587. A lot of SMTP servers respond to that as well!)
            SMTP_Client Sender = new SMTP_Client("smtp.yourisp.com");

            // Sends the mail
            Sender.Send(Message, From, Receiver);
        }

    }
}
