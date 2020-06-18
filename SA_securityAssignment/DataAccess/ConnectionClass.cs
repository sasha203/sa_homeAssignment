using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace DataAccess
{
    public class ConnectionClass 
    {
        //musicShareDBEntities
        public musicShareDBEntities Entity { get; set; }

        //Constructor which will connect to DB on call.
        public ConnectionClass() {
            Entity = new musicShareDBEntities();
        }
    }
}
