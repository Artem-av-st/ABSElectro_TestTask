using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSElectro_TestTask
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandLineApplication(throwOnUnexpectedArg: false);

            app.HelpOption("-?|-h |--help");
            app.OptionHelp.Description = "Показать информацию о доступных коммандах";
            
            app.OnExecute(() =>
            {
                app.ShowHelp();
                return 1;
            });

            FindFilesCommand.Register(app);     
            
            try
            {
                app.Execute(args);                                
            }
            catch (CommandParsingException ex)
            {
                Console.WriteLine(ex.Message);
                app.ShowHelp();
            }
        }
    }
}
