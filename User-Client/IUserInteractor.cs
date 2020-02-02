using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User_Client
{
    interface IUserInteractor
    {
        void ShowFolderFiles(IEnumerable<string> files);
        void ShowFolderForlders(IEnumerable<string> folders);
        UserAction AskAction();
    }
}
