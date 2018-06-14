using UnityEngine;
using System.Collections;
using OECS;
using System;

public class SeaSystem : OECS.System
{

    public TransformArray2DVariable mapCellsRef;
    public float transfareRatio = .1f;
    //entity data
    Transform m_EntityTransform;
    Vector3 m_EntityPosition;
    int m_EntityX;
    int m_EntityZ;
    float m_EntityHeight;

    // mapData
    Vector2 m_mapSize;

    //ScannerData
    Vector3 m_ScanedPos;

    void Update()
    {
        entitiesGroup.ExcuteInEntities(OnEntityUpdate);
        m_mapSize = new Vector2(mapCellsRef.value.GetUpperBound(0), mapCellsRef.value.GetUpperBound(1));
    }

    public void OnEntityUpdate(Entity entity)
    {
        m_EntityTransform = entity.transform;
        m_EntityPosition = m_EntityTransform.position;
        m_EntityX = (int)m_EntityTransform.position.x;
        m_EntityZ = (int)m_EntityTransform.position.z;
        m_EntityHeight = m_EntityTransform.localScale.y;


        m_ScanedPos = m_EntityPosition + Vector3.forward;
        if (m_ScanedPos.z < m_mapSize.y)
        {
            TransfareHeight(m_EntityTransform, mapCellsRef.value[m_EntityX, m_EntityZ + 1]);
        }

        m_ScanedPos = m_EntityPosition + Vector3.back;
        if (m_ScanedPos.z >= 0)
        {
            TransfareHeight(m_EntityTransform, mapCellsRef.value[m_EntityX, m_EntityZ - 1]);
        }

        m_ScanedPos = m_EntityPosition + Vector3.right;
        if (m_ScanedPos.x < m_mapSize.x)
        {
            TransfareHeight(m_EntityTransform, mapCellsRef.value[m_EntityX + 1, m_EntityZ]);
        }

        m_ScanedPos = m_EntityPosition + Vector3.left;
        if (m_ScanedPos.x >= 0)
        {
            TransfareHeight(m_EntityTransform, mapCellsRef.value[m_EntityX - 1, m_EntityZ]);
        }

    }

    private void TransfareHeight(Transform Entity, Transform Neighpor)
    {
        if (Neighpor.localScale.y < m_EntityHeight)
        {
            Neighpor.localScale += Vector3.up* transfareRatio;
            m_EntityTransform.localScale += Vector3.down * transfareRatio;
        }
    }
}
