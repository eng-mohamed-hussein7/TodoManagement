namespace Infrastructure.Services.EmailServices;

public static class EmailTemplates
{
    public static string GetResetPasswordTemplate(string resetLink)
    {
        return @"
    <!DOCTYPE html>
    <html lang='ar'>
    <head>
        <meta charset='UTF-8'>
        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
        <title>إعادة تعيين كلمة المرور</title>
        <style>
            body { font-family: 'Arial', sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; direction: rtl; text-align: right; }
            .container { max-width: 500px; margin: 50px auto; background: #ffffff; border-radius: 12px; 
                         padding: 30px; box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1); text-align: right; }
            .header { font-size: 22px; font-weight: bold; color: #F97218; margin-bottom: 15px; text-align: center; }
            .logo-container { text-align: center; margin-bottom: 15px; }
            .logo-container img { max-width: 120px; display: block; margin: 0 auto; border-radius: 50%; }
            .content { font-size: 18px; color: #333; line-height: 1.6; }
            .button-container { margin: 30px 0; text-align: center; }
            .button { background: #F97218;    color: #ffffff !important;  padding: 14px 25px; border-radius: 30px; font-size: 16px; 
                      text-decoration: none; font-weight: bold; display: inline-block; transition: all 0.3s ease; }
            .button:hover { background: #e06514; }
            .footer { margin-top: 20px; font-size: 14px; color: #777; border-top: 1px solid #ddd; padding-top: 15px; text-align: center; }
            .footer a { color: #F97218; text-decoration: none; font-weight: bold; }
            @media (max-width: 500px) { 
                .container { padding: 20px; } 
                .button { padding: 12px 20px; font-size: 14px; } 
            }
        </style>
    </head>
    <body>
        <div class='container'>
            <div class='logo-container'>
                <img src='https://w-birgahmi.runasp.net/mdt-Default.png' alt='Logo'>
            </div>
            <div class='header'>إعادة تعيين كلمة المرور</div>
            <div class='content'>
                <p>مرحبًا</p>
                <p>لقد طلبت إعادة تعيين كلمة المرور الخاصة بك. انقر على الزر أدناه لإكمال العملية:</p>
                <div class='button-container'>
                    <a href='" + resetLink + @"' class='button'>إعادة تعيين كلمة المرور</a>
                </div>
            </div>
            <div class='footer'>
                <p>للمساعدة، يرجى <a href='themdt7@gmail.com'>الاتصال بالدعم</a></p>
                <p>&copy; 2025 جميع الحقوق محفوظة.</p>
            </div>
        </div>
    </body>
    </html>";
    }
}
