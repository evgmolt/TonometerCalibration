using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCalibr
{
    public interface IMessageHandler
    {
        event Action<Message> WindowsMessageHandler;
    }
}
