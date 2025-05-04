using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StudentManagement.Models;

namespace StudentManagement.Controllers
{
    [Microsoft.AspNetCore.Components.Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        public IConfiguration config;

        public UseAI useAi;

        public ConfigController(IConfiguration config, IOptions<UseAI> optionsAI /*, IOptionsMonitor<UseAI> MoptionsAI*/)
        {
            this.config = config;
            this.useAi = optionsAI.Value;
            // this.useAi = MoptionsAI.CurrentValue;
        }

        [HttpGet("useallai")]
        public UseAI FetchAllUseAI() // If multiple data of an object in JSON file
        {
            // return config["useAI"];
            return useAi;
        }

        // [HttpGet("useai")]
        // public string? FetchUseAI() // If Single data present in Json File 
        // {
        //    return config["useAI"];
        // }

        /* OR */

        // [HttpGet("useai")]
        // public string FetchUseAI()
        // {
        //     IConfiguration config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build(); // This is not Dependency Injection 
        //     string? useAI = config["UseAI"];
        //     return useAI;
        // }



    }
}
