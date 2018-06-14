using UnityEngine;
using UnityEditor;


[CreateAssetMenu(fileName = "TransformArray2DVariable", menuName = "Variables/TransformArray2DVariable", order = 1)]
public class TransformArray2DVariable : ScriptableObject
{
    public Transform[,] value;
    public void SetValue(Transform[,] val)
    {
        value = val;
    }
}
