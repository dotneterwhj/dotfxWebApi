using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotneterWhj.WebApi
{
    /// <summary>
    /// 忽略模型验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class IgnoreModelStateValidAttribute : Attribute
    {

    }
}