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
            Response obj = JsonConvert.DeserializeObject<Response>(response.ToString());
            String semanticValue = (string)obj.SemanticValue["constellation"];

            if (semanticValue == "Get")
            {
                switch ((string)obj.SemanticValue["data_type"])
                {
                    case "la fête du jour":
                        PackageHost.WriteInfo("Fête du jour: {0}", this.Fete.DynamicValue);
                        break;
                    case "la température du GPU":
                        PackageHost.WriteInfo("Température du GPU: {0}°C", this.GPU.DynamicValue.Value);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
