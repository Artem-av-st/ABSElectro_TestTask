using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace ABSElectro_TestTask
{
    internal class FindFilesCommand
    {
        public static void Register(CommandLineApplication app)
        {
            app.Command("find", Setup);
        }

        private static void Setup(CommandLineApplication command)
        {
            command.HelpOption("-?|-h|--help");
            command.OptionHelp.Description = "Показать информацию о параметрах комманды";
            command.Description = "Поиск файлов в текущем каталоге";

            #region options
            var recursivelyOption = command.Option(
                "-r|--recursive",
                "Выполнять поиск во вложенных папках.",
                CommandOptionType.NoValue);

            var sortOrderOption = command.Option(
               "-s|--sort <order>",
               "Порядок сортировки. t - по времени последнего обновления, n - по имени, e - по расширению, s - по размеру файла. " +
               "+ после способа сортировки - по возрастунию, - по убыванию.",
               CommandOptionType.SingleValue);
            
            var regexOption = command.Option(
               "-e|--rgx <pattern>",
               "Поиск файлов, текст в которых соответствуюет регулярному выражению.",
               CommandOptionType.SingleValue);

            var searchTextOption = command.Option(
               "-t|--text <searchString>",
               "Поиск файлов, которые содержат заданную строку.",
               CommandOptionType.SingleValue);

            var searchPattern = command.Argument(
                "",
                "Шаблон поиска имени файла *.*",
                true);
            #endregion options
            
            command.OnExecuteCommonCatch(
                () =>
                {
                    #region Check options
                    string regexPattern = String.Empty;
                    if (regexOption.HasValue() &&  !String.IsNullOrWhiteSpace(regexOption.Value()))
                    {
                        regexPattern = regexOption.Value();
                    }

                    string searchString = String.Empty;
                    if(searchTextOption.HasValue() && !String.IsNullOrWhiteSpace(searchTextOption.Value()))
                    {
                        searchString = searchTextOption.Value();
                    }

                    if(searchString.Any() && regexPattern.Any())
                    {
                        throw new CommandParsingException(command, "Команда не может одновременно содержать --text и --rgx опции");
                    }

                    IEnumerable<string> fileNamePatterns = new List<string>();
                    if(searchPattern.Values.Any())
                    {
                        fileNamePatterns = searchPattern.Values.Where(pattern => !String.IsNullOrWhiteSpace(pattern));
                    }

                    var recursively = false;
                    if (recursivelyOption.HasValue())
                    {
                        recursively = true;
                    }

                    IEnumerable<SortPattern> sortPatterns = new List<SortPattern>();
                    if(sortOrderOption.HasValue() && !String.IsNullOrWhiteSpace(sortOrderOption.Value()))
                    {
                        sortPatterns = ParseSortPatterns(sortOrderOption.Value());
                    }
                                        
                    #endregion Check options

                    var logger = new ConsoleLogger();
                   
                    var fileProcessor = new FileProcessor(logger);

                    var cancellationTokenSource = new CancellationTokenSource();
                    var token = cancellationTokenSource.Token;

                    var task = Task.Run(() => fileProcessor.FindFiles(
                        fileNamePatterns: fileNamePatterns,
                        recursievly: recursively,
                        regularExpression: regexPattern,
                        searchString: searchString,
                        sortPatterns: sortPatterns,
                        token: token));
                    Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
                    {
                        e.Cancel = true;
                        cancellationTokenSource.Cancel();
                    };
                    logger.Message("Нажмите CTRL+C для прекращения работы приложения");
                    do
                    {
                        
                    } while (!token.IsCancellationRequested && !task.IsCompleted);


                    Console.WriteLine("Для выхода нажмите любую клавишу...");
                    Console.ReadKey();
                    return 1;
                });
        }

        /// <summary>
        /// Parses input string by sort patterns. If patterns are not found then returns empty collection.
        /// </summary>        
        private static IEnumerable<SortPattern> ParseSortPatterns(string inputString)
        {
            var regex = new System.Text.RegularExpressions.Regex("[stne][+-]{0,}");
            var patterns = regex.Matches(inputString);

            List<SortPattern> sortPatterns = new List<SortPattern>();

            foreach (var pat in patterns)
            {
                var pattern = pat.ToString();

                SortPattern.SortPatternType patternType;
                switch (pattern[0].ToString().ToUpper())
                {
                    case "T": patternType = SortPattern.SortPatternType.Time; break;
                    case "S": patternType = SortPattern.SortPatternType.Size; break;
                    case "N": patternType = SortPattern.SortPatternType.Name; break;
                    case "E": patternType = SortPattern.SortPatternType.Name; break;
                    default: continue;
                }

                var isDesc = false;
                if(pattern.Length == 2)
                {
                    if(pattern[1] == '-')
                    {
                        isDesc = true;
                    }
                }

                yield return new SortPattern(patternType, isDesc);
            }
        }
    }
}

