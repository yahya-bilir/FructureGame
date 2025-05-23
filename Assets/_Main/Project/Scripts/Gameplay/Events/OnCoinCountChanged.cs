namespace Events
{
    public class OnCoinCountChanged
    {
        public int CurrentCoinCount { get; private set; }
        
        public OnCoinCountChanged(int currentCoinCount)
        {
            CurrentCoinCount = currentCoinCount;
        }
    }
}