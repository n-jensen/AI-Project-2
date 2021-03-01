using System;
using System.Collections.Generic;
using System.Text;

namespace AI_P2
{
    public class Node
    {
        public double Probability { get; set; }

        public double EvidenceProp { get; set; }

        public Position Position { get; set; }
    }
}
