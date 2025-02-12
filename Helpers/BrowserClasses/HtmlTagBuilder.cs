// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
#if !(SyncfusionDNX5_0)
using System.Web;
#else
using Microsoft.AspNetCore.Html;
#endif
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.AspNetCore.Html;

namespace SGCApp.Helpers.BrowserClasses
{
#if !SyncfusionFramework3_5
    [TypeForwardedFrom("System.Web.Mvc, Version=2.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")]
#endif

    public class TagBuilder
    {
        private string _idAttributeDotReplacement;
        private string _innerHtml;


        public TagBuilder(string tagName)
        {
            if (String.IsNullOrEmpty(tagName))
                throw new ArgumentException("Value cannot be null or an empty string.");

            TagName = tagName;
            Attributes = new SortedDictionary<string, string>(StringComparer.Ordinal);

        }
        public string TagName
        {
            get;
            private set;
        }

        public IDictionary<string, string> Attributes
        {
            get;
            private set;
        }


        public string IdAttributeDotReplacement
        {
            get
            {
                if (String.IsNullOrEmpty(_idAttributeDotReplacement))
                {
                    _idAttributeDotReplacement = "_";
                }
                return _idAttributeDotReplacement;
            }
            set
            {
                _idAttributeDotReplacement = value;
            }
        }


        public string InnerHtml
        {
            get
            {
                return _innerHtml ?? String.Empty;
            }
            set
            {
                _innerHtml = value;
            }
        }
        public void AddCssClass(string value)
        {
            string currentValue;

            if (Attributes.TryGetValue("class", out currentValue))
            {
                Attributes["class"] = value + " " + currentValue;
            }
            else
            {
                Attributes["class"] = value;
            }
        }

        public static string CreateSanitizedId(string originalId)
        {
            return CreateSanitizedId(originalId, HtmlIDHelper.IdAttributeDotReplacement);
        }

        public static string CreateSanitizedId(string originalId, string invalidCharReplacement)
        {
            if (String.IsNullOrEmpty(originalId))
            {
                return null;
            }

            if (invalidCharReplacement == null)
            {
                throw new ArgumentNullException("invalidCharReplacement");
            }

            char firstChar = originalId[0];
            if (!Html401IdUtil.IsLetter(firstChar))
            {
                // the first character must be a letter
                return null;
            }

            StringBuilder sb = new StringBuilder(originalId.Length);
            sb.Append(firstChar);

            for (int i = 1; i < originalId.Length; i++)
            {
                char thisChar = originalId[i];
                if (Html401IdUtil.IsValidIdCharacter(thisChar))
                {
                    sb.Append(thisChar);
                }
                else
                {
                    sb.Append(invalidCharReplacement);
                }
            }

            return sb.ToString();
        }

        public void GenerateId(string name)
        {
            if (!Attributes.ContainsKey("id"))
            {
                string sanitizedId = CreateSanitizedId(name, IdAttributeDotReplacement);
                if (!String.IsNullOrEmpty(sanitizedId))
                {
                    Attributes["id"] = sanitizedId;
                }
            }
        }

        private void AppendAttributes(StringBuilder sb)
        {
            foreach (var attribute in Attributes)
            {
                string key = attribute.Key;
                if (String.Equals(key, "id", StringComparison.Ordinal /* case-sensitive */) && String.IsNullOrEmpty(attribute.Value))
                {
                    continue; // DevDiv Bugs #227595: don't output empty IDs
                }
                string value = HttpUtility.HtmlAttributeEncode(attribute.Value);
                sb.Append(' ')
                  .Append(key)
                  .Append("=\"")
                  .Append(value)
                  .Append('"');
            }
        }
#if !SyncfusionFramework3_5
        public void MergeAttribute(string key, string value)
        {
            MergeAttribute(key, value, replaceExisting: false);
        }
#else
        public void MergeAttribute(string key, string value)
        {
            MergeAttribute(key, value, false);
        }
#endif 
        public void MergeAttribute(string key, string value, bool replaceExisting)
        {
            if (String.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Value cannot be null or an empty string.");
            }

            if (replaceExisting || !Attributes.ContainsKey(key))
            {
                Attributes[key] = value;
            }
        }
#if !SyncfusionFramework3_5
        public void MergeAttributes<TKey, TValue>(IDictionary<TKey, TValue> attributes)
        {
            MergeAttributes(attributes, replaceExisting: false);
        }
#else
        public void MergeAttributes<TKey, TValue>(IDictionary<TKey, TValue> attributes)
        {
            MergeAttributes(attributes, false);
        }
#endif 
        public void MergeAttributes<TKey, TValue>(IDictionary<TKey, TValue> attributes, bool replaceExisting)
        {
            if (attributes != null)
            {
                foreach (var entry in attributes)
                {
                    string key = Convert.ToString(entry.Key, CultureInfo.InvariantCulture);
                    string value = Convert.ToString(entry.Value, CultureInfo.InvariantCulture);
                    MergeAttribute(key, value, replaceExisting);
                }
            }
        }

        public void SetInnerText(string innerText)
        {
            InnerHtml = HttpUtility.HtmlEncode(innerText);
        }

        internal
#if SyncfusionDNX5_0
            HtmlString
#else
#if !SyncfusionFramework3_5
            HtmlString
#else
            String
#endif 
#endif
            ToHtmlString(TagRenderMode renderMode)
        {
#if !SyncfusionFramework3_5
            return new Microsoft.AspNetCore.Html.HtmlString(ToString(renderMode));
#else
            return ToString(renderMode);
#endif 
        }

        public override string ToString()
        {
            return ToString(TagRenderMode.Normal);
        }

        public string ToString(TagRenderMode renderMode)
        {
            StringBuilder sb = new StringBuilder();
            switch (renderMode)
            {
                case TagRenderMode.StartTag:
                    sb.Append('<')
                      .Append(TagName);
                    AppendAttributes(sb);
                    sb.Append('>');
                    break;
                case TagRenderMode.EndTag:
                    sb.Append("</")
                      .Append(TagName)
                      .Append('>');
                    break;
                case TagRenderMode.SelfClosing:
                    sb.Append('<')
                      .Append(TagName);
                    AppendAttributes(sb);
                    sb.Append(" />");
                    break;
                default:
                    sb.Append('<')
                      .Append(TagName);
                    AppendAttributes(sb);
                    sb.Append('>')
                      .Append(InnerHtml)
                      .Append("</")
                      .Append(TagName)
                      .Append('>');
                    break;
            }
            return sb.ToString();
        }

        // Valid IDs are defined in http://www.w3.org/TR/html401/types.html#type-id
        private static class Html401IdUtil
        {
            private static bool IsAllowableSpecialCharacter(char c)
            {
                switch (c)
                {
                    case '-':
                    case '_':
                    case ':':
                        // note that we're specifically excluding the '.' character
                        return true;

                    default:
                        return false;
                }
            }

            private static bool IsDigit(char c)
            {
                return ('0' <= c && c <= '9');
            }

            public static bool IsLetter(char c)
            {
                return (('A' <= c && c <= 'Z') || ('a' <= c && c <= 'z'));
            }

            public static bool IsValidIdCharacter(char c)
            {
                return (IsLetter(c) || IsDigit(c) || IsAllowableSpecialCharacter(c));
            }
        }
    }

    public class HtmlIDHelper
    {

        private static string _idAttributeDotReplacement;
        internal HtmlIDHelper()
        {
        }

        // This property got copied from MVC's HtmlHelper along with TagBuilder.
        // It was a global property in MVC so it should not have scoped semantics here either.
        public static string IdAttributeDotReplacement
        {
            get
            {
                if (String.IsNullOrEmpty(_idAttributeDotReplacement))
                {
                    _idAttributeDotReplacement = "_";
                }
                return _idAttributeDotReplacement;
            }
            set { _idAttributeDotReplacement = value; }
        }

    }

}
