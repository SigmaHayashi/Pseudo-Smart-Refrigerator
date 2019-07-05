using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchRecognition : MonoBehaviour {

	[NonSerialized]
	public Vector2 touch_position_of_refrigerator = new Vector2(-1, -1);

	[NonSerialized]
	public bool touch_start = false;

	[NonSerialized]
	public bool touch_on_image = false;

	//public Text debug_text;

	//タッチしたポイントをCanvasのどこなのかに変換するやつ
	private Vector2 offset_to_pseudo_screen_size;

	//冷蔵庫のイメージの左下と右上
	private Vector2 image_left_down;
	private Vector2 image_right_up;

	private Vector2[] offset_to_refrigerator_size = new Vector2[2];
	private Vector2 real_refrigerator_size = new Vector2(0.35f, 0.38f);


	// Start is called before the first frame update
	void Start() {
		//画面サイズ取得
		Vector2 screen_size = new Vector2(Screen.width, Screen.height);
		Debug.Log("Screen Size: " + screen_size);
		//debug("Screen Size: " + screen_size);

		//画面サイズによって変わったCanvasのサイズ取得，変換するオフセット値を計算
		RectTransform rect_refrigerator_canvas = GameObject.Find("Refrigerator Canvas").GetComponent<RectTransform>();
		Vector2 canvas_size = rect_refrigerator_canvas.sizeDelta;

		Debug.Log("Canvas Size: " + canvas_size);
		offset_to_pseudo_screen_size = new Vector2(canvas_size.x / screen_size.x, canvas_size.y / screen_size.y);

		//冷蔵庫の左下と右端の座標を計算
		Image whole_image = GameObject.Find("Whole Image").GetComponent<Image>();
		Vector2 image_size = whole_image.rectTransform.sizeDelta;

		image_left_down = (canvas_size - image_size)/ 2;
		image_right_up = (canvas_size + image_size) / 2;
		Debug.Log("Left Down: " + image_left_down);
		Debug.Log("Right Up: " + image_right_up);

		//タッチした点を冷蔵庫サイズでいうとどこなのかに変換するオフセット値を計算
		/*************************
		 * a0 * x0 + b0 = 0
		 * a0 * x1 + b0 = 0.35
		 * a1 * y0 + b1 = 0
		 * a1 * y1 + b1 = 0.38
		 * 
		 * [0] <= a0, a1
		 * [1] <= b0, b1
		 ************************/
		offset_to_refrigerator_size[0] = real_refrigerator_size / (image_right_up - image_left_down);
		offset_to_refrigerator_size[1] = offset_to_refrigerator_size[0] * image_left_down * -1;
	}

	// Update is called once per frame
	void Update() {
		if (Application.isEditor) {
			if (Input.GetMouseButtonDown(0)) {
				touch_start = true;

				/*
				Vector2 click_position = Input.mousePosition * offset_to_pseudo_screen_size * offset_to_refrigerator_size[0] + offset_to_refrigerator_size[1];
				if(click_position.x >= 0 && click_position.y >= 0 && click_position.x <= real_refrigerator_size.x && click_position.y < real_refrigerator_size.y) {
					//debug("Click: " + click_position.ToString("f2") + ", OK");
				}
				else {
					//debug("Click: " + click_position.ToString("f2") + ", NG");
				}
				*/

				touch_position_of_refrigerator = Input.mousePosition * offset_to_pseudo_screen_size * offset_to_refrigerator_size[0] + offset_to_refrigerator_size[1];
				if (touch_position_of_refrigerator.x >=  0 && touch_position_of_refrigerator.y >= 0 && touch_position_of_refrigerator.x <= real_refrigerator_size.x && touch_position_of_refrigerator.y <= real_refrigerator_size.y) {
					//touch_position_of_refrigerator = new Vector2(-1, -1);
					touch_on_image = true;
				}
				else {
					touch_on_image = false;
				}
			}
			else {
				touch_start = false;
			}
		}
		else {
			if(Input.touchCount > 0) {
				Touch touch = Input.GetTouch(0);
				/*
				TouchPhase touch_phase = touch.phase;
				Vector2 touch_position = touch.position * offset_to_pseudo_screen_size * offset_to_refrigerator_size[0] + offset_to_refrigerator_size[1];

				if(touch_phase == TouchPhase.Began) {
					if (touch_position.x >= 0 && touch_position.y >= 0 && touch_position.x <= real_refrigerator_size.x && touch_position.y < real_refrigerator_size.y) {
						//debug("Touch: " + touch_position.ToString("f2") + ", OK");
					}
					else {
						//debug("Touch: " + touch_position.ToString("f2") + ", NG");
					}
				}*/

				if(touch.phase == TouchPhase.Began) {
					touch_start = true;

					touch_position_of_refrigerator = touch.position * offset_to_pseudo_screen_size * offset_to_refrigerator_size[0] + offset_to_refrigerator_size[1];
					if (touch_position_of_refrigerator.x >= 0 && touch_position_of_refrigerator.y >= 0 && touch_position_of_refrigerator.x <= real_refrigerator_size.x && touch_position_of_refrigerator.y <= real_refrigerator_size.y) {
						touch_on_image = true;
					}
					else {
						touch_on_image = false;
					}
				}
				else {
					touch_start = false;
				}
			}
		}
	}

	/*
	void debug(object message) {
		if(debug_text != null) {
			debug_text.text = message.ToString();
		}
	}
	*/
}