﻿using UnityEngine;
namespace HSVPicker.Examples
{
    public class ColorPickerTester : MonoBehaviour 
    {

        public new Renderer renderer;
        public ColorPicker picker;

        public Color Color = Color.red;
        public bool SetColorOnStart = false;

	    // Use this for initialization
	    void Start () 
        {
            picker.onValueChanged.AddListener(color =>
            {
                print("Yes Here");
                renderer.material.color = color;
                //renderer.material.SetColor("_Color", color);
                Color = color;
            });

		    renderer.material.color = picker.CurrentColor;
            if(SetColorOnStart) 
            {
                picker.CurrentColor = Color;
            }
        }
    }
}