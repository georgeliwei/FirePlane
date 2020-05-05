using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainControl : MonoBehaviour
{
    public static MainControl instance = null;

    private AudioSource mAudioSource;

    private int max_row_num = 8;
    private float enemy_H_space = 0.8f;
    private float enemy_V_space = 0.6f;
    private float firePlane_X = 0.0f;
    private float firePlane_Y = -3.0f;

    private float maxAttackTimeInterval = 5.0f;
    private float attackStartTime = 0.0f;

    private ArrayList mEnemyArray;
    private ArrayList attackEnemy;

    private float mEnemyLastLiveCheck = 0.0f;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
    }
    
    private bool EnemyLiveCheck()
    {
        if (mEnemyArray == null)
        {
            mEnemyArray = new ArrayList();
            return false;
        }
        int liveEnemyCnt = 0;
        for (int idx = 0; idx < mEnemyArray.Count; idx++)
        {
            GameObject attackEnemy = (GameObject)mEnemyArray[idx];
            if (attackEnemy != null)
            {
                liveEnemyCnt += 1;
                break;
            }
        }
        if (liveEnemyCnt > 0)
        {
            return true;
        }
        return false;
    }


    private void CreateEnemy()
    {
        if (mEnemyArray == null)
        {
            mEnemyArray = new ArrayList();
        }
        for (int row = 0; row < max_row_num; row++)
        {
            int rowEnemyNum = row * 2 + 1;
            float left = -(rowEnemyNum / 2 * enemy_H_space);
            for (int col = 0; col < rowEnemyNum; col++)
            {
                GameObject enemy = (GameObject)Resources.Load("GamePrefabs\\EnemyPlane2");
                enemy = Instantiate(enemy);
                string enemy_name = "enemy_row_" + row.ToString() + "_col_" + col.ToString();
                enemy.name = enemy_name;
                float pos_y = enemy_V_space * row;
                float pos_x = left + enemy_H_space * col;
                enemy.transform.position = new Vector3(pos_x, pos_y, 0.0f);
                mEnemyArray.Add(enemy);
            }
        }
    }

    private void AttackPlayer()
    {
        float currentTime = Time.time;
        if (currentTime - attackStartTime > maxAttackTimeInterval)
        {
            //随机选择一个敌人
            int selectEnemy = Random.Range(0, mEnemyArray.Count);
            GameObject attackEnemy = (GameObject)mEnemyArray[selectEnemy];
            if (attackEnemy == null)
            {
                return;
            }
            EnemyControl pc = attackEnemy.GetComponent<EnemyControl>();
            if (pc != null)
            {
                pc.StartAttack();
            }
            //发起一次攻击
            attackStartTime = currentTime;
            maxAttackTimeInterval = Random.Range(1.0f, 10.0f);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        attackStartTime = Time.time;
        mAudioSource = GetComponent<AudioSource>();
        CreateEnemy();
        attackEnemy = new ArrayList();

        GameObject firePlane = (GameObject)Resources.Load("GamePrefabs\\FirePlane");
        firePlane = Instantiate(firePlane);
        firePlane.name = "FirePlane";
        firePlane.transform.position = new Vector3(firePlane_X, firePlane_Y, 0);
    }
    // Update is called once per frame
    void Update()
    {
        float currentTime = Time.time;
        if (currentTime - mEnemyLastLiveCheck > 5.0f)
        {
            mEnemyLastLiveCheck = currentTime;
            if (!EnemyLiveCheck())
            {
                CreateEnemy();
            }
        }
        AttackPlayer();
    }

    public void AudioPlay(AudioClip clip, float volumeScale)
    {
        mAudioSource = GetComponent<AudioSource>();
        mAudioSource.PlayOneShot(clip, volumeScale);
    }


}
