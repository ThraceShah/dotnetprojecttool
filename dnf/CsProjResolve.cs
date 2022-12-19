using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;


namespace dnf;

public class CsProjResolve
{
    private string _projPath;

    private string _dirPath;
    public CsProjResolve(string projPath)
    {
        this._projPath = projPath;
        this._dirPath = Path.GetDirectoryName(projPath);
    }

    public IEnumerable<string> GetAllProjectPath()
    {

        var lines = File.ReadAllLines(_projPath);
        yield return this._dirPath;
        if (lines is null)
        {
            yield break;
        }
        foreach (var line in lines)
        {
            var l = line.TrimStart();
            if (l.StartsWith("<ProjectReference"))
            {
                var p = "Include=\"(.*)\"";
                string relativePath = Regex.Match(l, p).Result("$1")+"/..";
                var projPath = Path.Combine(this._dirPath, relativePath);
                projPath = Path.GetFullPath(projPath);
                yield return projPath;
            }
        }

    }

}
