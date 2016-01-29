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
        /// <summary>
        /// GPU temperature
        /// </summary>
        [StateObjectLink("HWMonitor", "/nvidiagpu/0/temperature/0")]
        public StateObjectNotifier GPU { get; set; }

        /// <summary>
        /// Name day
        /// </summary>
        [StateObjectLink("DayInfo", "NameDay")]
        public StateObjectNotifier Fete { get; set; }

        /// <summary>
        /// Informations about the sun
        /// </summary>
        [StateObjectLink("DayInfo", "SunInfo")]
        public StateObjectNotifier Sun { get; set; }

        /// <summary>
        /// Information about the % of the battery
        /// </summary>
        [StateObjectLink("BatteryChecker","7EAC20FAA976CD84DE5ADE97A2501658909BDE44")]
        public StateObjectNotifier Battery { get; set; }

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
            
            // Adding our AI into the group which receive the information about the SpeechRecognition.
            PackageHost.AddToGroup("JarvisSpeech");
        }

        /// <summary>
        /// Response handler
        /// </summary>
        /// <param name="response">Response of the SpeechRecognition</param>
        [MessageCallback(IsHidden = true)]
        public void SpeechReceive(object response)
        {
            string text = "";

            Response obj = JsonConvert.DeserializeObject<Response>(response.ToString());
            String semanticValue = (string)obj.SemanticValue["constellation"];

            if (semanticValue == "Get")
            {
                switch ((string)obj.SemanticValue["data_type"])
                {
                    case "la fête du jour":
                        text = String.Format("Aujourd'hui, c'est la fête des {0}",this.Fete.DynamicValue);
                        break;
                    case "la température du GPU":
                        text = String.Format("La température de votre carte graphique est de {0} °C",this.GPU.DynamicValue.Value);
                        break;
                    case "le pourcentage de ma batterie":
                        int value = Battery.DynamicValue.EstimatedChargeRemaining;
                        PackageHost.WriteInfo("{0}", value);
                        if (value < 50)
                        {
                            text = String.Format("Il vous reste {0}% de batterie.",value);
                        }
                        else
                        {
                            text = String.Format("Votre batterie est chargée à {0}%.", value);
                        }
                        break;
                    default:
                        break;
                }
            }
            else if (semanticValue == "Lock")
            {
                switch((string)obj.SemanticValue["data_lock"])
                {
                    case "mon PC":
                    case "mon ordinateur":
                        PackageHost.CreateScope("WindowsControl").Proxy.LockWorkStation();
                        text = "C'est verrouillé.";
                        break;
                    default:
                        break;
                }
            }
            else if (semanticValue == "Time")
            {
                switch ((string)obj.SemanticValue["data_time"])
                {
                    case "se couche le soleil":
                        text = String.Format("Le soleil se couchera à {0}", Sun.DynamicValue.Sunset.Value);
                        break;
                    case "se lève le soleil":
                        text = String.Format("Le soleil s'est levé à {0}", Sun.DynamicValue.Sunrise.Value);
                        break;
                }
            }
            else if (semanticValue == "None")
            {
                text = "D'accord.";
            }
            else if (semanticValue == "Prepare")
            {
                text = "Je pense que ça va au-delà de mes compétences pour le moment. Mais, il vous suffit d'appuyer sur le bouton de la cafetière.";
            }
            else if (semanticValue == "End")
            {
                text = "Merci de nous avoir écoutés. Et le café est prêt.";
            }
            else if (semanticValue == "Sing")
            {
                text = "Une souris verte, qui courait dans l'herbe, je l'attrape par la queue, je la montre à ces messieurs. Ces messieurs me disent : Trempez-la dans l'huile, trempez-la dans l'eau, ça fera un escargot tout chaud.";
            }

            if (text != "")
            {
                PackageHost.CreateScope("Jarvis").Proxy.Speak(text);
            }
        }
    }
}
