using System.Threading.Tasks;

namespace TabletPal.InputSender
{
    public interface IInputSender
    {
        void SendHold(string[] keys);
        void SendChord(string[] keys);
        void SendRelease(string[] keys);
        void SendClick(string keys);
    }
}