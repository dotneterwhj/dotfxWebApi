using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotneterWhj.DataTransferObject
{
    public class OrderFiledModel
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// 是否降序，默认升序
        /// </summary>
        public bool IsDesc { get; set; }

        public static List<OrderFiledModel> GetOrderFiledModel(string[] orderFiledArray)
        {
            List<OrderFiledModel> orderFileds = null;

            if (orderFiledArray != null)
            {
                orderFileds = new List<OrderFiledModel>();
                foreach (var item in orderFiledArray)
                {
                    var order = item.Trim().Split(' ');
                    if (order.Length > 1)
                    {
                        orderFileds.Add(new OrderFiledModel
                        {
                            PropertyName = order[0],
                            IsDesc = string.Equals(order[1].Trim(), "asc", StringComparison.InvariantCultureIgnoreCase) ? false : true
                        });
                    }
                    else
                    {
                        orderFileds.Add(new OrderFiledModel
                        {
                            PropertyName = order[0]
                        });
                    }
                }
            }

            return orderFileds;
        }
    }
}
