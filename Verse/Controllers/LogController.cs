using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Verse.Controllers
{
    [ApiController]
    [Route("/log")]
    public class LogController : ControllerBase
    {
        [HttpPost]
        public async Task Log()
        {
            if (Program.LogEnabled)
            {
                string text;
                using (var reader = new StreamReader(Request.Body))
                    text = await reader.ReadToEndAsync();

                Console.WriteLine();
                Console.WriteLine("Log got:");
                Console.WriteLine(text);
                Console.WriteLine();
            }

            await Response.WriteAsync("ok");
        }
    }
}