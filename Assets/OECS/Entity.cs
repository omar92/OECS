using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace OECS
{
    public abstract class Entity : MonoBehaviour
    {
        public List<EntitiesGroup> EntitiesGroups = new List<EntitiesGroup>();

        internal void OnEnable()
        {
            foreach (var group in EntitiesGroups)
            {
                group.RegisterListener(this);
            }
        }
        internal void OnDisable()
        {
            foreach (var group in EntitiesGroups)
            {
                group.UnregisterListener(this);
            }
        }

    }
}
