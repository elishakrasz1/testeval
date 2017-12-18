using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ItemsWebApp.Controllers
{
    public class SubItems
    {
        public int id { get; set; } = 0;
        public int item_id { get; set; } = 0;
        public string entity_text { get; set; }
        public string entity_html { get; set; }
        public int ans_no { get; set; }
        public List<SubItemImages> images { get; set; }

    }
}
