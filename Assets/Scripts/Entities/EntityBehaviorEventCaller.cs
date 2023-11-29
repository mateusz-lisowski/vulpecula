using UnityEngine;

public class EntityBehaviorEventCaller : MonoBehaviour
{
    public EntityBehaviorController controller;

    public void callEvent(string name)
    {
        controller.onEvent(name, null);
    }
}
