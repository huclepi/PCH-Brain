using Constellation;
using Constellation.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Constellation.Control;
using Newtonsoft.Json;
using PCHBrain.Model;

namespace PCHBrain
{
    public class Program : PackageBase
    {

        [StateObjectLink("HWMonitor", "/nvidiagpu/0/temperature/0")]
        public StateObjectNotifier GPU { get; set; }

        [StateObjectLink("DayInfo", "NameDay")]
        public StateObjectNotifier Fete { get; set; }

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {
            if (!PackageHost.HasControlManager)
            {
                PackageHost.WriteError("Unable to connect !");
                return;
            }

            PackageHost.ControlManager.RegisterStateObjectLinks(this);

            PackageHost.AddToGroup("JackySpeech");
        }

        [MessageCallback(IsHidden = true)]
        public void SpeechReceive(Object response)
        {
            string text = "";

            Response obj = JsonConvert.DeserializeObject<Response>(response.ToString());
            String semanticValue = (string)obj.SemanticValue["constellation"];

            if (semanticValue == "Get")
            {
                switch ((string)obj.SemanticValue["data_type"])
                {
                    case "la fête du jour":
                        text = String.Format("Aujourd'hui, c'est la fête des {0}",this.Fete.DynamicValue.Value);
                        //PackageHost.WriteInfo("Fête du jour: {0}", this.Fete.DynamicValue);
                        break;
                    case "la température du GPU":
                        text = String.Format("La température de votre carte graphique est de {0} °C",this.GPU.DynamicValue.Value);
                        //PackageHost.WriteInfo("Température du GPU: {0}°C ({1}", this.GPU.DynamicValue.Value,text);
                        break;
                    default:
                        break;
                }
            }

            if (text != "")
            {
                PackageHost.CreateScope("Jarvis").Proxy.Speak(text);
            }
        }
    }
}
