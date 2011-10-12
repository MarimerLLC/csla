using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            catch (Exception exception)
            {
                // Do logging
                throw;
            }
        }
    }
}
