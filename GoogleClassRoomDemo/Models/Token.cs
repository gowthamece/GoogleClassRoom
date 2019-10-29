using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleClassRoomDemo.Models
{
    public class Token
    {
        public string Access_token
        {
            get;
            set;
        }
        public string Token_type
        {
            get;
            set;
        }
        public string Expires_in
        {
            get;
            set;
        }
        public string Refresh_token
        {
            get;
            set;
        }
        public string Auth_code
        {
            get;
            set;
        }
    }
}
