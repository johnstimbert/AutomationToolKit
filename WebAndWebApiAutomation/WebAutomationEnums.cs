using System.ComponentModel;

namespace WebAndWebApiAutomation
{
    public class WebAutomationEnums
    {
        public enum HtmlAttributeType
        {
            AttributeText_Contains,
            AttributeText_ExactMatch,
            Id,
            Class,
            Name,
            Type,
            Href,
            Src,
            Title,
            InnerText_Contains,
            InnerText_ExactMatch,
            FormControlName,
            None
        }

        public enum HtmlTagType
        {
            #region tags
            a,
            acronym,
            address,
            area,
            b,
            basefont,
            bdo,
            bgsound,
            big,
            blockquote,
            body,
            br,
            button,
            caption,
            center,
            cite,
            code,
            col,
            colgroup,
            dd,
            del,
            dfn,
            dir,
            div,
            dl,
            dt,
            em,
            embed,
            fieldset,
            font,
            form,
            frame,
            frameset,
            h1,
            h2,
            h3,
            h4,
            h5,
            h6,
            head,
            hr,
            html,
            i,
            iframe,
            img,
            input,
            ins,
            isindex,
            kbd,
            label,
            legend,
            li,
            link,
            map,
            marquee,
            menu,
            meta,
            nobr,
            noframes,
            noscript,
            ol,
            option,
            p,
            param,
            pre,
            q,
            rt,
            ruby,
            s,
            samp,
            script,
            select,
            small,
            span,
            strike,
            strong,
            style,
            sub,
            sup,
            svg,
            table,
            tbody,
            td,
            textarea,
            tfoot,
            th,
            thead,
            title,
            tr,
            tt,
            u,
            ul,
            the,
            var,
            wbr,
            xml
            #endregion
        }

        public enum DriverType
        {
            Chrome,
            Firefox,
            Ie,
            Edge
        }

        public enum NavigationResult
        {
            /// <summary>
            /// Navigation was successful
            /// </summary>
            Success,
            /// <summary>
            /// No href attribute was found
            /// </summary>
            HrefIsNotPresent,
            /// <summary>
            /// Navigation failed see message
            /// </summary>
            Failed,
            /// <summary>
            /// No navigation was attempted
            /// </summary>
            NavigationNotAttempted
        }

    }
}
