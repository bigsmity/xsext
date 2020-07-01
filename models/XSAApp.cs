using System;
using System.Collections.Generic;
using System.Text;

namespace xsext.models
{
    public class XSAApp
    {
        public string Name { get; set; }
        public string RequestedState { get; set; }
        public string Instances { get; set; }
        public string Memory { get; set; }
        public string Disk { get; set; }
        public string Alerts { get; set; }
        public string Urls { get; set; }

    }
}
