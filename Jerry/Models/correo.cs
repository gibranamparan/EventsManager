using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace Jerry.Models
{
    public class Correo
    {
        [Key]
        public int correoID { get; set; }
        
        public string To { get; set; }

        [Display(Name ="Asunto")]
        public string Subject { get; set; }
        [Display(Name = "Contenido")]
        public string Body { get; set; }
        [Display(Name = "Correo administrador")]
        public string correoAdmin { get; set; }
        [Display(Name = "Contraseña")]
        public string contrasena { get; set; }
        [Display(Name = "smtpHost")]
        public string smtpHost { get; set; }
        [Display(Name = "Puerto")]
        public int puertoCorreo { get; set; }

        public ErrorEmail enviarCorreo(HttpPostedFileBase fileUploader, string emailDestino)
        {
            ErrorEmail err = new ErrorEmail();
            try { 
                MailMessage mail = new MailMessage(this.correoAdmin, emailDestino);
                if (fileUploader != null)
                {
                    string fileName = Path.GetFileName(fileUploader.FileName);
                    mail.Attachments.Add(new Attachment(fileUploader.InputStream, fileName));
                }

                mail.Subject = this.Subject;
                mail.Body = this.Body;
                mail.IsBodyHtml = false;
                SmtpClient smtp = new SmtpClient();
                //smtp.Host = "smtp-mail.outlook.com";
                smtp.Host = this.smtpHost;
                //smtp.Host = objModelMail.smtpHost;
                smtp.EnableSsl = true;
                NetworkCredential networkCredential = new NetworkCredential(this.correoAdmin, this.contrasena);
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = networkCredential;
                smtp.Port = this.puertoCorreo;
                smtp.Send(mail);
                err.code = ErrorEmail.ErrorEmailCode.SENT;

            }catch(Exception exc)
            {
                err.code = ErrorEmail.ErrorEmailCode.FAIL;
                err.msg = exc.Message;
            }

            return err;
        }

        public class ErrorEmail {
            public ErrorEmailCode code { get; set; } = ErrorEmailCode.NONE;
            public string msg { get; set; }
            
            public enum ErrorEmailCode{ NONE, SENT, FAIL }
        }
    }
}