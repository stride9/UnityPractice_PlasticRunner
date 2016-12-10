using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public static float ACCELERATION = 10.0f;//가속도
    public static float SPEED_MIN = 4.0f;//속도의 최소값
    public static float SPEED_MAX = 8.0f;//속도의 최대값
    public static float JUMP_HEIGHT_MAX = 3.0f;//점프 높이
    public static float JUMP_KEY_RELEASE_REDUCE = 0.5f; //점프 후의 감속도

    public enum STEP
    {//Player의 각종 상태를 나타내는 자료형
        NONE = -1, //상태정보 없음
        RUN = 0, //달린다
        JUMP, //뛴다
        MISS, //실패
        NUM, //상태가 몇 종류 있는지 보여준다.(=3)
    };

    public STEP step = STEP.NONE; //player의 현재 상태
    public STEP next_step = STEP.NONE; //player의 다음 상태

    public float step_timer = 0.0f; //경과 시간
    private bool is_landed = false; //착지했는가
    private bool is_colided = false; //뭔가와 충돌했는가
    private bool is_key_released = false; //버튼이 떨어졌는가


    // Use this for initialization
    void Start()
    {
        this.next_step = STEP.RUN;
    }

    // Update is called once per frame
    void Update()
    {
        //this.transform.Translate(new Vector3(0.0f, 0.0f, 3.0f * Time.deltaTime));
        Vector3 velocity = this.GetComponent<Rigidbody>().velocity; //속도를 설정
        this.check_landed();//착지 상태인지 체크
        this.step_timer += Time.deltaTime;//경과 시간을 진행한다.
        //Debug.Log(this.step);
        //다음 상태가 정해져 있지 않으면 상태의 변화를 조사한다.
        if (this.next_step == STEP.NONE)
        {
            switch (this.step)
            {//Player의 현재 상태에 따라 분기
                case STEP.RUN: //달리는 중
                    if (!this.is_landed)
                    {
                        //달리는 중이고 착지하지 않은 경우 아무것도 하지 않는다.
                    }
                    else
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            //달리는 중이고 착지했고 왼쪽 버튼이 눌렸으면
                            //상태를 점프로 변경
                            this.next_step = STEP.JUMP;
                            //Debug.Log(this.next_step);
                        }
                    }
                    break;
                case STEP.JUMP: //점프중
                    if (this.is_landed)
                    {
                        //점프중이고 착지했다면 다음 상태를 주행중으로 변경
                        this.next_step = STEP.RUN;
                    }
                    break;

            }
        }

        //상태가 변할 때
        while (this.next_step != STEP.NONE)
        {
            //Debug.Log("before change: " + this.step);
            this.step = this.next_step;//현재 상태를 다음 상태로 갱신
            //Debug.Log("after change: " + this.step);
            this.next_step = STEP.NONE;//다음 상태를 NONE으로 갱신
            switch (this.step)
            {//갱신된 현재 상태가
                case STEP.JUMP: //점프일 때
                    //점프할 높이로 점프 속도를 계산(2gh=v^2)
                    velocity.y = Mathf.Sqrt(2.0f * 9.8f * PlayerControl.JUMP_HEIGHT_MAX);
                    //버튼이 떨어졌음을 나타내는 플래그를 클리어한다
                    this.is_key_released = false;
                    break;
            }
            this.step_timer = 0.0f;
        }

        //상태별로 매 프레임 갱신 처리
        switch (this.step)
        {
            case STEP.RUN: //달리는 중일 때
                //속도를 높인다.
                velocity.x += PlayerControl.ACCELERATION * Time.deltaTime;
                //속도가 최고 속도 제한을 넘으면
                if (Mathf.Abs(velocity.x) > PlayerControl.SPEED_MAX)
                {
                    //최고 속도 제한 이하로 유지한다.
                    velocity.x *= PlayerControl.SPEED_MAX / Mathf.Abs(this.GetComponent<Rigidbody>().velocity.x);
                }
                break;
            case STEP.JUMP: //점프 중일 때
                //Debug.Log("Entered Jump State");
                do
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        Debug.Log("GetMouseButton released");
                    }
                    //버튼이 떨어진 순간이 아니면
                    if (!Input.GetMouseButtonUp(0))
                    {
                        //Debug.Log("GetMouseButtonUP");
                        break;//빠져나간다.
                    }
                    //이미 감속된 상태면(두 번 이상 감속하지 않도록)
                    if (this.is_key_released)
                    {
                        Debug.Log("is_key_released");
                        break;//빠져나간다.
                    }
                    //상하방향 속도가 0 이하면(하강 중이라면)
                    if (velocity.y <= 0.0f)
                    {
                        Debug.Log("velocity.y<=0");
                        Debug.Log(velocity.y);
                        break;//빠져나간다.
                    }
                    //버튼이 떨어져 있고 상승 중이라면 감속 시작
                    //점프의 상승은 여기서 끝
                    Debug.Log("Downforce");
                    velocity.y *= JUMP_KEY_RELEASE_REDUCE;

                    this.is_key_released = true;

                } while (false);
                break;
        }
        //Rigidbody의 속도를 위에서 구한 속도로 갱신
        //이 행은 매 번 실행된다.
        this.GetComponent<Rigidbody>().velocity = velocity;
    }

    private void check_landed()
    {
        this.is_landed = false;//일단 false로 설정
        do//지역적 if문들을 묶어서 처리(다른 조건문들과 섞이지 않게)
        {
            Vector3 s = this.transform.position; //Player의 현재 위치
            Vector3 e = s + Vector3.down * 1.0f;//s부터 아래로 1.0f 이동한 위치
            RaycastHit hit;
            if (!Physics.Linecast(s, e, out hit))
            {//s부터 e사이에 아무것도 없을 때
                break;//아무것도 하지 않고 do-while루프를 빠져나감(탈출구로)
            }

            //s부터 e사이에 뭔가 있을 때 아래의 처리가 실행
            if (this.step == STEP.JUMP)
            {//현재 점프 상태라면
             //경과 시간이 3.0f미만이면
                if (this.step_timer < Time.deltaTime * 3.0f)
                {
                    break;//루프를 빠져나감
                }
            }
            //s부터 e사이에 뭔가 있고 JUMP직후가 아닐 때만 아래가 실행
            this.is_landed = true;

        } while (false);
    }
}
