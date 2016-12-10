﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockControl : MonoBehaviour
{
    public MapCreator map_creator = null;
    // Use this for initialization
    void Start()
    {
        //MapCreator를 가져와서 멤버 변수 map_creator에 보관
        map_creator = GameObject.Find("GameRoot").GetComponent<MapCreator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.map_creator.isDelete(this.gameObject))
        {//안 보이면
            GameObject.Destroy(this.gameObject); //자기 자신을 삭제
        }
    }
}
