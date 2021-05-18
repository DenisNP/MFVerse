using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Verse.Helpers;
using Verse.Models.Salute;
using Verse.Models.Salute.Web;
using Verse.Services;

namespace Verse.Controllers
{
    [ApiController]
    [Route("/salute")]
    public class BackController : ControllerBase
    {
        private readonly SaluteService _saluteService;
        private readonly ILogger<BackController> _logger;

        public BackController(SaluteService saluteService, ILogger<BackController> logger)
        {
            _saluteService = saluteService;
            _logger = logger;
        }

        [HttpGet]
        public ContentResult Get()
        {
            return new()
            {
                ContentType = "text/html",
                Content = "It works!"
            };
        }
        
        [HttpPost]
        public async Task Post()
        {
            string body;
            using (var reader = new StreamReader(Request.Body)) 
                body = await reader.ReadToEndAsync();

            if (Program.LogEnabled)
                Console.WriteLine($"\n=================\n{body}\n=================\n");


            var request = JsonConvert.DeserializeObject<SaluteRequest>(body, Utils.ConverterSettings);
            if (request == null)
            {
                Console.WriteLine("Request is null:");
                Console.WriteLine(body);
                await Response.WriteAsync("Request is null");
                return;
            }
        
#if DEBUG == true
            Console.WriteLine(string.Join(" ", request.Tokens));
            Console.WriteLine(string.Join(" ", request.Lemmas));
            Console.WriteLine(string.Join(" ", request.Numbers));
#endif            
            /*if (Program.LogEnabled)
                _logger.LogInformation($"REQUEST:\n{JsonConvert.SerializeObject(request, Utils.ConverterSettings)}\n");*/
            
            try
            {
                SaluteResponse response = _saluteService.Handle(request);
                string stringResponse = JsonConvert.SerializeObject(response, Utils.ConverterSettings); 
                
                if (Program.LogEnabled)
                    _logger.LogInformation($"RESPONSE:\n{stringResponse}\n");

                await Response.WriteAsync(stringResponse);
                return;
            }
            catch (Exception e)
            {
                _logger.LogError($"\n::: ERROR for request:\n{body}:\n{e}\n");
                Console.WriteLine(e);

                var errorResponse = new SaluteResponse(request, new SaluteResponsePayload
                {
                    PronounceText = "Произошла какая-то ошибка, разработчик уже уведомлён.",
                    Items = new List<SItem>{ new BubbleItem("Произошла какая-то ошибка, разработчик уже уведомлён.") }
                });

                await Response.WriteAsync(JsonConvert.SerializeObject(errorResponse, Utils.ConverterSettings));
                return;
            }
        }
    }
}