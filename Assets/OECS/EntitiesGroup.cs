using UnityEngine;
using System.Collections.Generic;
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

        public void RegisterListener(Entity entity)
        {
            if (!m_List.Contains(entity)) m_List.Add(entity);
        }
        public void UnregisterListener(Entity entity)
        {
            if (m_List.Contains(entity)) m_List.Remove(entity);
        }

    }
}