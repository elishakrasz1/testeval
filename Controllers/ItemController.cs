using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace ItemsWebApp.Controllers
{
    [Route("api/[controller]")]
    public class ItemController : Controller
    {
        
        [HttpGet("[action]")]
        public DBEntityItem GetItem(int idIndb)
        {
            DBController connection = new DBController();
            return  connection.GetItemTextByID(idIndb);            
        }
    }
}
