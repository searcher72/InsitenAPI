using InsitenWebAPI.Controllers;
using InsitenWebAPI.ServiceInterfaces;
using NUnit.Framework;
using System.Threading.Tasks;

namespace InsitenAPIUT
{
    public class Tests
    {
        DataController _controller;
        IDataService _dataService;

        public Tests()
        {
            _dataService = 
            _controller = new DataController();
        }

        [OneTimeSetUp]
        public void Setup()
        {

        }

        [Test]
        public async Task GetDataTest()
        {
            
            Assert.Pass();
        }
    }
}