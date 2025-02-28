using UnityEngine;

public interface IInteractable
{
    void InteractObj();
}

public interface IUsable : IInteractable
{
    void Use();
    void GetObjPlayer(Transform position, Transform lookat);
    void Activate();
    void DesActivate();
    void Destroy();

    bool Energy();
    float getEnergy();
    float getMaxEnergy();

    ItemType GetName();

    public enum ActivationType
    {
        OneTime,
        MultipleTimes,
        Charge
    }
}
