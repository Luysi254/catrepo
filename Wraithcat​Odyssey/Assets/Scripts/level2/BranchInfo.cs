using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BranchData
{
    public Vector3 originalPosition;
    public Color originalColor;
}

public class BranchInfo : MonoBehaviour
{
    public BranchData data;
    public bool isPlaced = false;
}
