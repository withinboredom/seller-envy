namespace Objects
{
    public interface ISubscribeTo<in TEvent>
    {
        void Handle(TEvent e);
    }
}