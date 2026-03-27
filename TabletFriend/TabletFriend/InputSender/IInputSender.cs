using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TabletFriend.InputSender
{
    public interface IInputSender
    {
        Task SendHold(string keys);
        Task SendChord(string keys);
        Task SendRelease(string keys);
        Task SendClick(string keys);
    }
}