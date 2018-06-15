using UnityEngine;

namespace OECS
{
    public abstract class EntitySystem : MonoBehaviour
    {
        public EntitiesGroup entitiesGroup;


        //void Awake()
        //{
        //    OnSystemAwake();
        //    entitiesGroup.OnAwake(OnEntityAwake);
        //}

        //void Start()
        //{
        //    OnSystemStart();
        //    entitiesGroup.OnStart(OnEntityStart);
        //}
        //void Update()
        //{
        //    OnSystemUpdate();
        //    entitiesGroup.OnUpdate(OnEntityUpdate);
        //}


        //public abstract void OnSystemAwake();
        //public abstract void OnSystemStart();
        //public abstract void OnSystemUpdate();


        //public abstract void OnEntityAwake(Entity entity);
        //public abstract void OnEntityStart(Entity entity);
        //public abstract void OnEntityUpdate(Entity entity);
    }
}