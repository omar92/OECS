using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBuildr : MonoBehaviour
{

    public Vector2 mapsize;
    public GameObject MapCell;
    public TransformArray2DVariable mapCellsRef;
    // Use this for initialization

    private GameObject cell;
    void Awake()
    {
        mapCellsRef.value = new Transform[(int)mapsize.x, (int)mapsize.y];
        for (int x = 0; x < mapsize.x; x++)
        {
            for (int z = 0; z < mapsize.y; z++)
            {
                cell = GameObject.Instantiate(MapCell);
                cell.transform.parent = this.transform;
                cell.transform.localPosition = new Vector3(x, 0, z);
                mapCellsRef.value[x, z] = cell.transform;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
