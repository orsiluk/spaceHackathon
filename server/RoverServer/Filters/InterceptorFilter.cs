using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace RoverServer.Filters
{
    public class InterceptorFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext context)
        {
            return;
            Debug.WriteLine(context.Request);
            using (var contentStream = context.Request.Content.ReadAsStreamAsync().Result)
            {
                contentStream.Seek(0, SeekOrigin.Begin);
                using (var sr = new StreamReader(contentStream))
                {
                    string rawContent = sr.ReadToEnd();
                    Debug.WriteLine(rawContent);
                }
            }
        }
    }
}