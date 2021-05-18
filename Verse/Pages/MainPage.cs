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
        public SessionState State { get; set; } = new();
        public Foot Foot => new (State.CurrentFoot);

        protected override Task OnInitializedAsync()
        {
            if (Program.LogEnabled)
                Console.WriteLine("Main component opened");

            State = new SessionState
            {
                Syllables = new []
                {
                    new []
                    {
                        new Syllable {Text = "Б", Type = SyllableType.Consonant},
                        new Syllable {Text = "у", Type = SyllableType.Stressed},
                        new Syllable {Text = "р", Type = SyllableType.Consonant},
                        new Syllable {Text = "я", Type = SyllableType.Unstressed},
                    },
                    new []
                    {
                        new Syllable {Text = "мгл", Type = SyllableType.Consonant},
                        new Syllable {Text = "о", Type = SyllableType.Stressed},
                        new Syllable {Text = "ю", Type = SyllableType.Unstressed},
                    },
                    new []
                    {
                        new Syllable {Text = "н", Type = SyllableType.Consonant},
                        new Syllable {Text = "е", Type = SyllableType.Stressed},
                        new Syllable {Text = "б", Type = SyllableType.Consonant},
                        new Syllable {Text = "o", Type = SyllableType.Unstressed},
                    },
                    new []
                    {
                        new Syllable {Text = "кр", Type = SyllableType.Consonant},
                        new Syllable {Text = "о", Type = SyllableType.Stressed},
                        new Syllable {Text = "е", Type = SyllableType.Unstressed},
                        new Syllable {Text = "т", Type = SyllableType.Consonant}
                    }
                },
                CurrentFoot = FootType.Chorea
            };
            //Js.InvokeVoidAsync("initializeClient", Program.GetClientToken(), DotNetObjectReference.Create(this));
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
                        State = JsonConvert.DeserializeObject<SessionState>(stateString);
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