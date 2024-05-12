using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightPlacementCollider : MonoBehaviour
{
    // Platform index from 1 to 6 where 1 is the leftmost platform
    [SerializeField] private int platformIndex = 0;

    public int GetPlatformIndex()
    {
        return platformIndex;
    }
}
