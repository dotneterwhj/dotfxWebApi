namespace DotneterWhj.DataTransferObject
{
    public class AppSettings
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string Connection { get; set; }

        /// <summary>
        /// redis连接字符串
        /// </summary>
        public string RedisConnectionStr { get; set; }

        /// <summary>
        /// 当前环境
        /// </summary>
        public string ASPNET_ENVIRONMENT { get; set; }

    }
}