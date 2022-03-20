using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignSystem
{
    internal class PersonInfo
    {

        public  int id { get; set; }
        public string name { get; set; }

        public string email { get; set; }

        public string password { get; set; }

/*        public PersonInfo(int id, string name, string email, string password)
        {
            (this.id, this.name, this.email, this.password) = (id, name, email, password);
        }*/

        public PersonInfo()
        {

            Console.WriteLine("Burning");
        }
    }
}
