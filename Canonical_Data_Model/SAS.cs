using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Canonical_Data_Model
{
    public class SAS
    {
        public string Airline { get; set; }

        public string FlightNo { get; set; }

        public string Destination { get; set; }

        public string Origin { get; set; }

        public string ArivalDeparture { get; set; }

        public string Dato { get; set; }

        public string Tidspunkt { get; set; }


        public void SendMessage(string modtager)
        {
            Message message = new Message();

            var sdt = new DateTime(2017, 3, 6); 
            DateTime utctime = DateTime.UtcNow;

            SAS sas = new SAS();
            sas.Airline = "SAS";
            sas.FlightNo = "SK 239";
            sas.Destination = "JFK";
            sas.Origin = "CPH";
            sas.ArivalDeparture = "D";
            sas.Dato = $"{sdt.Day}. {sdt.ToString("MMMM")} {sdt.Year}";
            sas.Tidspunkt = $"{utctime.Hour}:{utctime.Minute}";

            message.Body = sas;
            message.Label = $"{sas.Airline} {modtager}";

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
            MessageQueue messageQueue = new MessageQueue(@".\Private$\SAS");

            Message combineMessage = new Message();

            try
            {
                Message message = messageQueue.Receive();
                string fra = message.Label;

                Console.WriteLine("[SAS]: Jeg har modtaget en besked fra " + fra);

            }
            catch (Exception e)
            {

                Console.WriteLine("Der gik noget galt i modtagelsen af beskeden " + e);

            }


        }



    }



}
