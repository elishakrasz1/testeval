using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ItemsWebApp.Controllers
{
    public class SubEntityImage
    {
        public int id { get; set; } = 0;
        public int subitem_id { get; set; } = 0;
        public byte[] image_data { get; set; }
        public string image_file_type { get; set; }
        public int tag_id { get; set; }
    }
}
