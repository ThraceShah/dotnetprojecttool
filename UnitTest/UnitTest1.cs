using System;
using dnf;
using Xunit;

namespace UnitTest;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var codeproj=CmdEnum.CODEPROJ;
        Console.WriteLine(codeproj.GetCmdEnumString());
        Console.WriteLine(codeproj.GetCmdEnumDespration());
        var testEnum="newcs".GetCmdEnum();
        Assert.Equal(codeproj.GetCmdEnumString(),nameof(codeproj));
        Assert.Equal(testEnum,CmdEnum.NEWCS);
    }
    [Fact]
    public void TestName()
    {
        // Given
    
        // When
    
        // Then
        ExMethod.UpadateCsProj(@"F:\Debug\MyService.csproj");
    }
    [Fact]
    public void CmdFuncTest()
    {
        string cmd="dotnet sln c:/Users/xqy17/source/repos/MyCode/TestSln/TestSW.sln add c:/Users/xqy17/source/repos/MyCode/TestSln/TestSw/TestSw.csproj";
        CmdFunc.RunShellCommand(cmd);
    }

    [Fact]
    public void ReletivePathTest()
    {
        // Uri url = new Uri(currentDir);
        // Uri relativeUrl = url.MakeRelativeUri(new Uri(dir));
    }
}