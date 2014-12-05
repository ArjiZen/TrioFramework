using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Bingosoft.TrioFramework.Models;
using Bingosoft.TrioFramework.Mvc.Models;

namespace Bingosoft.TrioFramework.Mvc.Extensions {
    /// <summary>
    /// 用于工作流的页面扩展
    /// </summary>
    public static class WorkflowHtmlExtension {

        /// <summary>
        /// 用于工作流的文本框控件
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString WorkflowTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes) {
            var isReadonly = (bool)htmlHelper.ViewData["Readonly"];
            if (isReadonly) {
                return htmlHelper.DisplayTextFor(expression);
            } else {
                return htmlHelper.TextBoxFor(expression, htmlAttributes);
            }
        }

        /// <summary>
        /// 用于工作流的文本域控件
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString WorkflowTextareaFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes) {
            var isReadonly = (bool)htmlHelper.ViewData["Readonly"];
            if (isReadonly) {
                return htmlHelper.DisplayTextFor(expression);
            } else {
                return htmlHelper.TextAreaFor(expression, htmlAttributes);
            }
        }

        /// <summary>
        /// 用于工作流的下拉列表控件
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="dictionaryCode"></param>
        /// <param name="expression"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString WorkflowDropdownFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string dictionaryCode, object htmlAttributes) {
            var isReadonly = (bool)htmlHelper.ViewData["Readonly"];
            var collection = DictionaryCollection.GetByCode(dictionaryCode);
            var selectListItems = collection.ToSelectListItems();
            if (isReadonly) {
                ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
                var value = metadata.Model.ToString();
                
                TagBuilder tagBuilder = null;
                tagBuilder = new TagBuilder("span");
                if (collection.ContainsCode(value)) {
                    tagBuilder.SetInnerText(collection[value]);
                } else {
                    tagBuilder.SetInnerText(value);
                }
                return new MvcHtmlString(tagBuilder.ToString());
            } else {
                return htmlHelper.DropDownListFor(expression, selectListItems, htmlAttributes);
            }
        }

        /// <summary>
        /// 用于工作流的下拉列表控件
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="selectListItems"></param>
        /// <param name="expression"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString WorkflowDropdownFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectListItems, object htmlAttributes) {
            var isReadonly = (bool)htmlHelper.ViewData["Readonly"];
            if (isReadonly) {
                return htmlHelper.DisplayTextFor(expression);
            } else {
                return htmlHelper.DropDownListFor(expression, selectListItems, htmlAttributes);
            }
        }


        /// <summary>
        /// 用于工作流的隐藏文本框控件
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MvcHtmlString WorkflowHiddenFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression) {
            var isReadonly = (bool)htmlHelper.ViewData["Readonly"];
            if (isReadonly) {
                return new MvcHtmlString("");
            } else {
                return htmlHelper.HiddenFor(expression);
            }
        }

    }
}