

using System.Net;
using System.Threading.Tasks;
using Xunit;
using Backend.Controllers;
using Backend.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Backend.Test
{

    public class SensorControllerUnitTests : TestWithSqlite
    {
        private readonly ApplicationUser _user;
        private readonly SensorController _controller;
        public SensorControllerUnitTests()
        {
            _user = new Models.ApplicationUser() { Email = "UserWithSensor@sbdia.iot" };
            base.DbContext.Add(_user);
            base.DbContext.SaveChanges();
            var claimsUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, this._user.Id) }, "mock"));
            _controller = new SensorController(this.DbContext);
            _controller.ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext() { User = claimsUser } };
        }

        [Fact]
        public void GetSensorEmptyList()
        {
            // Act
            var actionResult = _controller.GetSensorAll();
            // Assert
            Assert.Null(actionResult.Result);
        }
    }
}