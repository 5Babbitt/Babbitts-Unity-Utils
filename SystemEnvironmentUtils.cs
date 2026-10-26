using System;

namespace BabbittsUnityUtils
{
    public static class SystemEnvironmentUtils
    {
        public static bool EnvironmentHasCommandFlag(string flag)
        {
            string[] args = Environment.GetCommandLineArgs();
            foreach (var arg in args)
            {
                if (arg.Equals(flag, StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }
    }
}