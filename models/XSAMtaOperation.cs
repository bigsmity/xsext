using System;
using System.Collections.Generic;
using System.Text;

namespace xsext.models
{
    public class XSAMtaOperation
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string MtaId { get; set; }
        public string Status { get; set; }
        public string StartedAt { get; set; }
        public string StartedBy { get; set; }
    }

}
