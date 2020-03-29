using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("GameScripts/Sprite")]

public class Sprite : MonoBehaviour
{
    public float m_speed = 3; // 速度值

    Direction m_direction = Direction.Unknown; // 方向

    bool m_jumping; // 是否正处于跳跃状态

    ParticleSystem m_fireParticle;
    ParticleSystem m_sparkParticle;

    // Start is called before the first frame update
    void Start()
    {
        // 将精灵刚体类型设为Kinematic
        this.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        // 遍历子节点
        foreach (Transform trans in this.GetComponentsInChildren<Transform>()) {
            if(trans.name.CompareTo("Fire") == 0) {
                m_fireParticle = trans.GetComponent<ParticleSystem>();
            }
            else if(trans.name.CompareTo("Spark") == 0) {
                m_sparkParticle = trans.GetComponent<ParticleSystem>();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_direction == Direction.Unknown || !m_jumping) {
            return;
        }
        if (Input.GetMouseButtonDown(0)) {
            Jump();
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
        if (m_jumping) {
            return;
        }
        // 重置状态
        m_jumping = true;
        // 更新速度
        float xSpeed = m_direction == Direction.Right ? m_speed : -m_speed;
        this.GetComponent<Rigidbody2D>().velocity = new Vector2(xSpeed, m_speed * 0.5f); // 设置速度
        // 将body type设为Dynamic
        Rigidbody2D rigidbody = this.GetComponent<Rigidbody2D>();
        if (rigidbody.bodyType != RigidbodyType2D.Dynamic) {
            rigidbody.bodyType = RigidbodyType2D.Dynamic;
        }
        updateFireFlip(0.5f);
    }

    // 抓住
    public void Hold() {
        m_jumping = false;
        // 将精灵刚体类型设为Kinematic
        this.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
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
            this.GetComponent<Rigidbody2D>().velocity = collider.GetComponent<Rigidbody2D>().velocity; // 设置速度
        }
    }
}
