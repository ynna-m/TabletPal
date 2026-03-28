using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TabletPal.InputSender
{
    public static class InputSenderFactory
    {
        public static IInputSender Create()
        {
            var session = Environment.GetEnvironmentVariable("XDG_SESSION_TYPE");

            if (session == "wayland")
                return new YdotoolSender();

            return new XdotoolSender();
        }
    }
}