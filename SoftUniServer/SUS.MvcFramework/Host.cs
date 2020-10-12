using SUS.HTTP;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SUS.MvcFramework
{
    public static class Host
    {
        public static async Task CreateHostAsync
            (IMvcApplication application, int port = 80)
        {
            List<Route> routeTable = new List<Route>();

            string[] staticFiles = Directory.GetFiles("wwwroot", "*", SearchOption.AllDirectories);

            foreach (var staticFile in staticFiles)
            {
                var url = staticFile.Replace("wwwroot", string.Empty)
                    .Replace("\\", "/");

                routeTable.Add(new Route(url, HttpMethod.Get, (request) => 
                {
                    byte[] fileContent = File.ReadAllBytes(staticFile);
                    string fileExt = new FileInfo(staticFile).Extension;
                    
                    string contentType = fileExt switch
                    {
                        ".txt" => "text/plain",
                        ".js" => "text/javascript",
                        ".css" => "text/css",
                        ".jpg" => "image/jpg",
                        ".jpeg" => "image/jpg",                        
                        ".png" => "image/png",
                        ".gif" => "image/gif",
                        ".ico" => "image/vnd.microsoft.icon",
                        ".html" => "text/html",
                        _ => "text/plain",
                    };

                    return new HttpResponse(contentType, fileContent, HttpStatusCode.Ok);
                }));
            }

            application.ConfigureServices();
            application.Configure(routeTable);

            System.Console.WriteLine("All registered routes:");
            foreach (var route in routeTable)
            {
                System.Console.WriteLine($"{route.Method} -> {route.Path}");
            }

            IHttpServer server = new HttpServer(routeTable);
            
            //Process.Start(@"C:\Program Files\Opera\launcher.exe", "http://localhost");

            await server.StartAsync(port);
        }
    }
}
