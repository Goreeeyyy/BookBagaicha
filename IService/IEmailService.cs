<<<<<<< HEAD
﻿namespace BookBagaicha.IService
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
=======
﻿using System.Threading.Tasks;

namespace BookBagaicha.IService
{
    public interface IEmailService
    {
        //sending emails
        Task SendEmailAsync(string to, string subject, string body, bool isHtml = false);
>>>>>>> 17faaceed86e8d33184d627fb7213dea0f26f325
    }
}