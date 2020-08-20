namespace REBGV.Functions
{
    public class PID
    {
        public int RawPid { get; }
        public string FormattedPid
        {
            get { return RawPid.ToString().Insert(3, "-").Insert(7, "-"); }
        }

        // The constructor sets the PID's number based on the integer passed to it.

        public PID(int currentPid)
        {
            RawPid = currentPid;
        }
    }
}