﻿using UnityEngine;
using System.Collections;

public class ControlScale : MonoBehaviour 
{
	//
	// Data
	//

	public float scaleSpeed = 45.0f;
	public float startingScale = 12.0f;
	public float flinchShrinkTime = 0.12f;
	public float flinchShrinkSpeed = 360.0f;
	public bool disableControls = false;


	public const float MinScale = 1.0f;
	public const float MaxScale = 100.0f;

	private float currentScaleValue;

	private static ControlScale singetonInstance;


	//
	// Methods
	//

	void Awake()
	{
		ControlScale.singetonInstance = this;
	}

	// Use this for initialization
	void Start () 
	{
		this.currentScaleValue = this.startingScale;
	}

	void OnDestroy()
	{
		ControlScale.singetonInstance = null;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(!this.disableControls)
		{
			if(TwoButtonDelegator.GetGrowButton())
			{
				this.currentScaleValue += (this.scaleSpeed * Time.deltaTime);
				if(this.currentScaleValue > ControlScale.MaxScale)
				{
					this.currentScaleValue = ControlScale.MaxScale;
				}
				
			}
			else if(TwoButtonDelegator.GetShrinkButton())
			{
				this.currentScaleValue += (-this.scaleSpeed * Time.deltaTime);
				if(this.currentScaleValue < ControlScale.MinScale)
				{
					this.currentScaleValue = ControlScale.MinScale;
				}
			}
		}
	}

	private void PrivateShrinkAnimation()
	{
		StartCoroutine(Flinch());
	}

	IEnumerator Flinch()
	{
		float endTime = Time.time + this.flinchShrinkTime;
		while(Time.time < endTime)
		{
			this.currentScaleValue -= this.flinchShrinkSpeed * Time.deltaTime;
			if(this.currentScaleValue < ControlScale.MinScale)
			{
				this.currentScaleValue = ControlScale.MinScale;
				break;
			}
			yield return null;
		}
	}



	//
	// Static Methods
	//

	/// <summary>
	/// 	Take a "from" number between 1 and 100 and convert it to a custom "to" range
	/// </summary>
	/// <param name="fromValue">Value must be between 1.0f and 100.0f</param>
	/// <param name="toMin">Lowest value to convert to</param>
	/// <param name="toMax">Highest value to convert to</param>
	public static float Rescale(float fromValue, float toMin, float toMax)
	{
		float fromRange = ControlScale.MaxScale - ControlScale.MinScale;
		float toRange = toMax - toMin;
		float convertRatio = toRange / fromRange;
		fromValue -= ControlScale.MinScale;
		float toValue = convertRatio * fromValue;
		return toValue + toMin;
	}

	/// <summary>
	/// 	The shrink animation parameters are set in the inspector for ControlScale script
	/// </summary>
	public static void ShrinkForTime()
	{
		Debug.Assert(ControlScale.singetonInstance != null, "ControlScale singeton needs to exist before using! Make one in the scene.");
		ControlScale.singetonInstance.PrivateShrinkAnimation();
	}


	public static void DisableTheControls()
	{
		Debug.Assert(ControlScale.singetonInstance != null, "ControlScale singeton needs to exist before using! Make one in the scene.");
		ControlScale.singetonInstance.disableControls = true;
	}



	//
	// Properties
	//

	/// <summary>
	/// 	Should return a float between 1.0f and 100.0f
	/// </summary>
	public static float CurrentScaleValue
	{
		get
		{
			Debug.Assert(ControlScale.singetonInstance != null, "ControlScale singeton needs to exist before using! Make one in the scene.");
			return ControlScale.singetonInstance.currentScaleValue;
		}
	}
}
