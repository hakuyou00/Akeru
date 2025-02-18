﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TankController : MonoBehaviour
{
    //砲身
    [NonSerialized]
    public GameObject Barrel;

    //砲弾、Prefabを入れる
    [SerializeField]
    private GameObject bullet;
    //DoorController
    private DoorController _door;

    //砲弾の発射位置
    private GameObject point;

    //砲弾の発射の感覚をあけるためのもの
    private float timeBullet;

    //Tankのアニメーター
    private Animator tankAnime;

    //アニメーション用の真偽変数
    private bool isStay = true;

    //Doorの開閉を管理する真偽変数
    private bool isDoorOpen = false;
    private bool isPass = false;

    void Start()
    {
        //砲身を取得(実際はその土台)
        Barrel = GameObject.Find("Pivot");
        //砲弾の発射位置を取得
        point = Barrel.transform.GetChild(1).gameObject;

        //開始時刻で初期化
        timeBullet = Time.time;

        //Animatorを取得
        tankAnime = this.GetComponent<Animator>();

        //
        _door = GameObject.Find("axis").GetComponent<DoorController>();

    }

    void Update()
    {
        //上下の入力で砲身を上下させる

        float rotate = Input.GetAxis("Vertical");
        float y = Barrel.transform.localEulerAngles.y;
        if(y > 30f && y < 85f) //30°～85°の範囲で移動させる
        {
            Barrel.transform.Rotate(new Vector3(0 , rotate * -0.1f));
        }
        else if(y >= 85f && rotate >= 0f) //範囲外に行かないようにする
        {
            Barrel.transform.Rotate(new Vector3(0 , rotate * -0.1f));
        }
        else if(y <= 30f && rotate <= 0f) //上に同じ
        {
            Barrel.transform.Rotate(new Vector3(0 , rotate * -0.1f));
        }

        float timedif = Time.time - timeBullet;
        //Space入力で砲弾を発射
        if(Input.GetKeyDown(KeyCode.Space) && timedif >= 0.4f)
        {
            //ベースの1000に時間が空くほど速度が上がるようにする、(三項演算子…使うな！！！)
            float speed = 900f * ( ( timedif > 1.5f ? 1.5f : timedif ) + 0.5f );
            //砲身の角度を弧度法で取得
            y = Barrel.transform.localEulerAngles.y * Mathf.Deg2Rad;
            //砲弾を生成、pointの位置に向きはそのまま
            var obj = Instantiate(bullet , point.transform.position , Quaternion.identity);
            //AddForceで砲弾を飛ばす,
            obj.GetComponent<Rigidbody>().AddForce(0 , Mathf.Cos(y) * speed , Mathf.Sin(y) * -speed);
            //射撃終了時刻で時間をリセット
            timeBullet = Time.time;
        }
        //z座標の取得
        float z = this.transform.position.z;
        //tankのz座標が25以下(ドアをくぐる前)-10以上(ドアをくぐった後)の時に実行
        if(z <= 25f && z >= -10f)
        {
            //ドアが開いておらず、ドアをくぐっていないとき
            if(!isDoorOpen && !isPass)
            {
                //ドアを開ける
                isDoorOpen = !isDoorOpen;
                _door.DoorAnimation();
            }
            //ドアが開いている時にドアをくぐったことを感知
            if(z <= -7f && isDoorOpen) isPass = !isPass;
            //ドアが開いていて、ドアをくぐったあと
            if(isDoorOpen && isPass)
            {
                //ドアを閉める
                isDoorOpen = !isDoorOpen;
                _door.DoorAnimation();
                Invoke(nameof(SceneChange) , 6.0f);
            }
        }
    }
    //クリアした時のTankの前進
    public void GoTank()
    {
        isStay = !isStay;
        tankAnime.SetBool("isStay" , isStay);
    }
    void SceneChange()
	{
        SceneManager.LoadScene("Select");
	}
}
