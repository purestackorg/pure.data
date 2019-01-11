using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pure.Data.Gen.CodeServer.RazorPaser
{
    public class RazorCustomizedTemplate<T> : TemplateBase<T>
    {
        public new T Model
        {
            get { return base.Model; }
            set { base.Model = value; }
        }
        public RazorHtmlHelper Html { get; set; }
        public RazorCustomizedTemplate()
        {
            Html = new RazorHtmlHelper();
        }
    }
}
