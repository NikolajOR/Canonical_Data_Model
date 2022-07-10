using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Canonical_Data_Model
{
    class Program
    {
        static void Main(string[] args)
        {
            SAS sas = new SAS();
            SW sw = new SW();
            KLM klm = new KLM();

            CDM cdm = new CDM();

            sas.SendMessage("KLM");
            sw.SendMessage("SAS");
            klm.SendMessage("SW");

            cdm.DataTransform();

            sas.ReciveMessage();
            klm.ReciveMessage();
            sw.ReciveMessage();

            Console.Read();
        }
    }
}
