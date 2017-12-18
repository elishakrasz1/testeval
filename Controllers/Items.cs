using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ItemsWebApp.Controllers
{
    public class Items
    {
        public int entity_data_ik { get; set; }
        public SubItems question { get; set; }
        public List<SubItems> answers { get; set; } = new List<SubItems>();
    }
}
