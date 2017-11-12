using UnityEngine;
using System.Collections;
 
public class UIMaskLayer : MonoBehaviour
{
	void Start()
	{
		int width = Screen.width;
		int height = Screen.height;
		int designWidth = 960;//开发时分辨率宽
		int designHeight = 640;//开发时分辨率高
		float s1 = (float)designWidth / (float)designHeight;
		float s2 = (float)width / (float)height;
		if(s1 < s2) {
			designWidth = (int)Mathf.FloorToInt(designHeight * s2);
		} else if(s1 > s2) {
			designHeight = (int)Mathf.FloorToInt(designWidth / s2);
		}
		float contentScale = (float)designWidth/(float)width;
		RectTransform rectTransform = this.transform as RectTransform;
		if(rectTransform != null){
			rectTransform.sizeDelta = new Vector2(designWidth,designHeight);
		}
	}
}