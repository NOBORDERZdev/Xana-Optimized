using System;
using System.Text;
using UnityEngine;

public class ItemBase : MonoBehaviour
{
    public uString assetId;
    public uString assetName;
    public uString categoryId;
    public uString subCategoryId;
    public uString materialId;
    public uString thumbnailId;
    public uString textureId;

    public void Init()
    {
        //if(name.Contains("Spawn"))
        //{
        //    SpawnPointInit();
        //    return;
        //}

        assetId.Value = name.Substring(2, 10);
        assetName.Value = "";/*GameManager.Instance.itemList.GetAssetName(assetId.Value);*/
        categoryId.Value = assetId.Value.Substring(0, 3);
        subCategoryId.Value = assetId.Value.Substring(3, 2);
        StringBuilder sb = new StringBuilder();
        sb.Append(Constants.materialPrefix);
        sb.Append(assetId.Value);
        //sb.Append(Constants.materialExt);
        materialId.Value = sb.ToString();
        sb.Clear();
        sb.Append(assetId.Value);
        sb.Append(Constants.thumbnailSuffix);
        thumbnailId.Value = sb.ToString();
        sb.Clear();
        sb.Append(Constants.texturePrefix);
        sb.Append(assetId.Value);
        sb.Append(Constants.TextureType._Diffuse);
        sb.Append(Constants.textureExt);
        textureId.Value = sb.ToString();
        sb.Clear();

        //if(GameManager.Instance != null)
        //{
        //    GameManager.Instance.UpdateItemCatSubCatList(categoryId.Value, subCategoryId.Value);
        //}
    }

    public void InitComponents()
    {
        ItemComponent[] itemComponents = GetComponents<ItemComponent>();
        for (int i = 0; i < itemComponents.Length; i++)
        {
            itemComponents[i].Init();
        }
    }
}
[RequireComponent(typeof(ItemBase))]
public abstract class ItemComponent : MonoBehaviour, IComponentBehaviour
{
    [SerializeField]
    private ItemBase _itemBase;
    protected Constants.ItemComponentType _componentType;
    Constants.ItemComponentType IComponentBehaviour.ComponentType => _componentType;

    public ItemBase ItemBase
    {
        get
        {
            return _itemBase;
        }
        set
        {
            _itemBase = value;
        }
    }

    public GameObject[] activeObjects;
    public Behaviour[] activeBehaviours;
    public bool activeOnlyIfEnabled;

    bool init = false;
    bool activated = false;
    protected bool isPlaying = false;

    public string assetId
    {
        get
        {
            if (ItemBase == null) return string.Empty;
            return ItemBase.assetId.Value;
        }
    }

    public Constants.ItemComponentType ComponentType => throw new NotImplementedException();

    public bool CompareAssetId(string id)
    {
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(ItemBase.assetId.Value)) return false;
        return ItemBase.assetId.Value.Equals(id);
    }

    public virtual void Init()
    {
        if (ItemBase == null) ItemBase = GetComponent<ItemBase>();
        OnInit();
        AssignItemComponentType();
    }
    public virtual void OnInit() { }

    public void Activate()
    {
        if (init && activated) return;
        init = true;
        activated = true;
        if (activeObjects != null) activeObjects.SetActiveCollection(true);
        activeBehaviours.ForEachItem((b) => { b.enabled = true; });
        if (!enabled) enabled = true;
        OnActivate();
    }
    public virtual void OnActivate() { }

    public void RecallOnActivate()
    {
        new Delayed.Action(OnActivate, .1f);
    }

    public void Deactivate()
    {
        if (init && !activated) return;
        init = true;
        activated = false;
        if (enabled) enabled = false;
        if (activeObjects != null) activeObjects.SetActiveCollection(false);
        if (activeBehaviours != null) activeBehaviours.ForEachItem((b) => { if (b != null) b.enabled = false; });
        OnDeactivate();
    }

    public virtual void OnDeactivate() { }

    protected virtual void Awake()
    {
        if (ItemBase == null) ItemBase = GetComponent<ItemBase>();
        Init();
    }

    protected virtual void OnDestroy()
    {
        Deactivate();
    }

    void OnEnable()
    {
        if (activeOnlyIfEnabled) Activate();
    }

    void OnDisable()
    {
        if (activeOnlyIfEnabled) Deactivate();
    }


    public virtual void OnValidate()
    {
        if (ItemBase == null) ItemBase = GetComponent<ItemBase>();
    }

    protected T TryGetComponent<T>(Transform tr) where T : Component
    {
        try
        {
            return tr.GetComponent<T>();
        }
        catch (Exception e) { print(e.Message); }
        return null;
    }
    protected T TryFindComponentInChildren<T>(string goName, Transform parent) where T : Component
    {
        try
        {
            T[] comps = parent.GetComponentsInChildren<T>(true);
            foreach (T comp in comps)
            {
                if (comp.name.Equals(goName)) return comp;
            }
        }
        catch (Exception e) { print(e.Message); }
        return null;
    }

    public virtual void PlayBehaviour() { }

    public virtual void StopBehaviour() { }

    public virtual void ResumeBehaviour() { }

    public virtual void ToggleBehaviour() { }

    public abstract void AssignItemComponentType();

    public abstract void CollisionExitBehaviour();
    public abstract void CollisionEnterBehaviour();

}