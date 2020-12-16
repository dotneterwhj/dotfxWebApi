using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotneterWhj.DataTransferObject
{
    /// <summary>
    /// 通用分页信息类
    /// </summary>
    public class PageModel<T>
    {
        /// <summary>
        /// 当前页标
        /// </summary>
        public int CurrentPage { get; set; } = 1;
        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount
        {
            get
            {
                return (int)Math.Ceiling(TotalCount * 1.0 / PageSize);
            }
        }
        /// <summary>
        /// 数据总数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 每页大小
        /// </summary>
        public int PageSize { set; get; }
        /// <summary>
        /// 返回数据
        /// </summary>
        public List<T> Data { get; set; }

    }
}
