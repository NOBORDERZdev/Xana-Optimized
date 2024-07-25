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

    internal ItemData itemData;
    internal Renderer m_renderer;    //renderer reference

    #endregion

    #region UNITY_METHOD
    private void OnEnable()
    {
        m_renderer = GetComponentInChildren<Renderer>();
    }
    #endregion

    #region PUBLIC_METHODS
    public void SetData(ItemData itemData)
    {
        //transform.localRotation = itemData.Rotation;
        CollectibleComponentData collectibleComponentData = itemData.collectibleComponentData;
        if (collectibleComponentData.IsActive)
        {
            CollectibleComponent itemComponent = gameObject.AddComponent<CollectibleComponent>();
            itemComponent.Init(collectibleComponentData);

            BuilderEventManager.AddItemComponent?.Invoke(itemComponent);
        }
        RotatorComponentData rotatorComponentData = itemData.rotatorComponentData;
        if (rotatorComponentData.IsActive)
        {
            RotatorComponent itemComponent = gameObject.AddComponent<RotatorComponent>();
            itemComponent.Init(rotatorComponentData, itemData.RuntimeItemID);
            BuilderEventManager.AddItemComponent?.Invoke(itemComponent);
        }
        TimerCountdownComponentData timerCountdownComponentData = itemData.timerCountdownComponentData;
        if (timerCountdownComponentData.IsActive)
        {
            TimerCountdownComponent itemComponent = gameObject.AddComponent<TimerCountdownComponent>();
            itemComponent.Init(timerCountdownComponentData);
            BuilderEventManager.AddItemComponent?.Invoke(itemComponent);
        }
        AddForceComponentData addForceComponentData = itemData.addForceComponentData;
        if (addForceComponentData.isActive)
        {
            AddForceComponent itemComponent = gameObject.AddComponent<AddForceComponent>();
            itemComponent.Init(addForceComponentData);
            BuilderEventManager.AddItemComponent?.Invoke(itemComponent);
        }
        TimerComponentData timerComponentData = itemData.timerComponentData;
        if (timerComponentData.IsActive)
        {
            TimerComponent itemComponent = gameObject.AddComponent<TimerComponent>();
            itemComponent.Init(timerComponentData);
            BuilderEventManager.AddItemComponent?.Invoke(itemComponent);
        }
        TimeLimitComponentData timeLimitComponentData = itemData.timeLimitComponentData;
        if (timeLimitComponentData.IsActive)
        {
            TimeLimitComponent itemComponent = gameObject.AddComponent<TimeLimitComponent>();
            itemComponent.Init(timeLimitComponentData);
            BuilderEventManager.AddItemComponent?.Invoke(itemComponent);
        }
        RandomNumberComponentData randomNumberComponentData = itemData.randomNumberComponentData;
        if (randomNumberComponentData.IsActive)
        {
            RandomNumberComponent itemComponent = gameObject.AddComponent<RandomNumberComponent>();
            itemComponent.Init(randomNumberComponentData);
            BuilderEventManager.AddItemComponent?.Invoke(itemComponent);
        }
        ElapsedTimeComponentData elapsedTimeComponentData = itemData.elapsedTimeComponentData;
        if (elapsedTimeComponentData.IsActive)
        {
            ElapsedTimeComponent itemComponent = gameObject.AddComponent<ElapsedTimeComponent>();
            itemComponent.Init(elapsedTimeComponentData);
            BuilderEventManager.AddItemComponent?.Invoke(itemComponent);
        }
        NarrationComponentData narrationComponentData = itemData.narrationComponentData;
        if (narrationComponentData.IsActive)
        {
            NarrationComponent itemComponent = gameObject.AddComponent<NarrationComponent>();
            itemComponent.Init(narrationComponentData);
            BuilderEventManager.AddItemComponent?.Invoke(itemComponent);
        }
        SituationChangerComponentData situationChangerComponentData = itemData.situationChangerComponentData;
        if (situationChangerComponentData.IsActive)
        {
            SituationChangerComponent itemComponent = gameObject.AddComponent<SituationChangerComponent>();
            itemComponent.Init(situationChangerComponentData);
            BuilderEventManager.AddItemComponent?.Invoke(itemComponent);
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
            itemComponent.InitToFro(toFroComponentData, itemData.RuntimeItemID);
            BuilderEventManager.AddItemComponent?.Invoke(itemComponent);
        }

        TranslateComponentData translateComponentData = itemData.translateComponentData;
        if (translateComponentData.IsActive)
        {
            TranslateComponent itemComponent = gameObject.AddComponent<TranslateComponent>();
            itemComponent.InitTranslate(translateComponentData);
            BuilderEventManager.AddItemComponent?.Invoke(itemComponent);
        }

        ScalerComponentData scalerComponentData = itemData.scalerComponentData;
        if (scalerComponentData.IsActive)
        {
            TransformComponent itemComponent = gameObject.AddComponent<TransformComponent>();
            itemComponent.InitScale(scalerComponentData, itemData.RuntimeItemID);
            BuilderEventManager.AddItemComponent?.Invoke(itemComponent);
        }

        RotateComponentData rotateComponentData = itemData.rotateComponentData;
        if (rotateComponentData.IsActive)
        {
            TransformComponent itemComponent = gameObject.AddComponent<TransformComponent>();
            itemComponent.InitRotate(rotateComponentData, itemData.RuntimeItemID);
            BuilderEventManager.AddItemComponent?.Invoke(itemComponent);
        }

        HelpButtonComponentData helpButtonComponentData = itemData.helpButtonComponentData;
        if (helpButtonComponentData.IsActive)
        {
            HelpButtonComponent itemComponent = gameObject.AddComponent<HelpButtonComponent>();
            itemComponent.Init(helpButtonComponentData);
            BuilderEventManager.AddItemComponent?.Invoke(itemComponent);
        }
        DisplayMessageComponentData displayMessageComponentData = itemData.displayMessageComponentData;
        if (displayMessageComponentData.IsActive)
        {
            DisplayMessagesComponent itemComponent = gameObject.AddComponent<DisplayMessagesComponent>();
            itemComponent.Init(displayMessageComponentData);
            BuilderEventManager.AddItemComponent?.Invoke(itemComponent);
        }
        QuizComponentData quizComponentData = itemData.quizComponentData;
        if (quizComponentData.IsActive)
        {
            QuizComponent itemComponent = gameObject.AddComponent<QuizComponent>();
            itemComponent.Init(quizComponentData);
            BuilderEventManager.AddItemComponent?.Invoke(itemComponent);
        }
        DoorKeyComponentData doorKeyComponentData = itemData.doorKeyComponentData;
        if (doorKeyComponentData.IsActive)
        {
            DoorKeyComponent itemComponent = gameObject.AddComponent<DoorKeyComponent>();
            itemComponent.Init(doorKeyComponentData);
            BuilderEventManager.AddItemComponent?.Invoke(itemComponent);
        }

        WarpFunctionComponentData warpFunctionComponentData = itemData.warpFunctionComponentData;
        if (warpFunctionComponentData.IsActive)
        {
            if (warpFunctionComponentData.isWarpPortalStart || warpFunctionComponentData.isWarpPortalEnd)
            {
                WarpFunctionComponent itemComponent = gameObject.AddComponent<WarpFunctionComponent>();
                itemComponent.Init(warpFunctionComponentData);
                BuilderEventManager.AddItemComponent?.Invoke(itemComponent);
            }
        }

        SpecialItemComponentData specialItemComponentData = itemData.speicalItemComponentData;
        if (specialItemComponentData.IsActive)
        {
            SpecialItemComponent itemComponent = gameObject.AddComponent<SpecialItemComponent>();
            itemComponent.Init(specialItemComponentData);
            BuilderEventManager.AddItemComponent?.Invoke(itemComponent);
        }

        BlindfoldedDisplayComponentData blindfoldedDisplayComponentData = itemData.blindfoldedDisplayComponentData;
        if (blindfoldedDisplayComponentData.IsActive)
        {
            BlindfoldedDisplayComponent itemComponent = gameObject.AddComponent<BlindfoldedDisplayComponent>();
            itemComponent.Init(blindfoldedDisplayComponentData);
            BuilderEventManager.AddItemComponent?.Invoke(itemComponent);
        }

        NinjaComponentData ninjaComponentData = itemData.ninjaComponentData;
        if (ninjaComponentData.IsActive)
        {
            NinjaComponent itemComponent = gameObject.AddComponent<NinjaComponent>();
            itemComponent.Init(ninjaComponentData);
            BuilderEventManager.AddItemComponent?.Invoke(itemComponent);
        }

        ThrowThingsComponentData throwThingsComponentData = itemData.throwThingsComponentData;
        if (throwThingsComponentData.IsActive)
        {
            ThrowThingsComponent itemComponent = gameObject.AddComponent<ThrowThingsComponent>();
            itemComponent.Init(throwThingsComponentData);
            BuilderEventManager.AddItemComponent?.Invoke(itemComponent);
        }

        AudioComponentData audioComponentData = itemData.audioComponentData;
        if (audioComponentData.IsActive)
        {
            if (audioComponentData.audioPath != "")
            {
                AudioComponent itemComponent = gameObject.AddComponent<AudioComponent>();
                itemComponent.Init(audioComponentData);
                BuilderEventManager.AddItemComponent?.Invoke(itemComponent);
            }
        }
        HyperLinkComponentData hyperLinkComponentData = itemData.hyperLinkComponentData;
        if (hyperLinkComponentData.IsActive)
        {
            HyperLinkPopComponent itemComponent = gameObject.AddComponent<HyperLinkPopComponent>();
            itemComponent.Init(hyperLinkComponentData);
            BuilderEventManager.AddItemComponent?.Invoke(itemComponent);
        }

        AvatarChangerComponentData avatarChangerComponentData = itemData.avatarChangerComponentData;
        if (avatarChangerComponentData.IsActive)
        {
            AvatarChangerComponent itemComponent = gameObject.AddComponent<AvatarChangerComponent>();
            itemComponent.InitAvatarChanger(avatarChangerComponentData);
            BuilderEventManager.AddItemComponent?.Invoke(itemComponent);
        }
        BlindComponentData blindComponentData = itemData.blindComponentData;
        if (blindComponentData.IsActive)
        {
            BlindComponent itemComponent = gameObject.AddComponent<BlindComponent>();
            itemComponent.Init(blindComponentData);
            BuilderEventManager.AddItemComponent?.Invoke(itemComponent);
        }
        PhysicsComponentData physicsComponentData = itemData.physicsComponentData;
        if (physicsComponentData.PhysicsComponentIsActive)
        {
            PhysicsComponent itemComponent = gameObject.AddComponent<PhysicsComponent>();
            itemComponent.Init(physicsComponentData);
            BuilderEventManager.AddItemComponent?.Invoke(itemComponent);
        }

        TeleportComponentData teleportComponentData = itemData.teleportComponentData;
        if (teleportComponentData.IsActive)
        {
            TeleportComponent itemComponent = gameObject.AddComponent<TeleportComponent>();
            itemComponent.Init(teleportComponentData);
            BuilderEventManager.AddItemComponent?.Invoke(itemComponent);
        }

        Color color;
        ColorUtility.TryParseHtmlString("#" + itemData.placedMaterialColor, out color);
        itemGFXHandler.SetMaterialColorFromItemData(color);
    }
    #endregion

    #region PRIVATE_METHODS
    #endregion

    #region COROUTINE
    #endregion

    #region DATA
    #endregion
}
