using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace ClientExpandableObjectRecogniser
{
    public class Program
    {
        public static void Main(string[] args)
        {
			var configurations = new ConfigurationBuilder()
				.AddCommandLine(args)
				.Build();
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
