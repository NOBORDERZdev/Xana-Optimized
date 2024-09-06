// -------------------------------------------
// Control Freak 2
// Copyright (C) 2013-2021 Dan's Game Tools
// http://DansGameTools.blogspot.com
// -------------------------------------------

//#define DRAW_DEBUG_GUI

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

using ControlFreak2.Internal;

namespace ControlFreak2
{
// -------------------
//! Touch Button class.
// -------------------
public class TouchButton : DynamicTouchControl, IBindingContainer
	{
//! \cond

	public enum ToggleOnAction
		{
		OnPress,
		OnRelease 
		}

	public enum ToggleOffAction
		{
		OnPress,
		OnRelease,
		OnTimeout
		}



	public bool				toggle;
	public ToggleOnAction	toggleOnAction;
	public ToggleOffAction	toggleOffAction;
	public bool				toggleOffWhenHiding;
	public bool 			autoToggleOff;
	public float			autoToggleOffTimeOut;
	
	public bool 			linkToggleToRigSwitch;
	public string			toggleRigSwitchName;
	private int				toggleRigSwitchId;

		
//! \cond 

	public DigitalBinding		
		pressBinding;
	public DigitalBinding
		toggleOnlyBinding;

	public bool
		emulateTouchPressure;
	public AxisBinding
		touchPressureBinding;

//! \endcond 


	private bool	
		toggledCur,
		toggledPrev,
		curTouchToggledOn;
		
	private float
		elapsedSinceToggled;
	
//! \endcond


	// ---------------------
	public TouchButton() : base()
		{
		this.pressBinding		= new DigitalBinding();
		this.toggleOnlyBinding	= new DigitalBinding();
		
		this.emulateTouchPressure = true;
		this.touchPressureBinding = new AxisBinding();


		this.centerWhenFollowing = true;

		this.toggleOnAction 	= ToggleOnAction.OnPress;
		this.toggleOffAction	= ToggleOffAction.OnRelease;

		this.autoToggleOff 			= false;
		this.autoToggleOffTimeOut	= 1.0f;

		}
		
	// ----------------
	//public bool Pressed()						{ return this.touchStateWorld.PressedRaw(); }
	//public bool JustPressed()					{ return this.touchStateWorld.JustPressedRaw(); }
	//public bool JustReleased()					{ return this.touchStateWorld.JustReleasedRaw(); }
		
	public bool	Toggled()						{ return (this.toggledCur); }
	public bool	JustToggled()					{ return (this.toggledCur && !this.toggledPrev); }
	public bool	JustUntoggled()					{ return (!this.toggledCur && this.toggledPrev); }
		
	public bool PressedOrToggled()				{ return (this.Pressed() || this.Toggled()); }


//! \cond

	// -----------------
	override protected void OnInitControl()
		{

		base.OnInitControl();

		if (this.toggle && this.linkToggleToRigSwitch && (this.rig != null))
			this.ChangeToggleState(this.rig.GetSwitchState(this.toggleRigSwitchName, ref this.toggleRigSwitchId, false), false);
			
		this.ResetControl();

		}
		


	// ------------------
	override public void ResetControl()
		{
		base.ResetControl();

		this.ReleaseAllTouches(); //true);
			
		this.touchStateWorld.Reset();
		this.touchStateScreen.Reset();
		this.touchStateOriented.Reset();
		
		}


	// ----------------
	override protected void OnUpdateControl()
		{

#if UNITY_EDITOR
		if (CFUtils.editorStopped)
			return;
#endif

		base.OnUpdateControl();

		
		this.toggledPrev = this.toggledCur;


		// Toggle logic...

		if (!this.toggle)
			{
			this.toggledCur = false;
			}
		else
			{
			bool changeToggleState = false;
				
			// Toggle ON?

			if (!this.Toggled())
				{
				if (this.toggleOnAction == ToggleOnAction.OnPress)
					{
					if (this.touchStateWorld.JustPressedRaw())
						{
						changeToggleState = true;
						this.curTouchToggledOn = true;
						}
					}

				else if (this.toggleOnAction == ToggleOnAction.OnRelease)
					{
					if (!this.curTouchToggledOn && this.touchStateWorld.JustReleasedRaw())
						{
						changeToggleState = true;
						this.curTouchToggledOn = false;
						}
					}
				}

			// Toggle OFF?

			else
				{
				if (this.toggleOffAction == ToggleOffAction.OnPress)
					{
					if (this.touchStateWorld.JustPressedRaw())
						{
						changeToggleState = true;
						this.curTouchToggledOn = true;
						}
					}

				else if (this.toggleOffAction == ToggleOffAction.OnRelease)
					{
					if (!this.curTouchToggledOn && this.touchStateWorld.JustReleasedRaw())
						{
						changeToggleState = true;
						this.curTouchToggledOn = false;
						}				
					}
				}


			if (!this.touchStateWorld.PressedRaw())
				this.curTouchToggledOn = false;
					

			if (changeToggleState)
				{
				this.ChangeToggleState(!this.toggledCur, true);
				}
			else
				{
				if (this.toggle && this.linkToggleToRigSwitch)
					this.ChangeToggleState(this.rig.rigSwitches.GetSwitchState(this.toggleRigSwitchName, ref this.toggleRigSwitchId, this.toggledCur), false);
				}


			if (this.Toggled() && (this.autoToggleOff || (this.toggleOffAction == ToggleOffAction.OnTimeout)))
				{
				if (this.Pressed())
					this.elapsedSinceToggled = 0;

				else if ((this.elapsedSinceToggled += CFUtils.realDeltaTime) > this.autoToggleOffTimeOut)
					this.ChangeToggleState(false, true);
				}			
			}

			
			
		if (this.IsActive())
			this.SyncRigState();		
		}
		


	// ---------------
	public void ChangeToggleState(bool toggleState, bool syncRigSwitch = true)
		{
		this.toggledCur				= toggleState;
		this.elapsedSinceToggled	= 0;
		
		if (syncRigSwitch && this.linkToggleToRigSwitch && (this.rig != null))
			{
			this.rig.rigSwitches.SetSwitchState(this.toggleRigSwitchName, ref this.toggleRigSwitchId, toggleState);
			}
		}


	// ---------------------
	private void SyncRigState()
		{
		if (this.PressedOrToggled())
			this.pressBinding.Sync(true, this.rig);

		this.toggleOnlyBinding.Sync(this.Toggled(), this.rig);


		if (this.Pressed())
			{
			if (this.IsTouchPressureSensitive())
				this.touchPressureBinding.SyncFloat(this.GetTouchPressure(), InputRig.InputSource.Analog, this.rig);

			else if (this.emulateTouchPressure)	
				this.touchPressureBinding.SyncFloat(1, InputRig.InputSource.Digital, this.rig);			
			}
		}




	// ----------------------
	override protected void OnGetSubBindingDescriptions(BindingDescriptionList descList,
		Object undoObject, string parentMenuPath) 
		{
		descList.Add(this.pressBinding, 	"Press", parentMenuPath, this);
		descList.Add(this.touchPressureBinding, InputRig.InputSource.Analog, "Touch Pressure", parentMenuPath, this);
 


		if (this.toggle || descList.addUnusedBindings)
			descList.Add(this.toggleOnlyBinding, "Toggle", parentMenuPath, this);

		}



	
	// ---------------------
	override protected bool OnIsBoundToAxis(string axisName, InputRig rig)
		{
		return (
			this.pressBinding.IsBoundToAxis(axisName, rig) ||
			this.touchPressureBinding.IsBoundToAxis(axisName, rig) ||
			this.toggleOnlyBinding.IsBoundToAxis(axisName, rig) );
		}


	// ----------------------
	override protected bool OnIsBoundToKey(KeyCode key, InputRig rig)
		{
		return (
			this.pressBinding.IsBoundToKey(key, rig) ||
			this.touchPressureBinding.IsBoundToKey(key, rig) ||
			this.toggleOnlyBinding.IsBoundToKey(key, rig) );
		}




	// ------------------
	override public void ReleaseAllTouches() //TouchEndType touchEndType) //bool cancel)
		{
		base.ReleaseAllTouches();


		this.ChangeToggleState(false, this.toggleOffWhenHiding);

		}

		public void SetupNewInits(string buttonID) {
				if (this.initialRectCopy == null)
					return;
			if (buttonID == "JOYSTICK")
			{
				this.GetComponent<RectTransform>().anchoredPosition = new Vector3(PlayerPrefs.GetFloat("joyX", 149f), PlayerPrefs.GetFloat("joyY", 75.90f), 0);
				this.transform.localScale = new Vector3(PlayerPrefs.GetFloat("joyScaleX", 1), PlayerPrefs.GetFloat("joyScaleY", 1), 0);
			}
			if (buttonID == "LP")
			{
				this.GetComponent<RectTransform>().anchoredPosition = new Vector3(PlayerPrefs.GetFloat("LPX", -116.40f), PlayerPrefs.GetFloat("LPY", 144.90f), 0);
				this.transform.localScale = new Vector3(PlayerPrefs.GetFloat("LPScaleX", 1), PlayerPrefs.GetFloat("LPScaleY", 1), 0);
			}
			if (buttonID == "HP")
			{
				this.GetComponent<RectTransform>().anchoredPosition = new Vector3(PlayerPrefs.GetFloat("HPX", -231.70f), PlayerPrefs.GetFloat("HPY", 144.90f), 0);
				this.transform.localScale = new Vector3(PlayerPrefs.GetFloat("HPScaleX", 1), PlayerPrefs.GetFloat("HPScaleY", 1), 0);
			}
			if (buttonID == "LK")
			{
				this.GetComponent<RectTransform>().anchoredPosition = new Vector3(PlayerPrefs.GetFloat("LKX", -231.70f), PlayerPrefs.GetFloat("LKY", 144.90f), 0);
				this.transform.localScale = new Vector3(PlayerPrefs.GetFloat("LKScaleX", 1), PlayerPrefs.GetFloat("LKScaleY", 1), 0);
			}
			if (buttonID == "HK")
			{
				this.GetComponent<RectTransform>().anchoredPosition = new Vector3(PlayerPrefs.GetFloat("HKX", -231.70f), PlayerPrefs.GetFloat("HKY", 144.90f), 0);
				this.transform.localScale = new Vector3(PlayerPrefs.GetFloat("HKScaleX", 1), PlayerPrefs.GetFloat("HKScaleY", 1), 0);
			}
			if (buttonID == "SP")
			{
				print("In SP");
				this.GetComponent<RectTransform>().anchoredPosition = new Vector3(PlayerPrefs.GetFloat("SPX", -231.70f), PlayerPrefs.GetFloat("SPY", 144.90f), 0);
				this.transform.localScale = new Vector3(PlayerPrefs.GetFloat("SPScaleX", 1), PlayerPrefs.GetFloat("SPScaleY", 1), 0);
			}
			if (buttonID == "BLOCK")
			{
				this.GetComponent<RectTransform>().anchoredPosition = new Vector3(PlayerPrefs.GetFloat("BX", -173.20f), PlayerPrefs.GetFloat("BY", 225.40f), 0);//(893.47, 225.80)
				this.transform.localScale = new Vector3(PlayerPrefs.GetFloat("BScaleX", 1), PlayerPrefs.GetFloat("BScaleY", 1), 0);
			}

			this.initialRectCopy.anchoredPosition3D = this.initialAnchoredPosition3D;
				this.initialRectCopy.anchorMin = this.initialAnchorMin;
				this.initialRectCopy.anchorMax = this.initialAnchorMax;
				this.initialRectCopy.offsetMin = this.initialOffsetMin;
				this.initialRectCopy.offsetMax = this.initialOffsetMax;
				this.initialRectCopy.pivot = this.initialPivot;
			StoreDefaultPos();
			if (buttonID == "SP") {
				print("Updated Anchor"+ this.initialAnchoredPosition3D);
			}
		}

#if UNITY_EDITOR		
	[ContextMenu("Add Default Animator")]
	private void ContextMenuCreateAnimator()
		{
		ControlFreak2Editor.TouchControlWizardUtils.CreateButtonAnimator(this, "-Animator", 
			ControlFreak2Editor.TouchControlWizardUtils.GetDefaultButtonSprite(this.name), 1, "Create Touch Button Animator");
		}
#endif


//! \endcond

	}
}
