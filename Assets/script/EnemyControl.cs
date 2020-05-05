using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    enum EmoveState
    { 
        SWING_move = 1,
        ATTACK_move,
        RETRUN_HOME_move
    };

    public AudioClip mBombClip;
    private Animator anim;

    private float moveSpeed;
    private float maxMoveDistance;
    private Vector3 mHomePos;
    private EmoveState mMoveState;
    private float mBezRunTime;
    private bool isRight;
    private ArrayList mAttackPointList;

    private float mLeftPos;
    private float mRightPos;
    private float mTopPos;
    private float mBottomPos;
    private bool isEnemyDestroy;




    struct tBezPoint
    {
        public Vector3 pos;
        public Vector3 val;
    };
    void Start()
    {
        moveSpeed = 0.05f;
        maxMoveDistance = 1.0f;
        mHomePos = transform.position;
        mMoveState = EmoveState.SWING_move;
        mBezRunTime = 0;
        isRight = true;
        mAttackPointList = null;
        Vector3 leftBtm_cornerPos = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f,
            Mathf.Abs(-Camera.main.transform.position.z))); //这里的z轴在正交视图下意义不大
        Vector3 rightTop_cornerPos = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f,
                    Mathf.Abs(-Camera.main.transform.position.z)));
        mLeftPos = leftBtm_cornerPos.x;
        mRightPos = rightTop_cornerPos.x;
        mTopPos = rightTop_cornerPos.y;
        mBottomPos = leftBtm_cornerPos.y;
    }

    public void HorizSwing_Move()
    {
        float currentTime = Time.time;
        Vector3 currentPosition = transform.position;
        if (isRight)
        {
            currentPosition.x += Mathf.Abs(Mathf.Sin(currentTime) * moveSpeed);
        }
        else
        {
            currentPosition.x -= Mathf.Abs(Mathf.Sin(currentTime) * moveSpeed);
        }
        transform.position = currentPosition;

        if (transform.position.x >= mHomePos.x + maxMoveDistance)
        {
            isRight = false;
        }
        if (transform.position.x <= mHomePos.x - maxMoveDistance)
        {
            isRight = true;
        }
    }

    private int Factorial(int n)
    {
        if (n <= 0)
        {
            return 1;
        }
        int result = 1;
        for (int idx = n; idx > 0; idx--)
        {
            result *= idx;
        }
        return result;
    }

    private bool IsPosSame(Vector3 aPos, Vector3 bPos)
    {
        if (Mathf.Abs(aPos.x - bPos.x) <= 0.01 && Mathf.Abs(aPos.y - bPos.y) <= 0.01)
        {
            return true;
        }
        return false;
    }

    private void InitBezLine(int bezNum, Vector3 startPos, Vector3 endPos)
    {
        if (mAttackPointList == null)
        {
            mAttackPointList = new ArrayList();
        }
        mAttackPointList.Clear();

        int bzIdx = 0;
        float factor = (float)Factorial(bezNum) / (float)(Factorial(bzIdx) * Factorial(bezNum - bzIdx));

        tBezPoint startBzPoint = new tBezPoint();
        startBzPoint.pos = startPos;
        startBzPoint.val.x = startBzPoint.pos.x * factor;
        startBzPoint.val.y = startBzPoint.pos.y * factor;
        startBzPoint.val.z = startBzPoint.pos.z * factor;
        mAttackPointList.Add(startBzPoint);

        bzIdx += 1;
        for (int idx = 0; idx < (bezNum - 1); idx++)
        {
            tBezPoint midBzPoint = new tBezPoint();
            midBzPoint.pos.x = Random.Range(2*mLeftPos, 2*mRightPos);
            midBzPoint.pos.y = Random.Range(2*mBottomPos, 2*mTopPos);
            midBzPoint.pos.z = 0.0f;
            factor = (float)Factorial(bezNum) / (float)(Factorial(bzIdx) * Factorial(bezNum - bzIdx));
            midBzPoint.val.x = midBzPoint.pos.x * factor;
            midBzPoint.val.y = midBzPoint.pos.y * factor;
            midBzPoint.val.z = midBzPoint.pos.z * factor;
            mAttackPointList.Add(midBzPoint);
            bzIdx += 1;
        }
        tBezPoint endBzPoint = new tBezPoint();
        endBzPoint.pos = endPos;
        factor = (float)Factorial(bezNum) / (float)(Factorial(bzIdx) * Factorial(bezNum - bzIdx));
        endBzPoint.val.x = endBzPoint.pos.x * factor;
        endBzPoint.val.y = endBzPoint.pos.y * factor;
        endBzPoint.val.z = endBzPoint.pos.z * factor;
        mAttackPointList.Add(endBzPoint);
        mBezRunTime = 0.0f;
    }
    private Vector3 CalcBezResult(float t)
    {
        int pointNum = mAttackPointList.Count;
        float xVal = 0;
        float yVal = 0;
        int bezNum = pointNum - 1;
        for (int idx = 0; idx < pointNum; idx++)
        {
            tBezPoint tmp = (tBezPoint)mAttackPointList[idx];
            xVal += tmp.val.x * Mathf.Pow((1 - t), bezNum - idx) * Mathf.Pow(t, idx);
            yVal += tmp.val.y * Mathf.Pow((1 - t), bezNum - idx) * Mathf.Pow(t, idx);
        }
        Vector3 rc = new Vector3(xVal, yVal, 0.0f);
        return rc;
    }
    private bool GetFirePlanePosition(out Vector3 pos)
    {
        GameObject firePlane = GameObject.Find("FirePlane");
        if (null != firePlane)
        {
            FirePlaneControl pc = firePlane.GetComponent<FirePlaneControl>();
            if (null != pc)
            {
                pos = pc.GetCurrentPosition();
                return true;
            }
        }
        pos = new Vector3();
        return false;
    }
    private void Attack_Beze_move()
    {
        tBezPoint attackBzObj = (tBezPoint)(mAttackPointList[mAttackPointList.Count - 1]);
        if (IsPosSame(attackBzObj.pos, transform.position))
        {
            mMoveState = EmoveState.RETRUN_HOME_move;
            int bezNum = Random.Range(5, 10);
            InitBezLine(bezNum, transform.position, mHomePos);
            return;
        }
        float moveSpeed = Mathf.Abs(Mathf.Sin(mBezRunTime));
        //float moveSpeed = mBezRunTime;
        Vector3 newPos = CalcBezResult(moveSpeed);
        transform.position = newPos; 
    }
    private void Return_Home_move()
    {
        Vector3 pos = transform.position;
        if (IsPosSame(pos, mHomePos))
        {
            mMoveState = EmoveState.SWING_move;
            return;
        }
        float moveSpeed = Mathf.Abs(Mathf.Sin(mBezRunTime));
        Vector3 newPos = CalcBezResult(moveSpeed);
        transform.position = newPos;
    }

    void Update()
    {
    }
    void FixedUpdate()
    {
        if (isEnemyDestroy)
        {
            Destroy(gameObject);
            isEnemyDestroy = false;
            return;
        }
        mBezRunTime += Time.deltaTime;
        switch (mMoveState)
        {
            case EmoveState.ATTACK_move:
                {
                    Attack_Beze_move();
                    break;
                }
            case EmoveState.RETRUN_HOME_move:
                {
                    Return_Home_move();
                    break;
                }
            case EmoveState.SWING_move:
                {
                    HorizSwing_Move();
                    break;
                }
            default:
                break;
        }
    }

    public void StartAttack()
    {
        if (mMoveState == EmoveState.SWING_move)
        {
            Vector3 startPos = transform.position;
            if (GetFirePlanePosition(out Vector3 endPos))
            {
                mMoveState = EmoveState.ATTACK_move;
                int bezNum = Random.Range(5, 10);
                InitBezLine(bezNum, startPos, endPos);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string collider_tag = collision.gameObject.tag;
        if (collider_tag == "Bullet")
        {
            MainControl.instance.AudioPlay(mBombClip, 3.0f);
            anim = GetComponent<Animator>();
            anim.Play("explode");
            Destroy(collision.gameObject);
        }
        return;
    }

    public void AfterExplode(int delFlag)
    { 
        if(delFlag > 0)
        {
            Destroy(gameObject);
        }
    }

}
