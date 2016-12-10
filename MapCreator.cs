using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreator : MonoBehaviour
{
    public static float BLOCK_WIDTH = 1.0f; //블록의 폭
    public static float BLOCK_HEIGHT = 0.2f; //블록의 높이
    public static int BLOCK_NUM_IN_SCREEN = 24; //화면 내에 들어가는 블록의 개수

    //블록에 관한 정보를 모아서 관리하는 구조체
    private struct FloorBlock
    {
        public bool is_created; //블록이 만들어졌는가
        public Vector3 position; //블록의 위치
    };

    private FloorBlock last_block; //마지막에 생성한 블록
    private PlayerControl player = null; //씬 상의 Player를 보관
    private BlockCreator block_creator; //BlockCreator를 보관

    // Use this for initialization
    void Start()
    {
        this.player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();//플레이어 스크립트 지정
        this.last_block.is_created = false;
        this.block_creator = this.gameObject.GetComponent<BlockCreator>(); //BlockCreator 스크립트 지정
    }

    // Update is called once per frame
    void Update()
    {
        //플레이어의 X위치를 가져온다.
        float block_generate_x = this.player.transform.position.x;
        //그리고 대략 반 화면만큼 오른쪽으로 이동
        //이 위치가 블록을 생성하는 문턱 값이 된다.
        block_generate_x += BLOCK_WIDTH * ((float)BLOCK_NUM_IN_SCREEN + 1) / 2.0f;

        //마지막에 만든 블록의 위치가 문턱 값보다 작으면
        while (this.last_block.position.x < block_generate_x)
        {
            //블록을 만든다
            this.create_floor_block();
        }
    }

    //블록을 만드는 위치를 결정하고 BlockCreator에 블록 생성 지시를 내린다.
    private void create_floor_block()
    {
        Vector3 block_position; //이제부터 만들 블록의 위치
        if (!this.last_block.is_created)//last_block이 생성되지 않은 경우
        {
            //블록의 위치를 일단 Player와 같게 한다
            block_position = this.player.transform.position;
            //그리고 나서 블록의 X위치를 화면 절반만큼 왼쪽으로 이동
            block_position.x -= BLOCK_WIDTH * ((float)BLOCK_NUM_IN_SCREEN / 2.0f);
            //블록의 Y위치는 0으로
            block_position.y = 0.0f;
        }
        else //last_block이 생성된 경우
        {
            //이번에 만들 블록의 위치를 직전에 만든 블록과 같게
            block_position = this.last_block.position;
        }

        //블록을 1블록만큼 오른쪽으로 이동
        block_position.x += BLOCK_WIDTH;

        //BlockCreator 스크립트의 createBlock()메서드에 생성을 지시
        //이제까지의 코드에서 설정한 block_position을 건네준다.
        this.block_creator.createBlock(block_position);

        //last_block의 위치를 현재 위치로 갱신
        last_block.position = block_position;
        //블록이 생성되었으므로 last_block의 is_created를 true로 설정
        this.last_block.is_created = true;
    }

    public bool isDelete(GameObject block_object)
    {
        bool ret = false; //반환값

        //Player로부터 반 화면만큼 왼쪽에 위치
        //이 위치가 사라지느냐 마느냐를 결정하는 문턱값이 됨
        float left_limit = this.player.transform.position.x - BLOCK_WIDTH * ((float)BLOCK_NUM_IN_SCREEN / 2.0f);

        //블록의 위치가 문턱값보다 작으면(왼쪽)
        if (block_object.transform.position.x < left_limit)
        {
            ret = true; //반환값을 사라져도 좋다로
        }
        return ret;//반환
    }


}
