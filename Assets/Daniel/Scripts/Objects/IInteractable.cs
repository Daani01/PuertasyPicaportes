public interface IInteractable
{
    void InteractObj();
}

public interface IUsable : IInteractable
{
    void Use();

    public enum ActivationType
    {
        OneTime,
        MultipleTimes,
        Charge
    }
}
