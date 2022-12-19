using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using dnf;

namespace dnf;
internal class Program
{
    /// <summary>
    /// 从配置文件中读取到的键值对
    /// </summary>
    private static Dictionary<string, string> _configDic;

    static void Main(string[] args)
    {
    start:
        int l = args.Length;
        if (!GetTempalteDirPath())
        {
            Console.WriteLine("获取模板配置路径失败！");
            return;
        }
        switch (l)
        {
            case 3:
                ThreeArgsState(args);
                return;
            case 2:
                TwoArgsState(args);
                return;
            case 1:
                OneArgState(args);
                return;
            default:
                Console.WriteLine("参数数目错误!请重新输入参数,输入 help查看帮助");
                string arg = Console.ReadLine();
                args = arg.Split(" ");
                goto start;
        }
    }

    static void ThreeArgsState(string[] args)
    {
        switch (args[0].GetCmdEnum())
        {
            case CmdEnum.NEW:
                NewProj(args[1], args[2]);
                break;
            case CmdEnum.NEWSLN:
                NewSolution(args[1], args[2]);
                break;
            case CmdEnum.DEBUGTYPE:
                SetDebugType(args[2], args[1]);
                break;
            case CmdEnum.SHOWPROJ:
                ShowSlnProjectInVscode(args[1], args[2]);
                break;
            case CmdEnum.SHOWREFPROJ:
                ShowRefProjecInVscode(args[1], args[2]);
                break;
            default:
                Console.WriteLine("{0}是未定义的命令", args[0]);
                return;
        }
    }

    static void TwoArgsState(string[] args)
    {
        switch (args[0].GetCmdEnum())
        {
            case CmdEnum.DEBUGTYPE:
                SetDebugType(args[1]);
                break;
            case CmdEnum.SHOWPROJ:
                ShowSlnProjectInVscode(args[1]);
                break;
            case CmdEnum.SHOWREFPROJ:
                ShowRefProjecInVscode(args[1]);
                break;
            case CmdEnum.NEWCS:
                NewCSFile(args[1]);
                break;
            default:
                break;
        }
    }
    static void OneArgState(string[] args)
    {
        switch (args[0].GetCmdEnum())
        {
            case CmdEnum.CODEPROJ:
                UpadateAllCsPrj();
                break;
            case CmdEnum.HELP:
                PrintHelpDoc();
                break;
            default:
                Console.WriteLine("{0}是未定义的命令", args[0]);
                return;
        }

    }

    static void ShowSlnProjectInVscode(string slnPath, string dirPath = "")
    {
        string currentDir = System.Environment.CurrentDirectory + '\\';
        string vscodeSetting = @$"{currentDir}\.vscode\settings.json";
        slnPath = Path.Combine(currentDir, slnPath);
        var resolve = new SlnResolve(slnPath);
        var includes = resolve.GetAllProjectPath().ToHashSet();
        dirPath = Path.Combine(currentDir, dirPath);
        var dirs = Directory.GetDirectories(dirPath);
        var list = new List<string>();
        foreach (var dir in dirs)
        {

            if (includes.Contains(dir) == false)
            {
                Uri url = new Uri(currentDir);
                Uri relativeUrl = url.MakeRelativeUri(new Uri(dir));
                string str = $"\"{relativeUrl.OriginalString}\":true,";
                Console.WriteLine(str);
            }

        }
    }

    static void ShowRefProjecInVscode(string projPath, string dirPath = "")
    {
        string currentDir = System.Environment.CurrentDirectory + '\\';
        string vscodeSetting = @$"{currentDir}\.vscode\settings.json";
        projPath = Path.Combine(currentDir, projPath);
        var resolve = new CsProjResolve(projPath);
        var includes = resolve.GetAllProjectPath().ToHashSet();
        dirPath = Path.Combine(currentDir, dirPath);
        var dirs = Directory.GetDirectories(dirPath);
        var list = new List<string>();
        foreach (var dir in dirs)
        {

            if (includes.Contains(dir) == false)
            {
                Uri url = new Uri(currentDir);
                Uri relativeUrl = url.MakeRelativeUri(new Uri(dir));
                string str = $"\"{relativeUrl.OriginalString}\":true,";
                Console.WriteLine(str);
            }

        }
    }


    static void PrintHelpDoc()
    {
        foreach (CmdEnum attr in Enum.GetValues<CmdEnum>())
        {
            Console.WriteLine("命令:\"{0}\" {1}", attr.GetCmdEnumString(), attr.GetCmdEnumDespration());
        }
    }
    static void UpadateAllCsPrj()
    {
        string currentDir = System.Environment.CurrentDirectory + '\\';
        SetAllFiles(currentDir, (filePath) =>
        {
            string fileEx = Path.GetExtension(filePath).ToLower();
            if (fileEx == ".csproj")
            {
                ExMethod.UpadateCsProj(filePath);
            }
        }, (dir) => { });
    }
    /// <summary>
    /// New CSharp File Method
    /// </summary>
    /// <param name="csFileName">newfile's name</param>
    /// <param name="tempName">templatefile's name</param>
    static void NewCSFile(string csFileName, string tempName = "Class.cs")
    {
        //控制台的当前工作路径
        string currentDir = System.Environment.CurrentDirectory + '\\';
        string tempPath = _configDic?["templatePath"] + tempName;
        if (!File.Exists(tempPath))
        {
            Console.WriteLine("{0}下不存在指定的模板目录", _configDic?["templatePath"]);
            return;
        }
        string filePath = currentDir + csFileName;
        if (Directory.Exists(filePath))
        {
            Console.WriteLine("当前路径下已经存在指定的项目文件夹,请指定其他名称");
            return;
        }
        string dirPath = currentDir;
        string[] dirs = csFileName.Split('\\', '/');
        for (int i = 0; i < dirs.Length - 1; i++)
        {
            dirPath = string.Format("{0}/{1}", dirPath, dirs[i]);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            else
            {
                var files = Directory.GetFiles(dirPath);
                foreach (var file in files)
                {
                    if (Path.GetExtension(file).ToLower() == ".csproj")
                    {
                        var fileLines = File.ReadAllLines(file, Encoding.Default).ToList();
                        int insertCount = 0;
                        foreach (var line in fileLines)
                        {
                            if (line.Contains("<ItemGroup>") && fileLines[insertCount + 1].Contains("<Compile"))
                            {
                                break;
                            }
                            insertCount++;
                        }
                        string insertStr = string.Format("\t\t<Compile Include=\"{0}\" />", filePath.Remove(0, dirPath.Length + 1));
                        fileLines.Insert(insertCount, insertStr);
                        File.WriteAllLines(filePath + ".cs", fileLines, Encoding.UTF8);
                    }
                }
            }
        }
        string name = dirs.Last();
        string ns = csFileName.Replace('\\', '.').Replace('/', '.').Remove(csFileName.Length - name.Length, name.Length);//名称空间，暂定如此
        string str = File.ReadAllText(tempPath);
        str = str.Replace("$safeitemrootname$", name).Replace("$rootnamespace$", ns);
        File.WriteAllText(filePath, str);
    }
    /// <summary>
    /// 新建解决方案
    /// </summary>
    /// <param name="tempName">解决方案模板名</param>
    /// <param name="slnName">新的解决方案名称</param>
    static void NewSolution(string tempName, string slnName)
    {
        //控制台的当前工作路径
        string currentDir = System.Environment.CurrentDirectory + '\\';
        string tempPath = string.Format("{0}{1}\\{2}", _configDic?["templatePath"], _configDic?["slnFolderName"], tempName);
        if (!Directory.Exists(tempPath))
        {
            Console.WriteLine("{0}下不存在指定的模板目录", _configDic?["templatePath"]);
            return;
        }
        string projPath = currentDir;
        try
        {
            SetAllFiles(tempPath, (filePath) =>
            {
                var fileInfo = new FileInfo(filePath);
                var newFile = filePath.Replace(tempPath, projPath);
                //修改项目文件的名称
                if (fileInfo.Extension.ToLower() == ".sln")
                {
                    newFile = string.Format("{0}\\{1}.sln", projPath, slnName);
                }

                var str = File.ReadAllBytes(filePath);
                File.WriteAllBytes(newFile, str);

            }, (dirPath) =>
            {
                var newDir = dirPath.Replace(tempPath, projPath);
                Directory.CreateDirectory(newDir);
            });
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.Source);
            Console.WriteLine(ex.StackTrace);
        }
        Console.WriteLine("{0}创建成功！", projPath);
    }
    /// <summary>
    /// 根据指定的模板创建在控制台的当前路径下创建新的项目
    /// </summary>
    /// <param name="tempName">摸吧名称</param>
    /// <param name="projName">新项目的名称</param>
    static void NewProj(string tempName, string projName)
    {
        //控制台的当前工作路径
        string currentDir = System.Environment.CurrentDirectory + '\\';
        string tempPath = string.Format("{0}{1}\\{2}", _configDic?["templatePath"], _configDic?["projFolderName"], tempName);
        if (!Directory.Exists(tempPath))
        {
            Console.WriteLine("{0}下不存在", tempPath);
            return;
        }
        string projPath = currentDir + projName;
        if (Directory.Exists(projPath))
        {
            Console.WriteLine("当前路径下已经存在指定的项目文件夹,请指定其他名称");
            return;
        }
        try
        {
            Directory.CreateDirectory(projPath);
            if (!Directory.Exists(projPath))
            {
                Console.WriteLine("项目创建失败！");
                return;
            }
            var guid = Guid.NewGuid().ToString().ToUpper();//生成项目的guid
            //修改项目文件
            SetAllFiles(tempPath, (filePath) =>
            {
                var fileInfo = new FileInfo(filePath);
                var newFile = filePath.Replace(tempPath, projPath);
                //修改项目文件的名称
                var extension = fileInfo.Extension.ToLower();
                if (extension == ".csproj")
                {
                    newFile = string.Format("{0}\\{1}.csproj", projPath, projName);
                }
                if (extension == ".csproj" || extension == ".cs" || extension == ".xaml")
                {
                    string str = File.ReadAllText(filePath);
                    str = str.Replace("$safeprojectname$", projName).Replace("$guid1$", guid);
                    File.WriteAllText(newFile, str);
                }
                else
                {
                    var bt = File.ReadAllBytes(filePath);
                    File.WriteAllBytes(newFile, bt);
                }

            }, (dirPath) =>
            {
                var newDir = dirPath.Replace(tempPath, projPath);
                Directory.CreateDirectory(newDir);
            });
            //将项目文件添加到解决方案中
            var filePaths = Directory.GetFiles(currentDir);
            foreach (var filePath in filePaths)
            {

                if (Path.GetExtension(filePath).ToLower() == ".sln")
                {
                    var cmd = string.Format("dotnet sln \"{0}\" add \"{1}\"", filePath, projPath);
                    CmdFunc.RunShellCommand(cmd);
                }
            }
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.Source);
            Console.WriteLine(ex.StackTrace);
        }
        Console.WriteLine("{0}创建成功！", projPath);
    }

    static void SetDebugType(string pdbValue, string configName = "")
    {
        //控制台的当前工作路径
        string currentDir = System.Environment.CurrentDirectory + '\\';
        SetAllFiles(currentDir, file =>
        {
            if (Path.GetExtension(file).ToLower() == ".csproj")
            {
                bool IsChanged = false;
                ExMethod.UpadateCsProj(file, (doc, node) =>
                {
                    if (IsChanged)
                        return;

                    switch (node.Name)
                    {
                        case "PropertyGroup":
                            if (configName == "")
                            {
                                doc.SetNodeValue("DebugType", pdbValue, node);
                                IsChanged = true;
                            }
                            else
                            {
                                var attr = node.Attributes?["Condition"];
                                if (node["Configuration"] == null && attr != null && attr.Value.Contains(configName))
                                {
                                    doc.SetNodeValue("DebugType", pdbValue, node);
                                    IsChanged = true;
                                }

                            }
                            break;
                        default:
                            break;
                    }
                });
            }
        }, dir => { });
    }
    /// <summary>
    /// 对指定文件夹下的所有文件执行一项操作
    /// </summary>
    /// <param name="dir">指定文件夹的路径</param>
    /// <param name="fileAction">以文件名为参数的Action</param>
    /// <param name="dirAction">以文件夹为参数的Action</param>
    static void SetAllFiles(string dir, Action<string> fileAction, Action<string> dirAction)
    {
        var filePaths = Directory.GetFiles(dir);
        foreach (var filePath in filePaths)
        {
            fileAction(filePath);
        }
        var dirs = Directory.GetDirectories(dir);
        foreach (var itemDir in dirs)
        {
            dirAction(itemDir);
            SetAllFiles(itemDir, fileAction, dirAction);
        }
    }
    /// <summary>
    /// 从配置文件中获取项目模板的路径
    /// </summary>
    /// <returns></returns>
    static bool GetTempalteDirPath()
    {
        //配置文件的路径
        var configPath = AppDomain.CurrentDomain.BaseDirectory + "config.json";
        if (!File.Exists(configPath))
        {
            Console.WriteLine("{0}不存在，请检查！", configPath);
            return false;
        }
        var fileLines = File.ReadAllLines(configPath, Encoding.Default).ToList();
        _configDic = new Dictionary<string, string>(fileLines.Count);
        Regex re = new("(?<=\").*?(?=\")", RegexOptions.None);
        foreach (var line in fileLines)
        {
            MatchCollection mc = re.Matches(line);
            if (mc.Count > 2 && !_configDic.ContainsKey(mc[0].Value))
            {
                _configDic.Add(mc[0].Value, mc[2].Value);
            }
        }
        if (Directory.Exists(_configDic?["templatePath"]))
            return true;
        Console.WriteLine($"{_configDic?["templatePath"]}不存在，请检查！");
        return false;
    }
}
