using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

namespace OECS
{
    [CreateAssetMenu(fileName = "EntitiesGroup", menuName = "ECS/EntitiesGroup", order = 0)]
    public class EntitiesGroup : ScriptableObject
    {
        List<Entity> m_List = new List<Entity>();

        private Entity m_Entity;

        public void ExcuteInEntities(Action<Entity> action)
        {
            for (int i = m_List.Count - 1; i >= 0; i--)
            {
                action(m_List[i]);
            }
        }

        public Coroutine ExcuteOnEntitiesCo(Action<Entity> action, EntitySystem sysInistance, bool IsLoop)
        {
            return sysInistance.StartCoroutine(EntityCO(action, IsLoop));
        }


        IEnumerator EntityCO(Action<Entity> action, bool IsLoop)
        {
            do
            {
                var time = Time.time;
                //Debug.Log("TIme:" + time);
                for (int i = m_List.Count - 1; i >= 0; i--)
                {
                  //  Debug.Log("Time:" + time);
                  //  Debug.Log("Time:" + Time.time);
                 //   Debug.Log("Time.time - time:" + (Time.time - time));
                 //   Debug.Log("Time.deltaTime" + (Time.deltaTime));
                    if (Time.time - time < Time.fixedTime)
                    {
                        action(m_List[i]);
                    }
                    else
                    {
                        yield return new WaitForEndOfFrame();
                        time = Time.time;
                        i++;
                    }
                }
                yield return new WaitForEndOfFrame();
            } while (IsLoop);
        }

        public void RegisterListener(Entity entity)
        {
            if (!m_List.Contains(entity)) m_List.Add(entity);
        }
        public void UnregisterListener(Entity entity)
        {
            if (m_List.Contains(entity)) m_List.Remove(entity);
        }

        public Entity GetEntity(int i)
        {
            return m_List[i];
        }

    }
}