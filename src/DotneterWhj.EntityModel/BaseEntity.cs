using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace DotneterWhj.EntityModel
{
    public class BaseEntity
    {
        public DateTime? CreateTime { get; set; } = DateTime.Now;

        [StringLength(100)]
        public string CreateId { get; set; } = HttpContext.Current?.User?.Identity?.Name;

        public DateTime? LastModifyTime { get; set; } = DateTime.Now;

        [StringLength(100)]
        public string LastModifyId { get; set; } = HttpContext.Current?.User?.Identity?.Name;
    }
}