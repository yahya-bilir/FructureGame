using BasicStackSystem;

namespace Events
{
    public class OnStackObjectReceived
    {
        public readonly BasicStack Stack;
        public readonly IStackable Item;

        public OnStackObjectReceived(BasicStack stack, IStackable item)
        {
            Stack = stack;
            Item  = item;
        }
    }
}