using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DotneterWhj.DataTransferObject;
using DotneterWhj.IServices;
using DotneterWhj.Repository;
using DotneterWhj.ToolKits;
using DotneterWhj.WebApi.Controllers.V5;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DotneterWhj.WebApi.Tests
{
    [TestClass]
    public class TestTeachMaterialController
    {
        [TestMethod]
        public void TestMethod1()
        {
            //var mockTeachMaterialService = new Mock<ITeachMaterialService>();
            //mockTeachMaterialService.Setup(s => s.GetAllAsync()).Returns(
            //    (new List<TeachMaterialLibraryDto>
            //    { 
            //        new TeachMaterialLibraryDto { }
            //    }).AsQueryable());
            //var mockMapper = new Mock<IMapper>();
            //var mockBasicApiService = new Mock<IBasicApiService>();

            //var mockICustomLogger = new Mock<ICustomLogger<TeachMaterialLibraryController>>();

            //var controller = new TeachMaterialLibraryController(
            //    mockTeachMaterialService.Object,
            //    mockICustomLogger.Object,
            //    mockMapper.Object,
            //    mockBasicApiService.Object);

            //var all = controller.GetTeachMaterialLibraries().Result;
        }
    }
}
