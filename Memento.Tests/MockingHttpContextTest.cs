using Memento.Controllers;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Moq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace Memento.Tests
{
    public class MockingHttpContextTest
    {
        private Mock<HttpContext> moqContext;
        private Mock<HttpRequest> moqRequest;

        public void SetupTests()
        {
            moqContext = new Mock<HttpContext>();
            moqRequest = new Mock<HttpRequest>();
            moqContext.Setup(x => x.Request).Returns(moqRequest.Object);
            
        }

       
        //#region RegionTest
        //[Fact]
        
        //#endregion

    }
}
