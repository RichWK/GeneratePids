using System.Collections.Generic;

namespace REBGV.Functions
{
    public class GeneratePIDs
    {
        private static int _limit { get; } = 10;

        public static List<string> Generate(int quantity)
        {
            // Fetches the latest PID from blob storage.
            
            int latestPid = int.Parse("100000000");

            // Validates the quantity, to make sure the built-in limit isn't exceeded.

            quantity = quantity <= _limit ? quantity : _limit;

            // Creates PIDs equal to the quantity requested.

            List<string> pids = new List<string>();

            for(int i = 0; i < quantity; i++)
            {
                pids.Add(new PID(++latestPid).FormattedPid);
            }

            return pids;
        }
    }
}