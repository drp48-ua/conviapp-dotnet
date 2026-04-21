using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace ConviAppWeb.Services
{
    /// <summary>
    /// EmailService — Sends email notifications via Gmail SMTP.
    /// Used for Enterprise plan applications.
    /// </summary>
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Send an Enterprise plan application notification email.
        /// </summary>
        public bool SendEnterpriseApplication(string applicantName, string applicantEmail, string company, string message)
        {
            try
            {
                var smtpHost = _config["Email:SmtpHost"] ?? "smtp.gmail.com";
                var smtpPort = int.Parse(_config["Email:SmtpPort"] ?? "587");
                var smtpUser = _config["Email:SmtpUser"] ?? "";
                var smtpPass = _config["Email:SmtpPassword"] ?? "";
                var toEmail = _config["Email:NotificationTo"] ?? "daniramonpoveda@gmail.com";

                if (string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPass))
                {
                    // Fallback: log to console if SMTP not configured
                    Console.WriteLine($"[EMAIL] Enterprise application from {applicantName} ({applicantEmail})");
                    Console.WriteLine($"[EMAIL] Company: {company}");
                    Console.WriteLine($"[EMAIL] Message: {message}");
                    return true;
                }

                using var smtp = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUser, smtpPass),
                    EnableSsl = true
                };

                var mailMsg = new MailMessage
                {
                    From = new MailAddress(smtpUser, "ConviApp Notificaciones"),
                    Subject = $"🏢 Nueva solicitud Enterprise — {applicantName}",
                    IsBodyHtml = true,
                    Body = $@"
                        <div style='font-family:Arial,sans-serif;max-width:600px;margin:0 auto;padding:20px;'>
                            <h2 style='color:#6366f1;'>🏢 Nueva Solicitud Plan Enterprise</h2>
                            <hr style='border:1px solid #eee;'/>
                            <p><strong>Nombre:</strong> {applicantName}</p>
                            <p><strong>Email:</strong> <a href='mailto:{applicantEmail}'>{applicantEmail}</a></p>
                            <p><strong>Empresa:</strong> {company}</p>
                            <p><strong>Mensaje:</strong></p>
                            <div style='background:#f5f5f5;padding:15px;border-radius:8px;'>
                                {message}
                            </div>
                            <hr style='border:1px solid #eee;margin-top:20px;'/>
                            <p style='color:#999;font-size:12px;'>ConviApp — {DateTime.Now:dd/MM/yyyy HH:mm}</p>
                        </div>"
                };

                mailMsg.To.Add(toEmail);
                smtp.Send(mailMsg);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EMAIL ERROR] {ex.Message}");
                return false;
            }
        }
    }
}
