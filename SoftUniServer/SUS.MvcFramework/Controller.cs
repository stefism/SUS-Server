using SUS.HTTP;
using System.Runtime.CompilerServices;
using System.Text;

namespace SUS.MvcFramework
{
    public abstract class Controller
    //Абстрактните класове не могат да се инстанцират но могат да се наследяват.
    {
        private SusViewEngine viewEngine;

        public Controller()
        {
            viewEngine = new SusViewEngine();
        }

        public HttpRequest Request { get; set; }

        public HttpResponse View
            (object viewModel = null, 
            [CallerMemberName]string viewPath = null)
        {            
            var viewContent = System.IO.File
                .ReadAllText("Views/" + 
                GetType().Name.Replace("Controller", string.Empty) 
                + "/" + viewPath + ".cshtml");           
            viewContent = viewEngine.GetHtml(viewContent, viewModel);

            var responseHtml = PutViewInLayout(viewContent, viewModel);

            var responseBodyBytes = Encoding.UTF8.GetBytes(responseHtml);

            var response = new HttpResponse("text/html", responseBodyBytes);

            return response;
        }      

        public HttpResponse File(string filePath, string contentType)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            var responce = new HttpResponse(contentType, fileBytes);

            return responce;
        }

        public HttpResponse Redirect(string url)
        {
            var response = new HttpResponse(HttpStatusCode.Found);
            //HttpStatusCode.Found = 302. 
            //При 302: POST -> 302 -> GET
            //HttpStatusCode.TemporaryRedirect = 307
            //При 307: POST -> 307 -> POST

            response.Headers.Add(new Header("Location", url));
            return response;
        }

        public HttpResponse Error(string errorText)
        {
            var viewContent = $"<div class=\"alert alert-danger\" role=\"alert\">{errorText}</div>";          
            var responseHtml = PutViewInLayout(viewContent);
            var responseBodyBytes = Encoding.UTF8.GetBytes(responseHtml);
            var response = new HttpResponse("text/html", responseBodyBytes, HttpStatusCode.ServerError);
            return response;
        }

        private string PutViewInLayout(string viewContent, object viewModel = null)
        {
            var layout = System.IO.File.ReadAllText("Views/Shared/_Layout.cshtml");
            layout = layout.Replace("@RenderBody()", "__VIEW__");
            layout = viewEngine.GetHtml(layout, viewModel);
            var responseHtml = layout.Replace("__VIEW__", viewContent);
            return responseHtml;
        }
    }
}
