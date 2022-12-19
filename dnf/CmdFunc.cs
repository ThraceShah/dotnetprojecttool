using System;
using System.Diagnostics;

namespace dnf;
public static class CmdFunc
{
    /// <summary>
    /// Runs a command with supplied araguments.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="arguments"></param>
    public static void RunShellCommand(String commandLine)
    {
        var cmd = new Process();
        cmd.StartInfo.FileName = "powershell.exe";
        cmd.StartInfo.RedirectStandardInput = true;
        cmd.StartInfo.RedirectStandardOutput = true;
        cmd.StartInfo.CreateNoWindow = true;
        cmd.StartInfo.UseShellExecute = false;
        cmd.Start();
        /* execute "dir" */

        cmd.StandardInput.WriteLine(commandLine);
        cmd.StandardInput.Flush();
        cmd.StandardInput.Close();
        Console.WriteLine(cmd.StandardOutput.ReadToEnd());
    }
}
