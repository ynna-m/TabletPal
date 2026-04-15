using System;

namespace TabletPal.InputSender
{
    public static class InputSenderFactory
    {
        public static IInputSender Create()
        {
            var session = Environment.GetEnvironmentVariable("XDG_SESSION_TYPE");

            return new X11Sender();
        }
    }
}