using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;
using System.Linq;

public class XanaItem : MonoBehaviour
{
    #region PUBLIC_VAR

    #endregion

    #region PRIVATE_VAR
    ItemGFXHandler _itemGFXHandler;
    public ItemGFXHandler itemGFXHandler
    {
        get
        {
            if (_itemGFXHandler == null) _itemGFXHandler = gameObject.AddComponent<ItemGFXHandler>();
            return _itemGFXHandler;
        }
    }


    ItemBase _itemBase;
    public ItemBase itemBase
    {
        get
        {
            if (_itemBase == null) _itemBase = GetComponent<ItemBase>();
            return _itemBase;
        }
    }

    #endregion

    #region UNITY_METHOD
    #endregion

    #region PUBLIC_METHODS
    public void SetData(ItemData itemData)
    {
        transform.localScale = itemData.Scale;
        //transform.localRotation = itemData.Rotation;

        CollectibleComponentData collectibleComponentData = itemData.collectibleComponentData;
        if (collectibleComponentData.IsActive)
        {
            CollectibleComponent itemComponent = gameObject.AddComponent<CollectibleComponent>();
            itemComponent.Init(collectibleComponentData);
        }

        RotatorComponentData rotatorComponentData = itemData.rotatorComponentData;
        if (rotatorComponentData.IsActive)
        {
            RotatorComponent itemComponent = gameObject.AddComponent<RotatorComponent>();
            itemComponent.Init(rotatorComponentData);
        }


        TimerCountdownComponentData timerCountdownComponentData = itemData.timerCountdownComponentData;
        if (timerCountdownComponentData.IsActive)
        {
            TimerCountdownComponent itemComponent = gameObject.AddComponent<TimerCountdownComponent>();
            itemComponent.Init(timerCountdownComponentData);
        }

        AddForceComponentData addForceComponentData = itemData.addForceComponentData;
        if (addForceComponentData.isActive)
        {
            AddForceComponent addForceComponent = gameObject.AddComponent<AddForceComponent>();
            addForceComponent.Init(addForceComponentData);
        }

        TimerComponentData timerComponentData = itemData.timerComponentData;
        if (timerComponentData.IsActive)
        {
            TimerComponent itemComponent = gameObject.AddComponent<TimerComponent>();
            itemComponent.Init(timerComponentData);
        }
        TimeLimitComponentData timeLimitComponentData = itemData.timeLimitComponentData;
        if (timeLimitComponentData.IsActive)
        {
            TimeLimitComponent itemComponent = gameObject.AddComponent<TimeLimitComponent>();
            itemComponent.Init(timeLimitComponentData);
        }
        RandomNumberComponentData randomNumberComponentData = itemData.randomNumberComponentData;
        if (randomNumberComponentData.IsActive)
        {
            RandomNumberComponent itemComponent = gameObject.AddComponent<RandomNumberComponent>();
            itemComponent.Init(randomNumberComponentData);
        }
        ElapsedTimeComponentData elapsedTimeComponentData = itemData.elapsedTimeComponentData;
        if (elapsedTimeComponentData.IsActive)
        {
            ElapsedTimeComponent itemComponent = gameObject.AddComponent<ElapsedTimeComponent>();
            itemComponent.Init(elapsedTimeComponentData);
        }
        NarrationComponentData narrationComponentData = itemData.narrationComponentData;
        if (narrationComponentData.IsActive)
        {
            NarrationComponent itemComponent = gameObject.AddComponent<NarrationComponent>();
            itemComponent.Init(narrationComponentData);
        }
        SituationChangerComponentData situationChangerComponentData = itemData.situationChangerComponentData;
        if (situationChangerComponentData.IsActive)
        {
            SituationChangerComponent itemComponent = gameObject.AddComponent<SituationChangerComponent>();
            itemComponent.Init(situationChangerComponentData);
        }

        //HealthComponentData healthComponentData = itemData.healthComponentData;
        //if (healthComponentData.IsActive)
        //{
        //    HealthComponent itemComponent = gameObject.AddComponent<HealthComponent>();
        //    itemComponent.Init(healthComponentData);
        //}

        ToFroComponentData toFroComponentData = itemData.toFroComponentData;
        if (toFroComponentData.IsActive)
        {
            TransformComponent itemComponent = gameObject.AddComponent<TransformComponent>();
            itemComponent.InitToFro(toFroComponentData);
        }

        TranslateComponentData translateComponentData = itemData.translateComponentData;
        if (translateComponentData.IsActive)
        {
            TranslateComponent itemComponent = gameObject.AddComponent<TranslateComponent>();
            itemComponent.InitTranslate(translateComponentData);
        }

        ScalerComponentData scalerComponentData = itemData.scalerComponentData;
        if (scalerComponentData.IsActive)
        {
            TransformComponent itemComponent = gameObject.AddComponent<TransformComponent>();
            itemComponent.InitScale(scalerComponentData);
        }

        RotateComponentData rotateComponentData = itemData.rotateComponentData;
        if (rotateComponentData.IsActive)
        {
            TransformComponent itemComponent = gameObject.AddComponent<TransformComponent>();
            itemComponent.InitRotate(rotateComponentData);
        }

        HelpButtonComponentData helpButtonComponentData = itemData.helpButtonComponentData;
        if (helpButtonComponentData.IsActive)
        {
            HelpButtonComponent itemComponent = gameObject.AddComponent<HelpButtonComponent>();
            itemComponent.Init(helpButtonComponentData);
        }
        DisplayMessageComponentData displayMessageComponentData = itemData.displayMessageComponentData;
        if (displayMessageComponentData.IsActive)
        {
            DisplayMessagesComponent itemComponent = gameObject.AddComponent<DisplayMessagesComponent>();
            itemComponent.Init(displayMessageComponentData);
        }
        QuizComponentData quizComponentData = itemData.quizComponentData;
        if (quizComponentData.IsActive)
        {
            QuizComponent itemComponent = gameObject.AddComponent<QuizComponent>();
            itemComponent.Init(quizComponentData);
        }
        DoorKeyComponentData doorKeyComponentData = itemData.doorKeyComponentData;
        if (doorKeyComponentData.IsActive)
        {
            DoorKeyComponent itemComponent = gameObject.AddComponent<DoorKeyComponent>();
            itemComponent.Init(doorKeyComponentData);
        }

        WarpFunctionComponentData warpFunctionComponentData = itemData.warpFunctionComponentData;
        if (warpFunctionComponentData.IsActive)
        {
            WarpFunctionComponent itemComponent = gameObject.AddComponent<WarpFunctionComponent>();
            itemComponent.Init(warpFunctionComponentData);
        }

        SpecialItemComponentData specialItemComponentData = itemData.speicalItemComponentData;
        if (specialItemComponentData.IsActive)
        {
            SpecialItemComponent itemComponent = gameObject.AddComponent<SpecialItemComponent>();
            itemComponent.Init(specialItemComponentData);
        }

        BlindfoldedDisplayComponentData blindfoldedDisplayComponentData = itemData.blindfoldedDisplayComponentData;
        if (blindfoldedDisplayComponentData.IsActive)
        {
            BlindfoldedDisplayComponent itemComponent = gameObject.AddComponent<BlindfoldedDisplayComponent>();
            itemComponent.Init(blindfoldedDisplayComponentData);
        }

        NinjaComponentData ninjaComponentData = itemData.ninjaComponentData;
        if (ninjaComponentData.IsActive)
        {
            NinjaComponent itemComponent = gameObject.AddComponent<NinjaComponent>();
            itemComponent.Init(ninjaComponentData);
        }

        ThrowThingsComponentData throwThingsComponentData = itemData.throwThingsComponentData;
        if (throwThingsComponentData.IsActive)
        {
            ThrowThingsComponent itemComponent = gameObject.AddComponent<ThrowThingsComponent>();
            itemComponent.Init(throwThingsComponentData);
        }

        //AudioComponentData audioComponentData = itemData.audioComponentData;
        //if (audioComponentData.IsActive)
        //{
        //    AudioComponent itemComponent = gameObject.AddComponent<AudioComponent>();
        //    itemComponent.Init(audioComponentData);
        //}
        HyperLinkComponentData hyperLinkComponentData = itemData.hyperLinkComponentData;
        if (hyperLinkComponentData.IsActive)
        {
            HyperLinkPopComponent itemComponent = gameObject.AddComponent<HyperLinkPopComponent>();
            itemComponent.Init(hyperLinkComponentData);
        }

        Color color;
        ColorUtility.TryParseHtmlString("#" + itemData.placedMaterialColor, out color);
        itemGFXHandler.SetMaterialColorFromItemData(color);

        //CreateBoxCollider();
        //var renderers = transform.GetChild(0).GetComponentsInChildren<Renderer>();
        //var bounds = new Bounds(transform.position, Vector3.zero);
        //foreach (var r in renderers)
        //{
        //    bounds.Encapsulate(r.bounds);
        //}
        //var boxCollider = gameObject.AddComponent<BoxCollider>();
        //var center = boxCollider.center;
        //boxCollider.center = new Vector3(center.x + 1, center.y + bounds.size.y / 2, center.z + 1);
        //boxCollider.size = bounds.size;
        //boxCollider.isTrigger = true;

    }
    #endregion

    #region PRIVATE_METHODS

    private void CreateBoxCollider()
    {
        BoxCollider tempCollider = transform.GetChild(0).gameObject.AddComponent<BoxCollider>();
        Vector3 size = tempCollider.size + Vector3.one;
        Vector3 center = tempCollider.center;
        DestroyImmediate(transform.GetChild(0).GetComponent<BoxCollider>());
        BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.size = size;
        boxCollider.center = center;
        boxCollider.isTrigger = true;
    }
    #endregion

    #region COROUTINE
    #endregion

    #region DATA
    #endregion
}
