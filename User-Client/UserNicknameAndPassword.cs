using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User_Client
{
    struct UserNicknameAndPassword
    {
        public UserNicknameAndPassword(string nickname, string password)
        {
            this.Nickname = nickname;
            this.Password = password;
        }
        public string Nickname;
        public string Password;
    }
}
