using UnityEngine;

public interface IInteractable
{
    void InteractObj();
}

public interface IUsable : IInteractable
{
    void Use();
    void DesActivateObj(Transform position);

    public enum ActivationType
    {
        OneTime,
        MultipleTimes,
        Charge
    }
}
