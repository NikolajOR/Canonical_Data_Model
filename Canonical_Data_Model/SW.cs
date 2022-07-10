using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Canonical_Data_Model
{
    public class SW
    {
        public string Airline { get; set; }

        public string FlightNo { get; set; }

        public string Destination { get; set; }

        public string Dato { get; set; }

        public string Departure { get; set; }

        public void SendMessage( string modtager)
        {
            Message message = new Message();

            var sdt = new DateTime(2017, 3, 6);
            DateTime time = DateTime.Now;

            string name = "SW";

            SW sw = new SW();
            sw.Airline = "South West Airlines";
            sw.FlightNo = "SW056";
            sw.Destination = "New York";
            sw.Dato = sdt.ToString("dd/MM/yyyy").Replace('-','/');
            CultureInfo ci = new CultureInfo("en-US");
            sw.Departure = time.ToString("hh:mm tt", ci);

            message.Body = sw;
            message.Label = $"{name} {modtager}";

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
            MessageQueue messageQueue = new MessageQueue(@".\Private$\SW");
            
             messageQueue.Formatter = new XmlMessageFormatter(new Type[] {typeof(SW)});

            Message combineMessage = new Message();

            try
            {
                Message message = messageQueue.Receive();

                SW sw = (SW)message.Body;

                Console.WriteLine(sw.Departure);

                string fra = message.Label;

                Console.WriteLine("[SW]: Jeg har modtaget en besked fra " + fra);

            }
            catch (Exception e)
            {

                Console.WriteLine("Der gik noget galt i modtagelsen af beskeden " + e);

            }


        }
    }

}
