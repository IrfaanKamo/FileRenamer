using System;

namespace RenamerPro.Classes
{
    public static class Logger
    {
        //--------------------------------------------------------------------------------

        public static void WriteError(string errorMessage)
        {
            Console.WriteLine("ERROR: " + errorMessage);
        }

        //--------------------------------------------------------------------------------

        public static void WriteWarning(string warningMessage)
        {
            Console.WriteLine("WARNING: " + warningMessage);
        }

        //--------------------------------------------------------------------------------
    }
}
