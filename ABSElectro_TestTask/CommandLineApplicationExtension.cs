using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSElectro_TestTask
{
    static class CommandLineApplicationExtension
    {
        /// <summary>
        /// A wrapper that catches a CommandParsingException thrown by the handler as a sign 
        /// of the incorrect command arguments, and displays help for the command. 
        /// It also catches exceptions from the command and displays them to the console before shutting down.
        /// </summary>        
        public static void OnExecuteCommonCatch(
            this CommandLineApplication command,
            Func<int> invoke)
        {
            command.OnExecute(
                () =>
                {
                    try
                    {
                        return invoke();
                    }
                    catch (CommandParsingException ex)
                    {
                        System.Console.WriteLine(ex.Message);
                        command.ShowHelp();
                        return -1;
                    }
                    catch (Exception ex)
                    {
                        command.PromtError(ex.ToString());
                        return -1;
                    }
                });
        }       

        public static void PromtError(
            this CommandLineApplication command,
            string text)
        {
            System.Console.BackgroundColor = ConsoleColor.Red;
            System.Console.ForegroundColor = ConsoleColor.White;
            System.Console.WriteLine(text);
            System.Console.ResetColor();
        }
    }
}
