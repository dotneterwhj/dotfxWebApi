using System;
using System.Net;

namespace DotneterWhj.ToolKits
{
    /// <summary>
    /// 通用返回信息类
    /// </summary>
    public class MessageModel<T>
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public HttpStatusCode Status { get; set; } = HttpStatusCode.ServiceUnavailable;

        /// <summary>
        /// 操作是否成功
        /// </summary>
        public bool Success
        {
            get
            {
                return (int)Status < 400;
            }
        }

        /// <summary>
        /// 返回信息
        /// </summary>
        public string Msg { get; set; } = "异常";

        /// <summary>
        /// 返回时间
        /// </summary>
        public DateTime Timestamp
        {
            get
            {
                return DateTime.Now;
            }
        }

        /// <summary>
        /// 当前版本
        /// </summary>
        public string Version { get; set; } = "5.0";

        /// <summary>
        /// 返回数据集合
        /// </summary>
        public T Data { get; set; }
    }
}