using System;
using System.Collections.Generic;
using System.Text;

namespace xsext.models
{
    public class XSARoute
    {
        public string Name { get; set; }
        public string Domain { get; set; }
        public string Port { get; set; }
        public string Path { get; set; }
        public string Type { get; set; }
        public string Apps { get; set; }
    }
}
