﻿using SUS.HTTP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SUS.MvcFramework
{
    public static class Host
    {
        public static async Task CreateHostAsync
            (IMvcApplication application, int port = 80)
        {
            List<Route> routeTable = new List<Route>();
            
            AutoRegisterStaticFiles(routeTable);
            AutoRegisterRoutes(routeTable, application);

            application.ConfigureServices();
            application.Configure(routeTable);

            Console.WriteLine("All registered routes:");
            foreach (var route in routeTable)
            {
                Console.WriteLine($"{route.Method} -> {route.Path}");
            }

            IHttpServer server = new HttpServer(routeTable);

            //Process.Start(@"C:\Program Files\Opera\launcher.exe", "http://localhost");

            await server.StartAsync(port);
        }

        private static void AutoRegisterRoutes(List<Route> routeTable, IMvcApplication application)
        {
            var controllerTypes = application.GetType().Assembly.GetTypes()
                .Where(x => x.IsClass && !x.IsAbstract 
                && x.IsSubclassOf(typeof(Controller)));

            foreach (var controllerType in controllerTypes)
            {
                Console.WriteLine(controllerType.Name);
                var methods = controllerType.GetMethods()
                    .Where(x => x.IsPublic && !x.IsStatic
                    && x.DeclaringType == controllerType && !x.IsAbstract
                    && !x.IsConstructor && !x.IsSpecialName); //x.IsSpecialName -> this is properties get and set methods.

                foreach (var method in methods)
                {
                    string url = "/" + controllerType.Name.Replace("Controller", string.Empty) + "/" + method.Name;

                    BaseHttpAttribute attribute 
                        = method.GetCustomAttributes(false)
                        .Where(x => x.GetType()
                        .IsSubclassOf(typeof(BaseHttpAttribute)))
                        .FirstOrDefault() as BaseHttpAttribute;

                    HttpMethod httpMethod = HttpMethod.Get;

                    if (attribute != null)
                    {
                        httpMethod = attribute.Method;
                    }

                    if (!string.IsNullOrEmpty(attribute?.Url))
                    {
                        url = attribute.Url;
                    }

                    routeTable.Add(new Route(url, httpMethod, (request) =>
                    {
                        Controller instance = Activator
                        .CreateInstance(controllerType) as Controller;
                        instance.Request = request;

                        HttpResponse response = method
                        .Invoke(instance, new object[] {  }) as HttpResponse;
                        return response;
                    }));

                    Console.WriteLine(url);
                }
            }
        }

        private static void AutoRegisterStaticFiles(List<Route> routeTable)
        {
            string[] staticFiles = Directory.GetFiles("wwwroot", "*", SearchOption.AllDirectories);

            foreach (var staticFile in staticFiles)
            {
                string url = staticFile.Replace("wwwroot", string.Empty)
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
        }
    }
}
