using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Jerry.Models
{
    public class Correo
    {
        [Key]
        public int correoID { get; set; }
        [Display(Name ="Asunto")]
        public string Subject { get; set; }
        [Display(Name = "Contenido")]
        public string Body { get; set; }
        [Display(Name = "Correo administrador")]
        public string correoAdmin { get; set; }

        public ErrorEmail enviarCorreo(string emailDestino,string clientName, Rotativa.ViewAsPdf rotativaFile,
            ControllerContext controllerContext, string aditionalBody)
        {
            var fileBytes = rotativaFile.BuildPdf(controllerContext);
            Stream stream = new MemoryStream(fileBytes);

            String fileName = rotativaFile.FileName;
            if (string.IsNullOrEmpty(fileName))
                fileName = "tempPDF.pdf";

            return enviarCorreo(emailDestino, clientName, stream, fileName, aditionalBody);
        }

        //public ErrorEmail enviarCorreo(HttpPostedFileBase fileUploader, string emailDestino)
        public ErrorEmail enviarCorreo(string emailDestino,string clientName, Stream fileStream, string fileName, string aditionalBody)
        {
            ErrorEmail err = new ErrorEmail();
            try {
                MailMessage mail = new MailMessage(new MailAddress(this.correoAdmin, "Jerry's System"),new MailAddress(emailDestino,clientName));
                mail.Bcc.Add(new MailAddress("gibranamparan@netcodesolutions.net", "Desarrollador"));
                //mail.Bcc.Add(new MailAddress("jerrycanez@hotmail.com", "Propietario"));

                if (fileStream != null)
                    mail.Attachments.Add(new Attachment(fileStream, fileName));

                mail.Subject = this.Subject;
                mail.Body = this.Body;
                mail.Body += aditionalBody != null ? aditionalBody : string.Empty;
                mail.IsBodyHtml = false;
                SmtpClient smtp = new SmtpClient();
                /*smtp.Host = this.smtpHost;
                smtp.EnableSsl = this.sslEnabled;
                NetworkCredential networkCredential = new NetworkCredential(this.correoAdmin, this.contrasena);
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = networkCredential;
                smtp.Port = this.puertoCorreo;*/
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