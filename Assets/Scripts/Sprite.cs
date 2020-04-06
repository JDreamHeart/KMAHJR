﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[AddComponentMenu("GameScripts/Sprite")]

public class Sprite : MonoBehaviour
{
    public float m_speed = 10; // 速度值

    public float m_offsetY = 100; // 纵轴偏移值

    Rigidbody2D m_rigidbody2D;

    Direction m_direction = Direction.Unknown; // 方向

    bool m_isJumping; // 是否正处于跳跃状态

    int m_jumpCount = 0; // 跳跃次数
    int m_jumpCntActualLimit = 2; // 跳跃次数实际限制
    public int m_jumpCountLimit = 2; // 跳跃次数限制

    ParticleSystem m_fireParticle;
    ParticleSystem m_sparkParticle;
    public ParticleSystem m_sprayParticle;

    // Start is called before the first frame update
    void Start()
    {
        // 查找化节点或组件
        m_rigidbody2D = this.GetComponent<Rigidbody2D>();
        // 遍历子节点
        foreach (Transform trans in this.GetComponentsInChildren<Transform>()) {
            if(trans.name.CompareTo("Fire") == 0) {
                m_fireParticle = trans.GetComponent<ParticleSystem>();
            }
            else if(trans.name.CompareTo("Spark") == 0) {
                m_sparkParticle = trans.GetComponent<ParticleSystem>();
            }
        }
        // 初始化玩家
        OnRestartGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (isOutboundary()) {
            onDead();
            return;
        }
        if (m_direction == Direction.Unknown || !m_isJumping) {
            return;
        }
    }

    void updateFireFlip(float val) {
        ParticleSystemRenderer fireParticleRenderer = m_fireParticle.GetComponent<ParticleSystemRenderer>();
        Vector3 fireFlip = fireParticleRenderer.flip;
        fireFlip.y = val;
        fireParticleRenderer.flip = fireFlip;
    }

    // 转左
    public void TurnLeft() {
        m_direction = Direction.Left;
        updateFireFlip(1);
    }
    
    // 转右
    public void TurnRight() {
        m_direction = Direction.Right;
        updateFireFlip(0);
    }

    // 跳跃
    public void Jump() {
        int jumpCntLimit = Mathf.Max(m_jumpCntActualLimit, m_jumpCountLimit);
        if (m_jumpCount >= jumpCntLimit) {
            return;
        }
        // 重置状态
        m_isJumping = true;
        // 新增跳跃次数
        m_jumpCount += 1;
        // 更新速度
        float xSpeed = m_direction == Direction.Right ? m_speed : -m_speed;
        m_rigidbody2D.velocity = new Vector2(xSpeed * m_jumpCount / jumpCntLimit, 0.2f * m_speed * m_jumpCount / jumpCntLimit); // 设置速度
        // 将body type设为Dynamic
        if (m_rigidbody2D.bodyType != RigidbodyType2D.Dynamic) {
            m_rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        }
        updateFireFlip(0.5f);
        // 播放音效
        AudioClip clip = Resources.Load<AudioClip>("Sounds/Jump");
        AudioSource.PlayClipAtPoint(clip, Vector3.zero);
    }

    // 抓住
    public void Hold() {
        m_isJumping = false;
        m_jumpCount = 0;
        // 将精灵刚体类型设为Kinematic
        m_rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
    }

    void OnCollisionEnter2D(Collision2D collision) {
        Collider2D collider = collision.collider;
        if (collider.tag == "Board") {
            Board board = collider.GetComponent<Board>();
            Direction direction = board.IsRight() ? Direction.Right : Direction.Left;
            if (direction != m_direction) {
                return;
            }
            Hold(); // 抓住Board
            // 更新精灵的方向
            if (board.IsRight()) {
                TurnLeft();
            } else {
                TurnRight();
            }
            m_rigidbody2D.velocity = collider.GetComponent<Rigidbody2D>().velocity; // 设置速度
            // 播放音效
            AudioClip clip = Resources.Load<AudioClip>("Sounds/Hit");
            AudioSource.PlayClipAtPoint(clip, Vector3.zero);
        }
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag == "Reward") {
            Reward reward = collider.GetComponent<Reward>();
            if (collider.name.Contains("DoubleJump")) {
                reward.PlayDeadAnim(GameManager.Instance.GetSkillTrans(), () => {
                    SkillReward sr = reward.GetComponent<SkillReward>();
                    GameManager.Instance.AddSkill(sr);
                    sr.OnAddSkill();
                });
            } else {
                reward.PlayDeadAnim(GameManager.Instance.GetScoreTrans(), () => {
                    GameManager.Instance.AddScore(reward.GetScore());
                });
            }
            // 播放音效
            AudioClip clip = Resources.Load<AudioClip>("Sounds/GainReward");
            AudioSource.PlayClipAtPoint(clip, Vector3.zero);
        }
    }

    // 检测是否超出边界
    bool isOutboundary() {
        Vector2 pos = Camera.main.WorldToScreenPoint(this.transform.position);
        if (pos.x <= 0 || pos.x >= Screen.width || pos.y <= -m_offsetY || pos.y >= Screen.height + m_offsetY) {
            return true;
        }
        return false;
    }

    void onDead() {
        Vector2 pv = -m_rigidbody2D.velocity; // 获取速度的反方向
        m_rigidbody2D.velocity = new Vector2(0, 0); // 重置主角的速度
        // 兼容速度为0的情况
        if (pv.magnitude == 0) {
            pv = m_direction == Direction.Right ? new Vector2(0, -1) : new Vector2(0, 1);
        }
        // 播放粒子效果
        ParticleSystem.ShapeModule shape = m_sprayParticle.shape;
        Vector3 shapeRot = shape.rotation;
        float angle = Vector2.Angle(pv, new Vector2(0, 1));
        if (pv.x < 0) {
            angle = -angle;
        }
        shapeRot.y = angle;
        shape.rotation = shapeRot;
        m_sprayParticle.GetComponent<Transform>().position = this.transform.position;
        m_sprayParticle.Play();
        // 隐藏主角
        this.gameObject.SetActive(false);
        this.transform.position = new Vector3(0, 0, 0);
        // 播放音效
        AudioClip clip = Resources.Load<AudioClip>("Sounds/DeadElectricity");
        AudioSource.PlayClipAtPoint(clip, Vector3.zero);
    }

    public void OnStartGame() {
        m_jumpCount = 0;
        Jump();
    }
    
    public void OnRestartGame() {
        this.gameObject.SetActive(true);
        // 将精灵刚体类型设为Kinematic
        m_rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        // 重置实际跳跃次数限制
        m_jumpCntActualLimit = m_jumpCountLimit;
    }

    // 更新跳跃次数限制
    public void UpdateJumpCountLimit(int limit) {
        if (limit > 0) {
            m_jumpCntActualLimit = limit;
        }
    }
}
