using System;
using System.Collections.Generic;
using System.Text;

namespace Users.FunctionalTests
{
    public static class Helpers
    {
        public static string TestRoot { get; }
            = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin"));
    }
}
