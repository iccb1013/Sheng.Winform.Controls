using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Sheng.Winform.Controls.Demo
{
    class StartPageCodePage
    {
        public StartPageCodePage()
        {
        }

        public virtual void RenderHeaderSection(StringBuilder builder)
        {
            builder.Append("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\" \"http://www.w3.org/TR/html4/loose.dtd\">");
            builder.Append("<html>");
            builder.Append("<head>");
            builder.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
            builder.Append("<title>欢迎</title>");
            //builder.Append("<link href=\"" +
            //    _environmentService.DataPath +
            //    Path.DirectorySeparatorChar + "resources" +
            //    Path.DirectorySeparatorChar + "startpage" +
            //    Path.DirectorySeparatorChar + "style.css\" rel=\"stylesheet\" type=\"text/css\">");
            builder.Append("</head>");

            builder.Append("<body style=\"font-size:14px;\">");
            builder.Append("<table width=\"100%\"  border=\"0\" cellspacing=\"0\" cellpadding=\"0\">");
            builder.Append("<tr>");
            //builder.Append("<td><img src=\"" + _environmentService.DataPath +
            //    Path.DirectorySeparatorChar + "resources" +
            //    Path.DirectorySeparatorChar + "startpage" +
            //    Path.DirectorySeparatorChar + "Top.png\" width=\"672\" height=\"74\"></td>");
            builder.Append("<td width=\"30\">&nbsp;</td>");
            builder.Append("<td height=\"90\" style=\"font-size:20px;\">");
            builder.Append("升讯威 .Net WinForm 控件库<br/>");
            builder.Append("Sheng.WinForm.Controls<br/>");
            builder.Append("Github : https://github.com/iccb1013/Sheng.Winform.Controls");
            builder.Append("</td>");
            builder.Append("</tr>");
            builder.Append("</table>");

            builder.Append("<table width=\"100%\"  border=\"0\" cellspacing=\"0\" cellpadding=\"0\">");
            builder.Append("<tr>");
            builder.Append("<td width=\"30\">&nbsp;</td>");
            builder.Append("<td height=\"30\">");
            builder.Append("<a href=\"startpage://start/\">启始页</a>&nbsp;&nbsp;&nbsp;");
            builder.Append("<a href=\"startpage://help/\">帮助与技术支持</a>&nbsp;&nbsp;&nbsp;");
            //builder.Append("<a href=\"http://www.shengxunwei.com\" target=\"_blank\">网站</a>&nbsp;&nbsp;&nbsp;");
            builder.Append("</td>");
            builder.Append("</tr>");
            builder.Append("</table>");

           
        }

        public virtual void RenderSectionStartBody(StringBuilder builder)
        {
            builder.Append("<table width=\"100%\"  border=\"0\" cellspacing=\"0\" cellpadding=\"0\">");
            builder.Append("<tr><td colspan='2'>&nbsp;</td></tr>");
            builder.Append("<tr>");
            builder.Append("<td width=\"30\">&nbsp;</td>");
            builder.Append("<td>");
            builder.Append("升讯威 .Net WinForm 控件库 是一个开源的控件库，该控件库源于 升讯威 WinForm Designer IDE。开源的目的是为了分享 .Net Winform 控件开发的方法和理念，你可以在此开源库的基础上进行更深入的二次开发。如需商业应用，请于权版画面声明使用了本控件库，并给出 Github 地址。 <br/><br/>");

            builder.Append("此启始页控件，是由 WebBrowser 定制的，允许你实现一个类似 VisualStudio 一样的启始页。<br/>");
            builder.Append("也可以使用此控件，实现 Html 页面与客户端混合的应用程序。<br/>");
            builder.Append("此页面中的数据可以由 C# 程序输出，此页面中的 JavaScript 函数可以由 C# 调用，反之亦然，页面中的 JavaScript 代码可以调用 C# 代码中的函数。<br/>");

            builder.Append("</td>");
            builder.Append("</tr>");
            builder.Append("</table>");

            builder.Append("<table width=\"100%\"  border=\"0\" cellspacing=\"0\" cellpadding=\"0\">");
            builder.Append("<tr>");
            builder.Append("<td>&nbsp;</td>");
            builder.Append("</tr>");
            builder.Append("<tr>");
            builder.Append("<td width=\"30\"><!--这里是最近项目列表--></td>");
            builder.Append("<td>");

            //这里是最近项目列表

            builder.Append("最近项目:");
            builder.Append("<table  width=\"600\"  border=\"1\" cellpadding=\"0\" cellspacing=\"0\" bordercolor=\"#CCCCCC\" style=\"border-collapse:collapse;\">");
            builder.Append("<tr bgcolor=\"#FAFAFA\">");
            builder.Append("<td width=\"180\" height=\"24\" bgcolor=\"#FAFAFA\">&nbsp;&nbsp;<strong>名称</strong></td>");
            builder.Append("<td>&nbsp;&nbsp;<strong>位置</strong></td>");
            builder.Append("</tr>");


            //循环部分
            for (int i = 0; i < 3; i++)
            {

                builder.Append("<tr>");
                builder.Append("<td height=\"24\">&nbsp;");
                builder.Append("项目名称");
                builder.Append("</td>");
                builder.Append("<td>");
                builder.Append("使用 C# 输出最近的项目。");
                builder.Append("</td>");
            }
            //循环部分结束

            builder.Append("</table>");

            //最近项目列表输出结束

            builder.Append("</td>");
            builder.Append("</tr>");
            builder.Append("<tr>");
            builder.Append("<td>&nbsp;</td>");
            builder.Append("</tr>");
            builder.Append("</table>");

            builder.Append("<table width=\"100%\"  border=\"0\" cellspacing=\"0\" cellpadding=\"0\">");
            builder.Append("<tr>");
            builder.Append("<td width=\"30\">&nbsp;</td>");
            builder.Append("<td>");
            builder.Append("<input type=\"button\" id=\"btnOpenProject\" value=\"打开项目\">");
            builder.Append("&nbsp;&nbsp;&nbsp;&nbsp;");
            builder.Append("<input type=\"button\" id=\"btnNewProject\" value=\"新建项目\">");
            builder.Append("</td>");
            builder.Append("</tr>");
            builder.Append("</table>");
        }

        public virtual void RenderSectionHelp(StringBuilder builder)
        {
            builder.Append(@"
            <table width='100%'  border='0' cellspacing='0' cellpadding='0'>
              <tr>
                <td colspan='2'>&nbsp;</td>
              </tr>
              <tr>
                <td width='30'><!--这里是最近项目列表--></td>
              <td><p>如何获取帮助?<br>
                <br>
                  官方网站：http://www.shengxunwei.com<br/>
                  欢迎加入QQ群：591928344<br/>
                  有任何疑问或咨询也可联系 QQ：279060597
                </p>
                </td>
              </tr>
              <tr>
                <td colspan='2'>&nbsp;</td>
              </tr>
            </table>
            ");
        }

        public virtual void RenderFinalPageBodySection(StringBuilder builder)
        {
            builder.Append("</body>");
        }

        public virtual void RenderPageEndSection(StringBuilder builder)
        {
            builder.Append("</html>");
        }

        public string Render(string section)
        {
            StringBuilder builder = new StringBuilder(2048);
            RenderHeaderSection(builder);

            switch (section.ToLowerInvariant())
            {
                case StartPageScheme.START_HOST:
                    RenderSectionStartBody(builder);
                    break;
                case StartPageScheme.HELP_HOST:
                    RenderSectionHelp(builder);
                    break;
            }

            RenderFinalPageBodySection(builder);
            RenderPageEndSection(builder);

            return builder.ToString();
        }
    }
}
