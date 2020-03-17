using Antlr.Runtime.Misc;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;

namespace CMS.Web.Helpers
{
    public static class HtmlHelpers
    {
        public static MvcHtmlString CheckBoxForBootstrap(string name, bool isChecked)
        {
            TagBuilder checkbox = new TagBuilder("input");
            checkbox.Attributes.Add("type", "checkbox");
            checkbox.Attributes.Add("name", name);
            checkbox.Attributes.Add("id", name);
            checkbox.Attributes.Add("data-val", "true");
            checkbox.Attributes.Add("value", "true");
            if (isChecked)
                checkbox.Attributes.Add("checked", "checked");

            TagBuilder label = new TagBuilder("label");
            //nest the checkbox inside the label
            label.InnerHtml = checkbox.ToString(TagRenderMode.Normal);

            TagBuilder hidden = new TagBuilder("input");
            hidden.Attributes.Add("type", "hidden");
            hidden.Attributes.Add("name", name);
            hidden.Attributes.Add("value", "false");

            return MvcHtmlString.Create(
            label.ToString(TagRenderMode.Normal)
            + hidden.ToString(TagRenderMode.Normal)
            );
        }

        public static MvcHtmlString SpanFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null)
        {
            var valueGetter = expression.Compile();
            var value = valueGetter(helper.ViewData.Model);

            var span = new TagBuilder("span");
            span.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            if (value != null)
            {
                span.SetInnerText(value.ToString());
            }

            return MvcHtmlString.Create(span.ToString());
        }
    }
}