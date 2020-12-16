using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace DotneterWhj.WebApi.App_Start
{
    /// <summary>
    /// json序列化器设置
    /// </summary>
    public class JsonSerializerConfig
    {
        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="config"></param>
        public static void Register(HttpConfiguration config)
        {

            // Remove the JSON formatter
            // config.Formatters.Remove(config.Formatters.JsonFormatter);

            // or

            // Remove the XML formatter
            // config.Formatters.Remove(config.Formatters.XmlFormatter);

            config.Formatters.Remove(config.Formatters.FormUrlEncodedFormatter);

            var json = config.Formatters.JsonFormatter;


            // 若要以 camel 大小写形式编写 JSON 属性名称，而不更改数据模型，请在序列化程序上设置 CamelCasePropertyNamesContractResolver 
            json.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // 若要写入缩进的 JSON，请将 格式 设置设置为 "格式设置"
            // json.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;

            json.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";

            json.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.None;
        }
    }
}