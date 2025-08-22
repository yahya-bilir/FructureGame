using BasicStackSystem;

namespace Events
{
    public class OnStackObjectEjected
    {
        public readonly BasicStack Stack;
        public readonly IStackable Item;

        public OnStackObjectEjected(BasicStack stack, IStackable item)
        {
            Stack = stack;
            Item  = item;
        }
    }
}