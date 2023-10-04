using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Applied Component Restriction", menuName = "ScriptableObjects/AppliedComponentRestriction", order = 2)]
public class AppliedComponentRestrictions : ScriptableObject
{
    public List<ItemComponentRestriction> restrictionList;
}

[System.Serializable]
public class ItemComponentRestriction
{
    public Constants.ItemComponentType componentType;
    public List<Constants.ItemComponentType> notAllowedComponents;
}