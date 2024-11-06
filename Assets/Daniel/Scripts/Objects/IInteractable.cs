using UnityEngine;

public interface IInteractable
{
    void InteractObj();
}

public interface IUsable : IInteractable
{
    void Use();
    void GetObjPlayer(Transform position);
    void Activate();
    void DesActivate();

    public enum ActivationType
    {
        OneTime,
        MultipleTimes,
        Charge
    }
}
