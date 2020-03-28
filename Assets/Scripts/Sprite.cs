using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction {
    Unknown = 0,
    Left = 1,
    Right = 2,
    Up = 3,
    Down = 4,
}

[AddComponentMenu("GameScripts/Sprite")]

public class Sprite : MonoBehaviour
{
    public float m_fource = 200; // 施加力值

    Direction m_direction = Direction.Unknown; // 方向

    bool m_jumping; // 是否正处于跳跃状态

    // Start is called before the first frame update
    void Start()
    {
        // 将精灵刚体类型设为Kinematic
        this.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
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

    // 转左
    public void TurnLeft() {
        m_direction = Direction.Left;
    }
    
    // 转右
    public void TurnRight() {
        m_direction = Direction.Right;
    }

    // 跳跃
    public void Jump() {
        // 重置状态
        m_jumping = true;
        // 将body type设为Dynamic
        Rigidbody2D rigidbody = this.GetComponent<Rigidbody2D>();
        if (rigidbody.bodyType != RigidbodyType2D.Dynamic) {
            rigidbody.bodyType = RigidbodyType2D.Dynamic;
        }
        // 施加一个力
        float xFource = m_direction == Direction.Right ? m_fource : -m_fource;
        rigidbody.AddForce(new Vector2(xFource * 2, m_fource));
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
            Hold(); // 抓住Board
            this.GetComponent<Rigidbody2D>().velocity = collider.GetComponent<Rigidbody2D>().velocity; // 设置速度
        }
    }
}
