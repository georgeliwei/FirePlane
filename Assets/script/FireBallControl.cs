using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallControl : MonoBehaviour
{
    private float mMoveSpeed;

    private float mTopPos;

    public void InitFireParameter(float moveSpeed)
    {
        mMoveSpeed = moveSpeed;
    }
    // Start is called before the first frame update
    void Start()
    {
        mMoveSpeed = 6.0f;
        Vector3 rightTop_cornerPos = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f,
                    Mathf.Abs(-Camera.main.transform.position.z)));
        mTopPos = rightTop_cornerPos.y;
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        pos.y += mMoveSpeed * Time.deltaTime;
        transform.position = pos;

        if (transform.position.y > mTopPos + 1.0f)
        {
            Destroy(gameObject);
        }
    }
}
