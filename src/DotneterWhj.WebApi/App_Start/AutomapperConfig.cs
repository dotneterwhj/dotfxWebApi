using AutoMapper;
using DotneterWhj.Extensions.AutoMappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace DotneterWhj.WebApi.App_Start
{
    /// <summary>
    /// automapper对象映射配置
    /// </summary>
    public class AutomapperConfig
    {
        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="config"></param>
        public static void Register(HttpConfiguration config)
        {
            var autoConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ViewModelDtoProfile>();
            });
        }
    }
}