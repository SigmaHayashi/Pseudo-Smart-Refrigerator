using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PseudoSmartRefrigerator : MonoBehaviour {

	private AndroidRosSocketClient wsc;

	private string publih_name = "tms_db_data";
	private tmsdbStamped stamped = new tmsdbStamped();

	private tmsdb cancoffee_data = new tmsdb();

	private double offset_x_refrigerator = 7.00 + 0.05;
	private double offset_y_refrigerator = 5.52 + 0.10;
	private double offset_z_refrigerator = 0.75;
	
	private Button DeleteButton;

	private float time = 0.0f;

	//タッチ関連
	private TouchRecognition recog;
	private Image cancoffee_image;

	// Start is called before the first frame update
	void Start() {
		MyUISetting();

		cancoffee_data.id = 7006;
		cancoffee_data.name = "cancoffee";
		cancoffee_data.place = 2009;
		cancoffee_data.sensor = 3018;
		cancoffee_data.tag = "E00401004E180EA0";

		//ROSTMSに接続
		wsc = GameObject.Find("Android Ros Socket Client").GetComponent<AndroidRosSocketClient>();
		wsc.Advertiser(publih_name, "tms_msg_db/TmsdbStamped");

		//タッチ関連システム取得
		recog = GameObject.Find("Touch Recognition System").GetComponent<TouchRecognition>();
		recog.ChangeImagePosition(cancoffee_image, false);
	}


	// Update is called once per frame
	void Update() {
		if(wsc.conneciton_state != wscCONST.STATE_CONNECTED) {
			time += Time.deltaTime;
			if(time > 3.0f) {
				time = 0.0f;
				wsc.Connect();
				wsc.Advertiser(publih_name, "tms_msg_db/TmsdbStamped");
			}
		}

		//タッチした場所をデータベースに登録
		if (recog.touch_start && recog.touch_on_image) {
			Debug.Log("Touch: " + recog.touch_position_of_refrigerator.ToString("f2"));

			cancoffee_data.time = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("00") + "-" + DateTime.Now.Day.ToString("00") + "T";
			cancoffee_data.time += DateTime.Now.Hour.ToString("00") + ":" + DateTime.Now.Minute.ToString("00") + ":" + DateTime.Now.Second.ToString("00") + "." + DateTime.Now.Millisecond.ToString();
			cancoffee_data.x = recog.touch_position_of_refrigerator.x + offset_x_refrigerator;
			cancoffee_data.y = recog.touch_position_of_refrigerator.y + offset_y_refrigerator;
			cancoffee_data.z = offset_z_refrigerator;
			cancoffee_data.state = 1;

			Publish(cancoffee_data);

			recog.ChangeImagePosition(cancoffee_image);

			Debug.Log("Change to: " + new Vector3((float)cancoffee_data.x, (float)cancoffee_data.y, (float)cancoffee_data.z));
		}
	}


	private void OnApplicationQuit() {
		wsc.UnAdvertiser(publih_name);
		wsc.Close();
	}


	/*******************************************************
	 * データベースを更新
	 ******************************************************/
	void Publish(tmsdb data) {
		tmsdb[] tmsdbs = new tmsdb[1];
		tmsdbs[0] = data;
		stamped.tmsdb = tmsdbs;

		wsc.PublisherDB(publih_name, stamped);
	}
	
	/*******************************************************
	 * ボタンとかテキストのセットアップ
	 ******************************************************/
	void MyUISetting() {
		DeleteButton = GameObject.Find("Main System/Button Canvas/Delete Button").GetComponent<Button>();
		DeleteButton.onClick.AddListener(onClickDelete);

		cancoffee_image = GameObject.Find("Main System/Refrigerator Canvas/CanCoffee Image").GetComponent<Image>();
	}

	/*******************************************************
	 * ボタンを押したときの動作
	 ******************************************************/
	void onClickDelete() {
		cancoffee_data.time = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("00") + "-" + DateTime.Now.Day.ToString("00") + "T";
		cancoffee_data.time += DateTime.Now.Hour.ToString("00") + ":" + DateTime.Now.Minute.ToString("00") + ":" + DateTime.Now.Second.ToString("00") + "." + DateTime.Now.Millisecond.ToString();
		cancoffee_data.x = -1;
		cancoffee_data.y = -1;
		cancoffee_data.z = -1;
		cancoffee_data.state = 0;

		Publish(cancoffee_data);

		recog.ChangeImagePosition(cancoffee_image, false);

		Debug.Log("Delete");
	}
}
