using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Unit.Application.DTOs.Request;
using Unit.Application.Sevices;
using Unit.Application.Util;

namespace Unit.Infra.Services
{
    public class EmailService: IEmailService
    {
        private readonly IServicoService _servicoService;
        private ConfigServico ConfigServico;

        public EmailService(IServicoService servicoService)
        {
            _servicoService = servicoService;
            this.ConfigServico = _servicoService.ConfigServico("Email").Result;
        }
        public async Task<Reply> SendEmailAsync(SendEmailRequest entidade)
        {
            var retorno = new Reply();

            try
            {
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(this.ConfigServico.Sender, "Admin")
                };

                mail.To.Add(new MailAddress(entidade.To));
                mail.CC.Add(new MailAddress(this.ConfigServico.CopyTo));
                mail.Subject = $"{entidade.Subject} at {string.Format("{0:dd/MM/yyyy hh:mm}", System.DateTime.Now)}";
                mail.Body = entidade.Body;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;

                //outras opções
                //mail.Attachments.Add(new Attachment(arquivo));
                //

                using (SmtpClient smtp = new SmtpClient(this.ConfigServico.Url,
                                                        Convert.ToInt32(this.ConfigServico.Port)))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(this.ConfigServico.User,
                                                             this.ConfigServico.Password);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }

                retorno.Messages.Add($"E-mail enviado para {entidade.To} com sucesso.");
            }
            catch (Exception)
            {
                retorno.Success = false;
                retorno.Messages.Add("Não foi possível enviar o e-mail. Tente novamente mais tarde.");
            }

            return retorno;
        }
    }
}
