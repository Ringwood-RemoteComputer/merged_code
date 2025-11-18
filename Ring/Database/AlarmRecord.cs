using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ring.Database
{
    public class AlarmRecord
    {
        public int ALMID { get; set; }
        public int ALMNUMBER { get; set; }
        public string ALMTIME { get; set; }
        public string ALMDATE { get; set; }
        public string ACKTIME { get; set; }
        public string ACKDATE { get; set; }
        public int ALMTYPENUMBER { get; set; }
        public int ALMSTATUSNUMBER { get; set; }
        public int ALMIDTYPE { get; set; }
        public string ALMNAME { get; set; }
    }
}
