using System;
using System.IO;
using System.Text;

namespace NFMWorld.Mad
{
    public class DevConsoleWriter : TextWriter
    {
        private readonly DevConsole _devConsole;
        private readonly TextWriter _originalOut;
        private string _logLevel = "default";

        public DevConsoleWriter(DevConsole devConsole, TextWriter originalOut)
        {
            _devConsole = devConsole;
            _originalOut = originalOut;
        }

        public override Encoding Encoding => Encoding.UTF8;

        public void WriteLine(string? value, string logLevel)
        {
            if (value != null)
            {
                _devConsole.Log(value, logLevel);
                _originalOut.WriteLine(value);
            }
        }

        public void Write(string? value, string logLevel)
        {
            if (value != null)
            {
                _devConsole.Log(value, _logLevel);
                _originalOut.Write(value);
            }
        }
    }
}