using System.Text.RegularExpressions;

namespace dnf;

public class SlnResolve
{

    private string _slnPath;

    private string _dirPath;
    public SlnResolve(string slnPath)
    {
        this._slnPath=slnPath;
        this._dirPath = Path.GetDirectoryName(this._slnPath);
    }

    public IEnumerable<string> GetAllProjectPath()
    {

        var lines=File.ReadAllLines(_slnPath);
        if(lines is null)
        {
            yield break;
        }
        string matchStr="^Project.*";
        foreach(var line in lines)
        {
            if(Regex.IsMatch(line,matchStr))
            {
               var ss=line.Split(',');
               var s=ss[1].TrimStart().TrimStart('"').Split('/','\\');
                var projPath = Path.Combine(this._dirPath,s[0]);
                projPath = Path.GetFullPath(projPath);
                yield return projPath;
            }
        }

    }



}
