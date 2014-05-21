using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Text;
using System.Net;

namespace ZsfProject.Tools
{
    public static class SendEmail
    {
        public enum EmailType { ResetPassword, RegisterSucceed,SystemErrorInfo }
        public static void SendEmailBase(string title, string content, EmailType emailType, string emailTo)
        {
            MailMessage message = new MailMessage();
            string mailFrom = "";
            //设置优先级
            message.Priority = MailPriority.Normal;
            //接收方
            message.To.Add(emailTo);
            //标题
            message.Subject = title + "—宅食府(zhaishifu.com)";
            message.SubjectEncoding = Encoding.GetEncoding("gb2312");
            //邮件正文是否支持HTML
            message.IsBodyHtml = true;
            //正文编码
            message.BodyEncoding = Encoding.GetEncoding("gb2312");
            message.Body = content;

            //实例化SmtpClient
            SmtpClient client = new SmtpClient("smtp.exmail.qq.com", 25);
            //出站方式设置为NetWork
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = false;
            //smtp服务器验证并制定账号密码
            switch (emailType)
            {
                case EmailType.ResetPassword:
                    mailFrom = "service@zhaishifu.com";
                    client.Credentials = new NetworkCredential(mailFrom, "jin4828466");
                    break;
                case EmailType.SystemErrorInfo:
                    mailFrom = "admin@zhaishifu.com";
                    client.Credentials = new NetworkCredential(mailFrom, "jin_4828466");
                    break;
            }
            //设置收件方看到的邮件来源为:发送方邮件地址、来源标题、编码
            message.From = new MailAddress(mailFrom, "宅食府", Encoding.GetEncoding("gb2312"));

            client.Send(message);
        }

        public static void ResetPassword(string emailTo,string newPwd)
        {
            SendEmailBase("重置密码", "我们为您生成了临时密码为&nbsp;<strong>" + newPwd + "</strong>&nbsp;。请您登陆后立刻修改！", EmailType.ResetPassword, emailTo);
        }

        public static void SendErrorReport(Exception error,string fromUrl)
        {
            string errorMsg = "<strong>错误内容:</strong><br/>" + error.Message.Replace("\n", "<br/>");
            if (!String.IsNullOrWhiteSpace(fromUrl))
                errorMsg += "<br/><br/><strong>错误网址：</strong><br/>" + fromUrl;
            if (error.InnerException != null)
                errorMsg += "<br/><br/><strong>错误详情: </strong><br/>" + error.InnerException.Message.Replace("\n", "<br/>");
            if (!String.IsNullOrWhiteSpace(error.StackTrace))
                errorMsg += "<br/><br/><strong>错误路径：</strong><br/>" + error.StackTrace.Replace("\n", "<br/>");


            SendEmailBase("系统错误报告", errorMsg, EmailType.SystemErrorInfo, "error@zhaishifu.com");
        }
    }
}