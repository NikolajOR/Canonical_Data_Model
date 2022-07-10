using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Canonical_Data_Model
{
    public class KLM
    {
        public string Airline { get; set; }

        public string FlightNo { get; set; }

        public string Destination { get; set; }

        public string Origin { get; set; }

        public string DatoTid { get; set; }

        public void SendMessage(string modtager)
        {
            Message message = new Message();

            var sdt = new DateTime(2017, 3, 6);
            DateTime utctime = DateTime.UtcNow;

            KLM klm = new KLM();
            klm.Airline = "KLM";
            klm.FlightNo = "154";
            klm.Destination = "San Diego";
            klm.Origin = "Schipol";
            int year = sdt.Year;
            int day = sdt.Day;
            klm.DatoTid = $"{sdt.ToString("MMMM")} {day} {year} {utctime.Hour}:{utctime.Minute}";

            message.Body = klm;
            message.Label = $"{klm.Airline} {modtager}";

            MessageQueue messageQueue = null;
            if (MessageQueue.Exists(@".\Private$\CDM"))
            {
                messageQueue = new MessageQueue(@".\Private$\CDM");
                messageQueue.Label = "Passenger info";
            }
            else
            {
                // Create the Queue
                MessageQueue.Create(@".\Private$\CDM");
                messageQueue = new MessageQueue(@".\Private$\CDM");
                messageQueue.Label = "GateInfo Queue";
            }
             
            messageQueue.Send(message);
        }

        public void ReciveMessage()
        {
            MessageQueue messageQueue = new MessageQueue(@".\Private$\KLM");

            try
            {
                Message message = messageQueue.Receive();
                string fra = message.Label;

                Console.WriteLine("[KLM]: Jeg har modtaget en besked fra " + fra);

            }
            catch (Exception e)
            {

                Console.WriteLine("Der gik noget galt i modtagelsen af beskeden " + e);

            }


        }
    }
}
