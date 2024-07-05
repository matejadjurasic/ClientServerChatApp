using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Domain
{
    [Serializable]
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }  
        public string Password { get; set; }

        public override bool Equals(object obj)
        {
            return obj is User user &&
                   Username == user.Username;
        }

        public override int GetHashCode()
        {
            return -182246463 + EqualityComparer<string>.Default.GetHashCode(Username);
        }
    }
}
