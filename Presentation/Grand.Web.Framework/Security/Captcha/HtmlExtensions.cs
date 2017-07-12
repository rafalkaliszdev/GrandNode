//using System.IO;
//
//using System.Web.UI;
//using Grand.Core.Infrastructure;

//namespace Grand.Web.Framework.Security.Captcha
//{
//    public static class HtmlExtensions
//    {
//        public static string GenerateCaptcha(this HtmlHelper helper)
//        {
//            var captchaSettings = EngineContextExperimental.Current.Resolve<CaptchaSettings>();
//            var htmlWriter = new HtmlTextWriter(new StringWriter());

//            var captchaControl = new GRecaptchaControl(captchaSettings.ReCaptchaVersion)
//            {
//                Theme = captchaSettings.ReCaptchaTheme,
//                Id = "recaptcha",
//                PublicKey = captchaSettings.ReCaptchaPublicKey,
//                Language = captchaSettings.ReCaptchaLanguage
//            };
//            captchaControl.RenderControl(htmlWriter);

//            return htmlWriter.InnerWriter.ToString();
//        }
//    }
//}