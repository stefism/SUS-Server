using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using SUS.MvcFramework.ViewEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SUS.MvcFramework
{
    public class SusViewEngine : IViewEngine
    {
        public string GetHtml(string templateCode, object viewModel)
        {
            string csharpCode = GenerateCSharpFromTemplate(templateCode);
            IView executableObject = GenerateExecutableCode(csharpCode, viewModel);
            string html = executableObject.ExecuteTemplate(viewModel);
            return html;
        }

        private string GenerateCSharpFromTemplate(string templateCode)
        {
            //string methodBody = GetMethodBody(templateCode);
            string scharpCode = @"
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using SUS.MvcFramework.ViewEngine;

namespace ViewNamespace
{
    public class ViewClass : IView
    {
        public string ExecuteTemplate(object viewModel)
        {
            var html = new StringBuilder();
            
            " + GetMethodBody(templateCode) + @"

            return html.ToString();
        }        
    }
}
";
            return scharpCode;
        }

        private string GetMethodBody(string templateCode)
        {
            StringBuilder csharpCode = new StringBuilder();
            StringReader sr = new StringReader(templateCode);
            
            string line;

            while ((line = sr.ReadLine()) != null)
            {
                csharpCode .AppendLine($"html.AppendLine(@\"{line.Replace("\"", "\"\"")}\");");
            }

            return csharpCode.ToString();
        }

        private IView GenerateExecutableCode(string csharpCode, object viewModel)
        {
            var compileResult = CSharpCompilation.Create("ViewAssembly")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(typeof(IView).Assembly.Location));

            if (viewModel != null)
            {
                compileResult = compileResult
                    .AddReferences(MetadataReference
                    .CreateFromFile(viewModel.GetType().Assembly.Location));
            }

            AssemblyName[] libraries = Assembly.Load(new AssemblyName("netstandard"))
                .GetReferencedAssemblies();

            foreach (var library in libraries)
            {
                compileResult = compileResult
                    .AddReferences(MetadataReference
                    .CreateFromFile(Assembly.Load(library).Location));
            }

            compileResult = compileResult
                .AddSyntaxTrees(SyntaxFactory.ParseSyntaxTree(csharpCode));

            using(MemoryStream memoryStream = new MemoryStream())
            {               
                EmitResult result = compileResult.Emit(memoryStream);

                if (!result.Success)
                {
                    return new ErrorView(result.Diagnostics
                        .Where(x => x.Severity == DiagnosticSeverity.Error)
                        .Select(x => x.GetMessage()), csharpCode);
                    ;
                }

                try
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    //The stream works like audio cassette. We record to the any point and then if we want to listen the record, we have to go back to the beginning. Otherwise, we start reading from where the stream has reached. Therefore, with the above line we return to the beginning of the stream.

                    byte[] byteAssembly = memoryStream.ToArray();
                    Assembly assembly = Assembly.Load(byteAssembly);
                    Type viewType = assembly.GetType("ViewNamespace.ViewClass");
                    object instance = Activator.CreateInstance(viewType);
                    return instance as IView;
                }
                catch (Exception ex)
                {
                    return new ErrorView(new List<string> 
                    { ex.ToString()}, csharpCode);
                }               
            }           
        }
    }
}
