using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCHBrain.Model
{
    /// <summary>
    /// Represents the response of the SpeechRecognition.
    /// </summary>
    public class Response
    {
        /// <summary>
        /// The corresponding sentence in the grammar with the pronounced sentence
        /// </summary>
        public String Text { get; set; }

        /// <summary>
        /// Represents the degree of confidence regarding the grammar sentence the voice recognizer understood.
        /// </summary>
        public double Confidence { get; set; }

        /// <summary>
        /// A set of key/values from the grammar, included in the pronounced sentence.
        /// </summary>
        public Dictionary<string, object> SemanticValue { get; set; }

        /// <summary>
        /// List of the Words contained in the sentence which was recognized.
        /// </summary>
        public List<String> Words { get; set; }
    }
}
