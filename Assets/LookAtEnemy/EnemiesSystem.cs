using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesSystem : OECS.System
{

    public TransformVariable Target;


    TankEntity tank;
    void Update()
    {
        entitiesGroup.ExcuteInEntities((entity) =>
        {
            tank = (TankEntity)entity;
            tank.Head.LookAt(Target.value);

        });

    }
}
