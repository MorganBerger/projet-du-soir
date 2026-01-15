using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CuttingPositions", menuName = "Scriptable Objects/CuttingPositions")]
public class CuttingPositions : ScriptableObject
{
    public List<Vector3> positions;
}
