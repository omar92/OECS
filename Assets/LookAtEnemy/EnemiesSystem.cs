
using System.Collections;
using UnityEngine;

public class EnemiesSystem : OECS.EntitySystem
{

    public TransformVariable Target;


    TankEntity tank;
    Coroutine co;
    void Update()
    {
        if (co == null)
            co = entitiesGroup.ExcuteOnEntitiesCo((entity) =>
            {
                tank = (TankEntity)entity;
                tank.Head.LookAt(Target.value);
            //  Debug.DrawLine(tank.Head.position, Target.value.position,Color.red);
        }, this, true);

    }
}