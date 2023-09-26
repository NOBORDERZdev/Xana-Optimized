using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ItemsManager : MonoBehaviour
{
    [SerializeField] private AppliedComponentRestrictions _appliedComponentRestricitions;
    [SerializeField] List<ComponentData> _itemComponents = new List<ComponentData>();

    private void OnEnable()
    {
        BuilderEventManager.onComponentActivated += OnComponentActivated;
        BuilderEventManager.AddItemComponent += AddItemComponent;

        _itemComponents.Clear();
    }

    private void OnDisable()
    {
        BuilderEventManager.onComponentActivated -= OnComponentActivated;
        BuilderEventManager.AddItemComponent -= AddItemComponent;
    }


    void AddItemComponent(IComponentBehaviour componentBehaviour)
    {
        ComponentData componentData = new ComponentData();
        componentData.componentType = componentBehaviour.ComponentType;
        componentData.componentBehaviour = componentBehaviour;

        if (_itemComponents.Contains(componentData))
            return;

        _itemComponents.Add(componentData);
    }

    private void StopItemComponents(List<Constants.ItemComponentType> notAllowedComponents)
    {
        foreach (ComponentData itemComponent in _itemComponents)
        {
            if (notAllowedComponents.Contains(itemComponent.componentType))
            {
                itemComponent.componentBehaviour.StopBehaviour();
            }
        }
    }

    private void OnComponentActivated(Constants.ItemComponentType componentType)
    {
        var itemComponentRestriction = _appliedComponentRestricitions.restrictionList.FirstOrDefault(rl => rl.componentType == componentType);

        if (itemComponentRestriction == null) return;

        List<Constants.ItemComponentType> notAllowedComponents = itemComponentRestriction.notAllowedComponents.ToList();
        notAllowedComponents.Add(itemComponentRestriction.componentType);

        StopItemComponents(notAllowedComponents);
    }
}

[Serializable]
public class ComponentData
{
    public Constants.ItemComponentType componentType;
    public IComponentBehaviour componentBehaviour;
}

[Serializable]
public class AllComponents
{
    public List<ComponentData> _itemComponents;
}