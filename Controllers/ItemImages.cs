using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ItemsWebApp.Controllers
{
    public class ItemImages
    {
        public int id { get; set; }
        public int item_id { get; set; }
        public string image_file_type { get; set; }
        public byte[] image_data { get; set; }
        public int tag_id { get; set; }
  
    }
}
