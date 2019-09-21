namespace AggravationCP
{
    public interface IBoardPosition
    {
        int GetDiffTopMiddle(int howMany);
        int GetDiffTopEdges(int space);
        /// <summary>
        /// needs to be 1 to 4 or -1 to -4
        /// 1 to 4 means to increase top  that makes most sense.
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int GetDiffTopHome(int row);
    }
}