using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Verse.Models;
using Verse.Models.Salute;

namespace Verse.Pages
{
    public class MainPage : ComponentBase
    {
        [Inject]
        public IJSRuntime Js { get; set; }
        public ParseResult LastResult { get; set; }

        protected override Task OnInitializedAsync()
        {
            if (Program.LogEnabled)
                Console.WriteLine("Main component opened");
            
            Js.InvokeVoidAsync("initializeClient", Program.GetClientToken(), DotNetObjectReference.Create(this));
            return base.OnInitializedAsync();
        }

        [JSInvokable]
        public void CommandGot(string commandRaw)
        {
            if (Program.LogEnabled)
                Console.WriteLine(commandRaw);
            
            var command = JsonConvert.DeserializeObject<SmartAppCommand>(commandRaw, new SmartAppCommandConverter());
            if (command?.Type == "smart_app_data")
            {
                if (command.Data.ContainsKey("state_updated"))
                {
                    var stateString = command.Data["state_updated"].ToString();
                    if (stateString != null)
                    {
                        LastResult = JsonConvert.DeserializeObject<ParseResult>(stateString);
                        StateHasChanged();
                    }
                }
                else if (command.Data.ContainsKey("close"))
                {
                    Close();
                }
            }
        }

        protected void SendData(string action, Dictionary<string, object> data, bool enableCallback = false)
        {
            Js.InvokeVoidAsync("sendData", action, data, enableCallback);
        }

        protected void Close()
        {
            Js.InvokeVoidAsync("close");
        }
    }
}