using Common.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.SystemOperations
{
    public class LoginSO : SystemOperationBase
    {
        private readonly User user;
        public User Result { get; set; }

        public LoginSO(User user)
        {
            this.user = user;
        }

        protected override void ExecuteConcreteOperation()
        {
            Result = broker.Login(user);
        }
    }
}
