using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchRecognition : MonoBehaviour {

	public Text debug_text;

	//タッチしたポイントをCanvasのどこなのかに変換するやつ
	Vector2 offset_to_pseudo_screen_size;

	//冷蔵庫のイメージの左下と右上
	Vector2 image_left_down;
	Vector2 image_right_up;

	// Start is called before the first frame update
	void Start() {
		//画面サイズ取得
		Vector2 screen_size = new Vector2(Screen.width, Screen.height);
		Debug.Log("Screen Size: " + screen_size);
		debug("Screen Size: " + screen_size);

		//画面サイズによって変わったCanvasのサイズ取得，変換するやつを設定
		RectTransform rect_refrigerator_canvas = GameObject.Find("Refrigerator Canvas").GetComponent<RectTransform>();
		Vector2 canvas_size = rect_refrigerator_canvas.sizeDelta;

		Debug.Log("Canvas Size: " + canvas_size);
		offset_to_pseudo_screen_size = new Vector2(canvas_size.x / screen_size.x, canvas_size.y / screen_size.y);

		//冷蔵庫の左下と右端の座標を設定
		image_left_down = canvas_size / 2 - new Vector2(600, 500);
		image_right_up = canvas_size / 2 + new Vector2(600, 500);
		Debug.Log("Left Down: " + image_left_down);
		Debug.Log("Right Up: " + image_right_up);
	}

	// Update is called once per frame
	void Update() {
		if (Application.isEditor) {
			if (Input.GetMouseButtonDown(0)) {
				Vector2 click_position = Input.mousePosition * offset_to_pseudo_screen_size;
				
				Vector2 tmp1 = image_right_up - click_position;
				Vector2 tmp2 = click_position - image_left_down;
				if(tmp1.x >= 0 && tmp1.y >= 0 && tmp2.x >= 0 && tmp2.y >= 0) {
					debug("Click: " + click_position + ", " + "OK");
				}
				else {
					debug("Click: " + click_position + ", " + "NG");
				}
			}
		}
		else {
			if(Input.touchCount > 0) {
				Touch touch = Input.GetTouch(0);
				TouchPhase touch_phase = touch.phase;
				Vector2 touch_position = touch.position * offset_to_pseudo_screen_size;
				
				if (touch_phase == TouchPhase.Began) {
					Vector2 tmp1 = image_right_up - touch_position;
					Vector2 tmp2 = touch_position - image_left_down;
					if (tmp1.x >= 0 && tmp1.y >= 0 && tmp2.x >= 0 && tmp2.y >= 0) {
						debug("Touch: " + touch_position + ", " + "OK");
					}
					else {
						debug("Touch: " + touch_position + ", " + "NG");
					}
				}
			}
		}
	}

	void debug(object message) {
		if(debug_text != null) {
			debug_text.text = message.ToString();
		}
	}
}
