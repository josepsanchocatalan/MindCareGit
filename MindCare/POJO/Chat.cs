using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindCare.POJO
{
    public class Chat
    {
        public string Id { get; set; }
        public List<string> Participantes { get; set; }
        public Dictionary<string, ChatMessage> Messages { get; set; }

        public string DisplayNombreParticipantes
        {
            get
            {
                return string.Join(", ", Participantes);
            }
        }


    }
}
