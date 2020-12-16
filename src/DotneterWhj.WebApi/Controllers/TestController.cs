using DotneterWhj.DataTransferObject;
using DotneterWhj.IServices;
using Microsoft.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebApi.OutputCache.V2;

namespace DotneterWhj.WebApi.Controllers.v1
{
    /// <summary>
    /// 测试用
    /// </summary>
    //[ApiVersion("1.0", Deprecated = true)]
    [ApiVersionNeutral]
    public class TestController : ApiController
    {

        public TestController()
        {
    
        }


        //[ResponseType(typeof())]
        [HttpGet]
        [ApiAuthorize(Roles = "")]
        //[CacheOutput(ClientTimeSpan = 60, ServerTimeSpan = 60)]
        public async Task<IHttpActionResult> Get(string id)
        {
            return Ok("231");
        }
    }
}
