using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCHBrain.Model
{
    public class Response
    {
        public String Text { get; set; }
        public double Confidence { get; set; }
        public Dictionary<string, object> SemanticValue { get; set; }
        public List<String> Words { get; set; }
    }
}
