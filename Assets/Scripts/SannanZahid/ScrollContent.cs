using UnityEngine;
using System.Collections.Generic;

namespace DynamicScrollRect
{
    public class ScrollContent : MonoBehaviour
    {
       /* public Vector2 Spacing = Vector2.zero;
        [Min(1)][SerializeField] private int _fixedItemCount = 1;
        public int TotalItems = 0;
        private DynamicScrollRect _dynamicScrollRect;
        public DynamicScrollRect DynamicScrollRect
        {
            get
            {
                if (_dynamicScrollRect == null)
                {
                    _dynamicScrollRect = GetComponent<DynamicScrollRect>();
                }
                return _dynamicScrollRect;
            }
        }
        private WorldItemView _referenceItem;
        private WorldItemView _ReferenceItem
        {
            get
            {
                if (_referenceItem == null)
                {
                    _referenceItem = GetComponentInChildren<WorldItemView>();
                }
                return _referenceItem;
            }
        }
        [SerializeField]
        List<WorldItemDetail> Worlds = new List<WorldItemDetail>();
        private List<WorldItemView> _activatedItems = new List<WorldItemView>();
        private List<WorldItemView> _deactivatedItems = new List<WorldItemView>();
        public float ItemWidth = 344f;
        public float ItemHeight = 344f;
        [SerializeField]
        private float _screenSizeX = 1080;
        float AlignSpace = default;
        string CurrentKey;
        private void Awake()
        {
            AlignSpace = (_screenSizeX - (ItemWidth * _fixedItemCount)) / 3.5f;
            _ReferenceItem.gameObject.SetActive(false);
        }
        public void InitScrollContent(string worldKey ,List<WorldItemDetail> contentDatum)
        {
            if (Worlds.Count.Equals(0))
            {
                Worlds = contentDatum;
                CurrentKey = worldKey;
                InitItemsVertical(Worlds.Count);
            }
            else if(worldKey != CurrentKey)
            {
                ClearContent();
                Worlds = contentDatum;
                CurrentKey = worldKey;
                InitItemsVertical(Worlds.Count);
                DynamicScrollRect.content.anchoredPosition = Vector2.zero;
            }
            else if(worldKey.Equals(APIURL.SearchWorld.ToString()))
            {
                ClearContent();
                Worlds = contentDatum;
                CurrentKey = worldKey;
                InitItemsVertical(Worlds.Count);
                DynamicScrollRect.content.anchoredPosition = Vector2.zero;
            }
            else if(worldKey.Equals(APIURL.SearchWorldByTag.ToString()))
            {
                ClearContent();
                Worlds = contentDatum;
                CurrentKey = worldKey;
                InitItemsVertical(Worlds.Count);
                DynamicScrollRect.content.anchoredPosition = Vector2.zero;
            }
            switch(RestrictState)
            {
                case 0:
                    {
                        if (GameManager.Instance.UiManager.PreviousScreen == 0 && (Worlds.Count > 0 && Worlds.Count < 5))
                        {
                            Debug.LogError("here locking");
                            DynamicScrollRect.RestrictFlag = true;
                            DynamicScrollRect.TopScroller.verticalNormalizedPosition = 1f;
                            RestrictState = 1;
                        }
                        break;
                    }
                case 1:
                    {
                        if (GameManager.Instance.UiManager.PreviousScreen == 0 && (Worlds.Count > 5))
                        {
                            DynamicScrollRect.RestrictFlag = false;
                            DynamicScrollRect.TopScroller.vertical = true;
                            RestrictState = 0;
                           
                        }
                        else if (GameManager.Instance.UiManager.PreviousScreen == 1)
                        {
                            DynamicScrollRect.RestrictFlag = false;
                            RestrictState = 0;
                        }
                        break;
                    }
            }
            DynamicScrollRect.velocity = Vector2.zero;
        }
        int RestrictState = 0;
        private void InitItemsVertical(int count)
        {
            int itemCount = 0;
            Vector2Int initialGridSize = CalculateInitialGridSize();
            for (int col = 0; col < initialGridSize.y; col++)
            {
                for (int row = 0; row < initialGridSize.x; row++)
                {
                    if (itemCount == count)
                    {
                        if (count == 0)
                        {
                            //LoadingHandler.Instance.SearchLoadingCanvas.SetActive(false);
                            LoadingHandler.Instance.worldLoadingScreen.SetActive(false);
                        }
                        return;
                    }
                    ActivateItem(itemCount, count);
                    itemCount++;
                }
            }
        }
        private Vector2Int CalculateInitialGridSize()
        {
            return new Vector2Int(_fixedItemCount, 9);
        }
        private WorldItemView ActivateItem(int itemIndex, int _loopcount=0)
        {
            Vector2 gridPos = GetGridPosition(itemIndex);
            Vector2 anchoredPos = GetAnchoredPosition(gridPos);
            WorldItemView scrollItem = null;
            if (_deactivatedItems.Count == 0)
            {
                scrollItem = CreateNewScrollItem();
            }
            else
            {
                scrollItem = _deactivatedItems[0];
                _deactivatedItems.Remove(scrollItem);
            }
            scrollItem.gameObject.SetActive(true);
            scrollItem.gameObject.name = $"{gridPos.x}_{gridPos.y}";
            scrollItem.RectTransform.anchoredPosition = anchoredPos;
            scrollItem.InitItem(itemIndex, gridPos, Worlds[itemIndex],_loopcount);
            bool insertHead = (_activatedItems.Count == 0 ||
                               (_activatedItems.Count > 0 && _activatedItems[0].Index > itemIndex));

            if (insertHead)
            {
                _activatedItems.Insert(0, scrollItem);
            }
            else
            {
                _activatedItems.Add(scrollItem);
            }
            return scrollItem;
        }
        private Vector2 GetGridPosition(int itemIndex)
        {
            int col = itemIndex / _fixedItemCount;
            int row = itemIndex - (col * _fixedItemCount);
            return new Vector2(row, col);
        }
        private Vector2 GetAnchoredPosition(Vector2 gridPosition)
        {
            return new Vector2(
                AlignSpace + (gridPosition.x * ItemWidth) + (gridPosition.x * Spacing.x),
                (-gridPosition.y * ItemHeight) - (gridPosition.y * Spacing.y));
        }
        private WorldItemView CreateNewScrollItem()
        {
            GameObject item = Instantiate(_ReferenceItem.gameObject, DynamicScrollRect.content);
            WorldItemView scrollItem = item.GetComponent<WorldItemView>();
            scrollItem.RectTransform.pivot = new Vector2(0, 1);
            return scrollItem;
        }
        public void ClearContent()
        {
            List<WorldItemView> activatedItems = new List<WorldItemView>(_activatedItems);
            foreach (WorldItemView item in activatedItems)
            {
                DeactivateItem(item);
            }
        }
        public bool CanAddNewItemIntoTail()
        {
            if (_activatedItems == null || _activatedItems.Count == 0)
            {
                return false;
            }
            return _activatedItems[_activatedItems.Count - 1].Index < TotalItems - 1;
        }
        public bool CanAddNewItemIntoHead()
        {
            if (_activatedItems == null || _activatedItems.Count == 0)
            {
                return false;
            }
            return _activatedItems[0].Index - 1 >= 0;
        }
        public Vector2 GetFirstItemPos()
        {
            if (_activatedItems.Count == 0)
            {
                return Vector2.zero;
            }
            return _activatedItems[0].RectTransform.anchoredPosition;
        }
        public Vector2 GetThirdItemPos()
        {
            if (_activatedItems.Count == 0)
            {
                return Vector2.zero;
            }
            return _activatedItems[4].RectTransform.anchoredPosition;
        }
        public Vector2 GetLastItemPos()
        {
            if (_activatedItems.Count == 0)
            {
                return Vector2.zero;
            }
            return _activatedItems[_activatedItems.Count - 1].RectTransform.anchoredPosition;
        }
        public void AddIntoHead()
        {
            for (int i = 0; i < _fixedItemCount; i++)
            {
                AddItemToHead();
            }
        }
        public void AddIntoTail()
        {
            for (int i = 0; i < _fixedItemCount; i++)
            {
                AddItemToTail();
            }
        }
        public void DeleteFromHead()
        {
            int firstRowIndex = (int) _activatedItems[0].GridIndex.y;
            DeleteRow(firstRowIndex);
        }
        public void DeleteFromTail()
        {
            int lastRowIndex = (int) _activatedItems[_activatedItems.Count - 1].GridIndex.y;
            DeleteRow(lastRowIndex);
        }
        private void DeleteRow(int rowIndex)
        {
            List<WorldItemView> items = _activatedItems.FindAll(i => (int) i.GridIndex.y == rowIndex);
            foreach (WorldItemView item in items)
            {
                DeactivateItem(item);
            }
        }
        int previousItems = 0;
        private void AddItemToTail()
        {
            if (!CanAddNewItemIntoTail())
            {
                //Debug.LogError("Can't add new item into tail");
                if (WorldManager.instance.dataIsFatched)
                {
                    WorldManager.instance.WorldPageLoading();
                }
                return;
            }
            int itemIndex = _activatedItems[_activatedItems.Count - 1].Index + 1;
            if (itemIndex == TotalItems)
            {
                return;
            }
            ActivateItem(itemIndex);
            if(itemIndex >= (int)(TotalItems *.75) && TotalItems > previousItems)
            {
                previousItems = TotalItems;
                //Debug.LogError("Fetch data again");
                if (WorldManager.instance.dataIsFatched)
                {
                    WorldManager.instance.WorldPageLoading();
                }
            }
        }
        private void AddItemToHead()
        {
            if (!CanAddNewItemIntoHead())
            {
                return;
            }
            int itemIndex = _activatedItems[0].Index - 1;
            if (itemIndex < 0)
            {
                return;
            }
            ActivateItem(itemIndex);
        }
        private void DeactivateItem(WorldItemView item)
        {
            _activatedItems.Remove(item);
            _deactivatedItems.Add(item);
            item.gameObject.SetActive(false);
        }
        public void ResetContent()
        {
            ClearContent();
            InitItemsVertical(Worlds.Count);
        }*/
    }
}