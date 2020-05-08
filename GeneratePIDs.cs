using System.Collections.Generic;

namespace REBGV.Functions
{
    public class GeneratePIDs
    {
        private static int _limit { get; } = 10;
        private static List<PID> _pids { get; } = new List<PID>();

        public static void Generate(int quantity)
        {
            // Fetches the latest PID from blob storage.
            
            int latestPid = int.Parse("111111111");

            // Validates the quantity, to make sure the built-in limit isn't exceeded.

            quantity = quantity <= _limit ? quantity : _limit;

            // Creates PIDs equal to the quantity requested.

            for(int i = 0; i < quantity; i++)
            {
                _pids.Add(new PID(++latestPid));
            }
        }
    }
}