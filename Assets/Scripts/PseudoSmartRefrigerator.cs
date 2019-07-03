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

	private double offset_x_refrigerator = 7.0;
	private double offset_y_refrigerator = 5.52;
	private double offset_z_refrigerator = 0.75;

	public Button PositionChangeButton1;
	public Button PositionChangeButton2;
	public Button PositionChangeButton3;
	public Button PositionChangeButton4;
	public Button AppearButton;
	public Button DeleteButton;

	private float time = 0.0f;


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

	}

	private void OnApplicationQuit() {
		wsc.UnAdvertiser(publih_name);
		wsc.Close();
	}

	void MyUISetting() {
		PositionChangeButton1.onClick.AddListener(onClickPositionChange1);
		PositionChangeButton2.onClick.AddListener(onClickPositionChange2);
		PositionChangeButton3.onClick.AddListener(onClickPositionChange3);
		PositionChangeButton4.onClick.AddListener(onClickPositionChange4);
		AppearButton.onClick.AddListener(onClickAppear);
		DeleteButton.onClick.AddListener(onClickDelete);
	}

	/*******************************************************
	 * ボタンを押したときの動作
	 ******************************************************/
	void onClickPositionChange1() {
		cancoffee_data.time = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("00") + "-" + DateTime.Now.Day.ToString("00") + "T";
		cancoffee_data.time += DateTime.Now.Hour.ToString("00") + ":" + DateTime.Now.Minute.ToString("00") + ":" + DateTime.Now.Second.ToString("00") + "." + DateTime.Now.Millisecond.ToString();
		cancoffee_data.x = 0.15 + offset_x_refrigerator;
		cancoffee_data.y = 0.15 + offset_y_refrigerator;
		cancoffee_data.z = offset_z_refrigerator;
		cancoffee_data.state = 1;

		Publish(cancoffee_data);

		Debug.Log("Change 1");
	}

	void onClickPositionChange2() {
		cancoffee_data.time = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("00") + "-" + DateTime.Now.Day.ToString("00") + "T";
		cancoffee_data.time += DateTime.Now.Hour.ToString("00") + ":" + DateTime.Now.Minute.ToString("00") + ":" + DateTime.Now.Second.ToString("00") + "." + DateTime.Now.Millisecond.ToString();
		cancoffee_data.x = 0.35 + offset_x_refrigerator;
		cancoffee_data.y = 0.15 + offset_y_refrigerator;
		cancoffee_data.z = offset_z_refrigerator;
		cancoffee_data.state = 1;

		Publish(cancoffee_data);

		Debug.Log("Change 2");
	}

	void onClickPositionChange3() {
		cancoffee_data.time = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("00") + "-" + DateTime.Now.Day.ToString("00") + "T";
		cancoffee_data.time += DateTime.Now.Hour.ToString("00") + ":" + DateTime.Now.Minute.ToString("00") + ":" + DateTime.Now.Second.ToString("00") + "." + DateTime.Now.Millisecond.ToString();
		cancoffee_data.x = 0.15 + offset_x_refrigerator;
		cancoffee_data.y = 0.35 + offset_y_refrigerator;
		cancoffee_data.z = offset_z_refrigerator;
		cancoffee_data.state = 1;

		Publish(cancoffee_data);

		Debug.Log("Change 3");
	}

	void onClickPositionChange4() {
		cancoffee_data.time = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("00") + "-" + DateTime.Now.Day.ToString("00") + "T";
		cancoffee_data.time += DateTime.Now.Hour.ToString("00") + ":" + DateTime.Now.Minute.ToString("00") + ":" + DateTime.Now.Second.ToString("00") + "." + DateTime.Now.Millisecond.ToString();
		cancoffee_data.x = 0.35 + offset_x_refrigerator;
		cancoffee_data.y = 0.35 + offset_y_refrigerator;
		cancoffee_data.z = offset_z_refrigerator;
		cancoffee_data.state = 1;

		Publish(cancoffee_data);

		Debug.Log("Change 4");
	}

	void onClickAppear() {
		/*
		cancoffee_data.time = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("00") + "-" + DateTime.Now.Day.ToString("00") + "T";
		cancoffee_data.time += DateTime.Now.Hour.ToString("00") + ":" + DateTime.Now.Minute.ToString("00") + ":" + DateTime.Now.Second.ToString("00") + "." + DateTime.Now.Millisecond.ToString();
		cancoffee_data.x = 0;
		cancoffee_data.y = 0;
		cancoffee_data.z = offset_z_refrigerator;
		cancoffee_data.state = 1;

		Publish(cancoffee_data);

		Debug.Log("Appear");
		*/
	}

	void onClickDelete() {
		cancoffee_data.time = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("00") + "-" + DateTime.Now.Day.ToString("00") + "T";
		cancoffee_data.time += DateTime.Now.Hour.ToString("00") + ":" + DateTime.Now.Minute.ToString("00") + ":" + DateTime.Now.Second.ToString("00") + "." + DateTime.Now.Millisecond.ToString();
		cancoffee_data.x = -1;
		cancoffee_data.y = -1;
		cancoffee_data.z = -1;
		cancoffee_data.state = 0;

		Publish(cancoffee_data);

		Debug.Log("Delete");
	}

	void Publish(tmsdb data) {
		tmsdb[] tmsdbs = new tmsdb[1];
		tmsdbs[0] = data;
		stamped.tmsdb = tmsdbs;

		wsc.PublisherDB(publih_name, stamped);
	}
}
