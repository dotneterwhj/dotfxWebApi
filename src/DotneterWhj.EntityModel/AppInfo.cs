using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotneterWhj.EntityModel
{
    public class AppInfo : BaseEntity
    {
        public int Id { get; set; }

        public string AppId { get; set; }

        public string AppSecret { get; set; }

        public bool IsEnable { get; set; }
    }
}
