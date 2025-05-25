namespace Events
{
    public class OnCoinCollectionIncreased
    {
        public float CoinCollectionPerSeconds { get; private set; }

        public OnCoinCollectionIncreased(float coinCollectionPerSeconds)
        {
            CoinCollectionPerSeconds = coinCollectionPerSeconds;
        }
    }
}