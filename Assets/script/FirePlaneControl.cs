using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirePlaneControl : MonoBehaviour
{
    private float moveSpeed = 5.0f;
    private float firePlaneTop_Y = -2.0f;

    private float mLeftPos;
    private float mRightPos;
    private float mTopPos;
    private float mBottomPos;

    private float mFireBallTime;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 leftBtm_cornerPos = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f,
            Mathf.Abs(-Camera.main.transform.position.z))); //这里的z轴在正交视图下意义不大
        Vector3 rightTop_cornerPos = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f,
                    Mathf.Abs(-Camera.main.transform.position.z)));
        mLeftPos = leftBtm_cornerPos.x;
        mRightPos = rightTop_cornerPos.x;
        mTopPos = rightTop_cornerPos.y;
        mBottomPos = leftBtm_cornerPos.y;

        mFireBallTime = 0;
    }

    private void MoveControl()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 pos = transform.position;
        pos.x += horizontalInput * moveSpeed * Time.deltaTime;
        pos.y += verticalInput * moveSpeed * Time.deltaTime;
        if (pos.y > firePlaneTop_Y)
        {
            pos.y = firePlaneTop_Y;
        }
        if (pos.y <= mBottomPos)
        {
            pos.y = mBottomPos;
        }
        if (pos.x >= mRightPos)
        {
            pos.x = mRightPos;
        }
        if (pos.x <= mLeftPos)
        {
            pos.x = mLeftPos;
        }
        transform.position = pos;
    }

    private void FireControl()
    {
        if (Input.GetMouseButton(0) || Input.GetAxis("Fire1") > 0 || Input.GetAxis("Fire2") > 0 || Input.GetAxis("Fire3") > 0)
        {
           
            float currentTime = Time.time;
            if (currentTime - mFireBallTime < 0.3)
            {
                return; 
            }
            mFireBallTime = currentTime;
            GameObject fireBall = (GameObject)Resources.Load("GamePrefabs\\FireBall");
            fireBall = Instantiate(fireBall);
            fireBall.name = "fireBall";
            fireBall.transform.position = transform.position;
            FireBallControl fc = fireBall.GetComponent<FireBallControl>();
            if (fc != null)
            {
                fc.InitFireParameter(3.0f);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        MoveControl();
        FireControl();
    }

    public Vector3 GetCurrentPosition()
    {
        return transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Destroy(gameObject);
            Destroy(collision.gameObject);
            SceneManager.LoadScene(0);
        }
        return;
    }

}
