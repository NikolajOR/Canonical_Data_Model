using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Canonical_Data_Model
{
    class CDM
    {

        
        // SAS -> fælles dataformat -> KLM
        
        public void DataTransform()
        {
            MessageQueue messageQueue = new MessageQueue(@".\Private$\CDM");

            messageQueue.MessageReadPropertyFilter.SetAll();

            messageQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(SAS), typeof(SW), typeof(KLM) });

            Message[] message = messageQueue.GetAllMessages();

            foreach(Message m in message)
            {
                Translater(m);
            }
           messageQueue.Purge();
        }

        public void Translater(Message message)
        {
            string tekst = message.Label;

            string[] splittekst = tekst.Split(' ');

           
            if (splittekst[0] == "SAS")
            {

                SAS sas = (SAS)message.Body;

                DataHub dataHub = new DataHub();
                dataHub.Airline = sas.Airline;
                dataHub.FlightNo = sas.FlightNo;
                dataHub.Destination = sas.Destination;
                dataHub.Origin = sas.Origin;
                string[] data = sas.Dato.Split(' ');
                string dato = data[0].Remove(data[0].Length-1, 1);
                dataHub.DatoTid = $"{data[1]} {dato} {data[2]} {sas.Tidspunkt}";

                Sender(splittekst[0], splittekst[1], dataHub);
            }

            if (splittekst[0] == "SW")
            {
                SW sw = (SW)message.Body;

                DataHub dataHub = new DataHub();
                dataHub.Airline = sw.Airline;
                dataHub.FlightNo = sw.FlightNo;
                dataHub.Destination = sw.Destination;
                string[] split = sw.Dato.Split('/');
                DateTime datotid = new DateTime(Convert.ToInt32(split[2]), Convert.ToInt32(split[1]), Convert.ToInt32(split[0]));
                DateTime dt = DateTime.Parse(sw.Departure);
                
                dataHub.DatoTid = $"{datotid.Year} {datotid.ToString("MMMM")} {datotid.Day} {dt.ToString("HH:mm")}";

                Sender(splittekst[0], splittekst[1], dataHub);
            }

            if (splittekst[0] == "KLM")
            {
                KLM klm = (KLM)message.Body;

                DataHub dataHub = new DataHub();
                dataHub.Airline = klm.Airline;
                dataHub.FlightNo = klm.FlightNo;
                dataHub.Destination = klm.Destination;
                dataHub.DatoTid = klm.DatoTid;

                Sender(splittekst[0], splittekst[1], dataHub);
            }

        }

        public void Sender(string from, string modtager, DataHub dataHub)
        {
            if (modtager == "KLM")
            {
                KLM klm = new KLM();

                klm.Airline = dataHub.Airline;
                klm.FlightNo = dataHub.FlightNo;
                klm.Destination = dataHub.Destination;
                klm.DatoTid = dataHub.DatoTid;

                Message message = new Message();
                message.Body = klm;
                message.Label = from;

                SendToReciver(modtager, message);
            }

            if (modtager == "SAS")
            {
                SAS sas = new SAS();

                sas.Airline = dataHub.Airline;
                sas.FlightNo = dataHub.FlightNo;
                sas.Destination = dataHub.Destination;
                string[] split = dataHub.DatoTid.Split(' ');
                sas.Dato = $"{split[1]} {split[0]} {split[2]}";
                sas.Tidspunkt = split[3];

                Message message = new Message();
                message.Body = sas;
                message.Label = from;

                SendToReciver(modtager, message);

            }

            if (modtager == "SW")
            {

                SW sw = new SW();

                sw.Airline = dataHub.Airline;
                sw.FlightNo = dataHub.FlightNo;
                sw.Destination = dataHub.Destination;
                string[] split = dataHub.DatoTid.Split(' ');
                int month = DateTime.ParseExact(split[0], "MMMM", CultureInfo.CurrentCulture).Month;
                sw.Dato = $"{month}/{split[1]}/{split[2]}";
                DateTime time = DateTime.Parse(split[3]);
                sw.Departure = time.ToString("hh:mm tt");

                Message message = new Message();
                message.Body = sw;
                message.Label = from;

                SendToReciver(modtager, message);

            }
        }

        public void SendToReciver(string reciver, Message message)
        {
            MessageQueue messageQueue = null;
            if (MessageQueue.Exists(@".\Private$\" + reciver))
            {
                messageQueue = new MessageQueue(@".\Private$\" + reciver);
                messageQueue.Label = "Passenger info";
            }
            else
            {
                // Create the Queue
                MessageQueue.Create(@".\Private$\" + reciver);
                messageQueue = new MessageQueue(@".\Private$\" + reciver);
                messageQueue.Label = "GateInfo Queue";
            }

            messageQueue.Send(message);
        }

    }
}
