using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PseudoSmartRefrigerator : MonoBehaviour {

	private AndroidRosSocketClient wsc;

	private string srvName = "tms_db_reader";
	private TmsDBReq srvReq = new TmsDBReq();

	private string publih_name = "tms_db_data";
	private tmsdbStamped stamped = new tmsdbStamped();

	private tmsdb cancoffee_data = new tmsdb();
	private tmsdb greentea_data = new tmsdb();
	private tmsdb soysauce_data = new tmsdb();
	private List<tmsdb> data_list = new List<tmsdb>();

	private double offset_x_refrigerator = 7.00 + 0.05;
	private double offset_y_refrigerator = 5.52 + 0.10;
	private double offset_z_refrigerator = 0.75;
	
	private Button DeleteButton;

	private float time = 0.0f;
	private bool init_flag = false;

	//タッチ関連
	private TouchRecognition recog;
	private Image cancoffee_image;
	private Image greentea_image;
	private Image soysauce_image;
	private List<Image> image_list = new List<Image>();

	private Dropdown dropdown;


	// Start is called before the first frame update
	void Start() {
		MyUISetting();

		DBSetting();

		//ROSTMSに接続
		wsc = GameObject.Find("Android Ros Socket Client").GetComponent<AndroidRosSocketClient>();
		try {
			wsc.Advertiser(publih_name, "tms_msg_db/TmsdbStamped");
		}
		catch { }

		//タッチ関連システム取得
		recog = GameObject.Find("Touch Recognition System").GetComponent<TouchRecognition>();
		InitPosition();
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
		else {
			if (!init_flag) {
				InitPosition();
			}
		}

		//タッチした場所をデータベースに登録
		if (recog.touch_start && recog.touch_on_image) {
			//Debug.Log("Touch: " + recog.touch_position_of_refrigerator.ToString("f2"));
			
			int dropdown_value = dropdown.value;
			data_list[dropdown_value].time = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("00") + "-" + DateTime.Now.Day.ToString("00") + "T";
			data_list[dropdown_value].time += DateTime.Now.Hour.ToString("00") + ":" + DateTime.Now.Minute.ToString("00") + ":" + DateTime.Now.Second.ToString("00") + "." + DateTime.Now.Millisecond.ToString();
			data_list[dropdown_value].x = recog.touch_position_of_refrigerator.x + offset_x_refrigerator;
			data_list[dropdown_value].y = recog.touch_position_of_refrigerator.y + offset_y_refrigerator;
			data_list[dropdown_value].z = offset_z_refrigerator;
			data_list[dropdown_value].state = 1;

			Publish(data_list[dropdown_value]);

			recog.ChangeImagePosition(image_list[dropdown_value]);

			Debug.Log("Change: " + data_list[dropdown_value].name + "(" + dropdown_value + "), " + new Vector3((float)data_list[dropdown_value].x, (float)data_list[dropdown_value].y, (float)data_list[dropdown_value].z).ToString("f2"));
		}
	}


	private void OnApplicationQuit() {
		wsc.UnAdvertiser(publih_name);
		wsc.Close();
	}

	/*******************************************************
	 * データベースにある初期位置を取得
	 ******************************************************/
	void InitPosition() {
		time += Time.deltaTime;
		if(time > 1.0f) {
			time = 0.0f;

			srvReq.tmsdb = new tmsdb("PLACE", 2009);
			wsc.ServiceCallerDB(srvName, srvReq);
		}
		if(wsc.IsReceiveSrvRes() && wsc.GetSrvResValue("service") == srvName) {
			string srvRes = wsc.GetSrvResMsg();
			Debug.Log("ROS: " + srvRes);

			ServiceResponseDB responce = JsonUtility.FromJson<ServiceResponseDB>(srvRes);

			foreach(tmsdb data in responce.values.tmsdb) {
				foreach (Image image in image_list) {
					if(image.name.IndexOf(data.name) != -1) {
						if(data.state == 1) {
							Vector2 position = new Vector2((float)data.x, (float)data.y);
							Vector2 offset = new Vector2((float)offset_x_refrigerator, (float)offset_y_refrigerator);
							position -= offset;
							Debug.Log(data.name + ": " + position.ToString("f2"));

							recog.touch_position_of_refrigerator = position;
							//recog.ChangeImagePosition(image, position);
							recog.ChangeImagePosition(image);
						}
						else {
							Debug.Log(data.name + ": not exist");
							recog.ChangeImagePosition(image, false);
						}
					}
				}
			}

			init_flag = true;
			Debug.Log("Init Position Complete");
		}
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

		greentea_image = GameObject.Find("Main System/Refrigerator Canvas/greentea_bottle Image").GetComponent<Image>();
		cancoffee_image = GameObject.Find("Main System/Refrigerator Canvas/cancoffee Image").GetComponent<Image>();
		soysauce_image = GameObject.Find("Main System/Refrigerator Canvas/soysauce_bottle_black Image").GetComponent<Image>();
		image_list.Add(greentea_image);
		image_list.Add(cancoffee_image);
		image_list.Add(soysauce_image);

		dropdown = GameObject.Find("Main System/Button Canvas/Item Dropdown").GetComponent<Dropdown>();
		dropdown.ClearOptions();
		dropdown.AddOptions(new List<string> { "Green Tea", "Cancoffee", "Soy Sauce" });
		dropdown.RefreshShownValue();
	}

	/*******************************************************
	 * データベースクラスの初期化
	 ******************************************************/
	void DBSetting() {
		greentea_data.id = 7004;
		greentea_data.name = "greentea_bottle";
		greentea_data.place = 2009;
		greentea_data.sensor = 3018;
		greentea_data.tag = "E00401004E180E60";

		cancoffee_data.id = 7006;
		cancoffee_data.name = "cancoffee";
		cancoffee_data.place = 2009;
		cancoffee_data.sensor = 3018;
		cancoffee_data.tag = "E00401004E180EA0";

		soysauce_data.id = 7009;
		soysauce_data.name = "soysauce_bottle_black";
		soysauce_data.place = 2009;
		soysauce_data.sensor = 3018;
		soysauce_data.tag = "E00401004E18C87";

		data_list.Add(greentea_data);
		data_list.Add(cancoffee_data);
		data_list.Add(soysauce_data);
	}

	/*******************************************************
	 * ボタンを押したときの動作
	 ******************************************************/
	void onClickDelete() {
		int dropdown_value = dropdown.value;
		data_list[dropdown_value].time = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("00") + "-" + DateTime.Now.Day.ToString("00") + "T";
		data_list[dropdown_value].time += DateTime.Now.Hour.ToString("00") + ":" + DateTime.Now.Minute.ToString("00") + ":" + DateTime.Now.Second.ToString("00") + "." + DateTime.Now.Millisecond.ToString();
		data_list[dropdown_value].x = -1;
		data_list[dropdown_value].y = -1;
		data_list[dropdown_value].z = -1;
		data_list[dropdown_value].state = 0;

		Publish(data_list[dropdown_value]);

		recog.ChangeImagePosition(image_list[dropdown_value], false);

		Debug.Log("Delete: " + data_list[dropdown_value].name + "(" + dropdown_value + ")");
	}
}
