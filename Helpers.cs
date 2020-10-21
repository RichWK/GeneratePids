using Newtonsoft.Json;
using System.Collections.Generic;

namespace REBGV.Functions
{
    public class Helpers
    {
        private static int _limit { get; } = 10;

        public static string GeneratePids(string startingPid, int quantity)
        {
            int pid;
            List<string> pids = new List<string>();

            int.TryParse(startingPid, out pid);

            // Validate the quantity, to make sure the built-in limit isn't exceeded.

            quantity = quantity <= _limit ? quantity : _limit;

            for(int i = 0; i < quantity; i++)
            {
                pids.Add(new Pid(++pid).FormattedPid);
            }

            // UpdateDatabaseAsync()

            return JsonConvert.SerializeObject(pids);
        }
    }
}