namespace REBGV.Functions
{
    public class Pid
    {
        public int RawPid { get; }
        public string FormattedPid
        {
            get { return RawPid.ToString().Insert(3, "-").Insert(7, "-"); }
        }

        // The PID's number is based on the integer passed to it.

        public Pid(int currentPid)
        {
            RawPid = currentPid;
        }
    }
}