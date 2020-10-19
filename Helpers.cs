using System.Collections.Generic;

namespace REBGV.Functions
{
    public class Helpers
    {
        private static int _limit { get; } = 10;

        public static List<string> Generate(string startingPid, int quantity)
        {
            int pid;

            int.TryParse(startingPid, out pid);

            // Validate the quantity, to make sure the built-in limit isn't exceeded.

            quantity = quantity <= _limit ? quantity : _limit;

            // Create PIDs equal to the quantity requested.

            List<string> pids = new List<string>();

            for(int i = 0; i < quantity; i++)
            {
                pids.Add(new Pid(++pid).FormattedPid);
            }

            return pids;
        }
    }
}