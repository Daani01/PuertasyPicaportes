using UnityEngine;

public interface IUsable
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