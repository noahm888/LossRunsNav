
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace LossRunsNavClient
{
    public static class HtmlDocumentPageExpectationsExtensions
    {
        public static bool ExpectLoginScreen(this HtmlDocument html)
        {
            if (
                html.HasElementInTemplate("lgnBroker_UserName")
                && html.HasElementInTemplate("lgnBroker_Password")
                && html.HasElementInTemplate("lgnBroker_LoginButton")
                )
            {
                return true;
            }
            return false;
        }

        public static bool ExpectSearchScreen(this HtmlDocument html)
        {
            if (
                html.HasElementInTemplate("txtPolicyNumber")
                && html.HasElementInTemplate("btnSearch")
                )
            {
                var iFrames = html.ElementInTemplate("resultsIFrameId");
                if(
                    iFrames.Any()
                    && iFrames.Where(iframe => string.IsNullOrEmpty(iframe.GetAttribute("src"))).Any()
                    )
                {
                    return true;
                }
            }
            return false;
        }

        public static bool ExpectMultipleResultsScreen(this HtmlDocument html)
        {
            return html.NavgSpecificSelectButtons().Any();
        }

        public static bool ExpectSingleResultScreen(this HtmlDocument html)
        {
            if (
                html.HasElementInTemplate("txtPolicyNumber")
                && html.HasElementInTemplate("btnSearch")
                )
            {
                var iFrames = html.ElementInTemplate("resultsIFrameId");
                if (
                    iFrames.Any()
                    && iFrames.Where(iframe => !string.IsNullOrEmpty(iframe.GetAttribute("src"))).Any()
                    )
                {
                    return true;
                }
            }
            return false;
        }
    }

    public static class HtmlDocumentExtensions
    {
        public static bool HasElementInTemplate(this HtmlDocument html, string fieldName)
        {
            return html.ElementInTemplate(fieldName).Any();
        }

        public static IEnumerable<HtmlElement> ElementInTemplate(this HtmlDocument html, string fieldName)
        {
            return
                (from HtmlElement tag in html.All
                 where tag != null && tag.Id != null && tag.Id.EndsWith(fieldName)
                 select tag);
        }

        public static IEnumerable<HtmlElement> MatchText(this HtmlDocument html, string value)
        {
            if (string.IsNullOrEmpty(value)) { return html.FindText(string.Empty); }
            return
                (from HtmlElement tag in html.All
                 where tag != null && tag.InnerText.Contains(value)
                 select tag);
        }

        public static IEnumerable<HtmlElement> FindText(this HtmlDocument html, string value)
        {
            string searchValue = value ?? string.Empty;
            return
                (from HtmlElement tag in html.All
                 where tag != null && value == tag.InnerText
                 select tag);
        }

        public static bool SetFieldInTemplate(this HtmlDocument html, string fieldName, string value)
        {
            var templateField =
                (from HtmlElement tag in html.All
                 where tag != null && !string.IsNullOrEmpty(tag.Id) && tag.Id.EndsWith(fieldName)
                 select tag).SingleOrDefault();
            if (templateField == null) { return false; }
            templateField.SetAttribute("value", value);
            return true;
        }

        public static bool ClickInTemplate(this HtmlDocument html, string fieldName)
        {
            var templateField =
                (from HtmlElement tag in html.All
                 where tag != null && !string.IsNullOrEmpty(tag.Id) && tag.Id.EndsWith(fieldName)
                 select tag).SingleOrDefault();
            if (templateField == null) { return false; }
            templateField.InvokeMember("click");
            return true;
        }

        public static IEnumerable<HtmlElement> NavgSpecificSelectButtons(this HtmlDocument html)
        {
            return
                (from HtmlElement tag in html.All
                 where tag != null
                 && tag.GetAttribute("type") == "button"
                 && tag.GetAttribute("value") == "Select"
                 select tag);
        }
    }
}