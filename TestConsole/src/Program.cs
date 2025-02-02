﻿// --------------------------------------------------------------------------
// <copyright file="Program.cs" company="-">
// Copyright (c) 2008-2017 Eser Ozvataf (eser@ozvataf.com). All rights reserved.
// Web: http://eser.ozvataf.com/ GitHub: http://github.com/eserozvataf
// </copyright>
// <author>Eser Ozvataf (eser@ozvataf.com)</author>
// --------------------------------------------------------------------------

//// This program is free software: you can redistribute it and/or modify
//// it under the terms of the GNU General Public License as published by
//// the Free Software Foundation, either version 3 of the License, or
//// (at your option) any later version.
////
//// This program is distributed in the hope that it will be useful,
//// but WITHOUT ANY WARRANTY; without even the implied warranty of
//// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//// GNU General Public License for more details.
////
//// You should have received a copy of the GNU General Public License
//// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Tassle.Logging.Telnet;
using Tassle.Telnet;
using System.Net;
using Tassle.Functions;
using System.Threading.Tasks;

namespace Tassle.TestConsole {
    public class Program {
        public static void Main(string[] args) {
            //TassleCoreConfig tassleConfig = new TassleCoreConfig() {
            //    Culture = "en-us"
            //};

            //TassleCore tassle = new TassleCore(tassleConfig);


            //tassle.Start();

            /*
            DynamicAssembly da = new DynamicAssembly("Deneme");

            var dc = da.AddClass("Eser");

            var df = dc.AddField("a", typeof(int), FieldAttributes.Private);
            var dp = df.ConvertToProperty(dc, "A");

            dc.Finalize();

            da.Save();

            Console.WriteLine("done");
            */

            // initialize bootstrapper
            var bootstrapper = new Bootstrapper();
            var serviceProvider = bootstrapper.GetServiceProvider();

            // // start telnet
            // var telnetServer = serviceProvider.GetService<ITelnetServer>();
            // telnetServer.Start();

            // // setup console logging
            // var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            // loggerFactory
            //     .AddConsole(LogLevel.Debug)
            //     .AddDebug()
            //     .AddTelnet(serviceProvider);

            var fm = new FunctionManager(serviceProvider);
            fm.RunAsync(
                new TestFunction(),
                new Request<int>() { Value = 5 });

            Console.Read();
        }
    }
}
