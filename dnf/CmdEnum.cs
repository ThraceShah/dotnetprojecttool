
/*************************************************************************/
//功能描述：
//数据表：
//作者：熊清宇
//日期：
//参考文档：
//
//修改者：
//修改日期：
//修改内容：
/*************************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace dnf;
public enum CmdEnum
{
    [Description("将.net framwork项目文件更新为vs code可调试的格式")]
    CODEPROJ,
    [Description("输出可用命令及其解释")]
    HELP,
    [Description("新建项目,格式:new 模板名称 新建项目名称")]
    NEW,
    [Description("新建解决方案,格式:newsln 模板名称 新建解决方案名称")]
    NEWSLN,
    [Description("新建C#代码文件,格式:newcs 模板名称 新建代码文件名称")]
    NEWCS,
    [Description("修改当前路径下所有项目的DebugType 格式:debugtpe 配置名称(Debug,Release或自定义,缺省时修改全局) 修改后的值")]
    DEBUGTYPE,
    [Description("修改vscode设置文件,使文件树仅显示存在于指定的.sln文件中的文件夹,格式:showproj sln文件路径 查找的文件夹路径")]
    SHOWPROJ,
    [Description("修改vscode设置文件,使文件树仅显示存在于指定的.csproj文件中引用项目所在的文件夹,格式:showproj csproJ文件路径 查找的文件夹路径")]
    SHOWREFPROJ,

}
public static class CmdEnumEx
{
    public static string GetCmdEnumString(this CmdEnum e)
    {
        return e.ToString().ToLower();
    }
    public static CmdEnum? GetCmdEnum(this string str)
    {
        foreach (CmdEnum type in Enum.GetValues<CmdEnum>())
        {
            if (str.ToUpper() == type.ToString().ToUpper())
            {
                return type;
            }
        }
        return null;
    }
    public static string GetCmdEnumDespration(this CmdEnum en)
    {
        Type type = en.GetType();   //获取类型  
        MemberInfo[] memberInfos = type.GetMember(en.ToString());   //获取成员  
        if (memberInfos != null && memberInfos.Length > 0)
        {
            var attrs = memberInfos[0].GetCustomAttributes(typeof(DescriptionAttribute), false);  //获取描述特性 
            foreach (DescriptionAttribute attr in attrs)
            {
                return attr.Description;
            }
        }
        return string.Empty;
    }
}
