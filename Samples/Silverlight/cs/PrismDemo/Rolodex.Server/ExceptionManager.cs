using System;

namespace Rolodex
{
    public static class ExceptionManager
    {
        public static void Process(Action action)
        {
            try
            {
                action();
            }
            catch (Exception)
            {
                // Do logging
                throw;
            }
        }
    }
}
