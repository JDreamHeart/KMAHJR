using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[AddComponentMenu("GameScripts/Sprite")]

public class Sprite : MonoBehaviour
{
    public float m_speed = 10; // 速度值
    float m_actualSpeed = 10; // 实际速度

    public float m_offsetY = 100; // 纵轴偏移值

    Rigidbody2D m_rigidbody2D;

    Direction m_direction = Direction.Unknown; // 方向

    bool m_isJumping; // 是否正处于跳跃状态

    int m_jumpCount = 0; // 跳跃次数
    int m_jumpCntActualLimit = 2; // 跳跃次数实际限制
    public int m_jumpCountLimit = 2; // 跳跃次数限制

    public float m_minGravity = 1; // 最小重力

    ParticleSystem m_fireParticle;
    ParticleSystem m_sparkParticle;
    public ParticleSystem m_sprayParticle;

    LineRenderer m_lineRender; // 线条渲染器

    public bool m_isShowTrajectory = false; // 是否显示运动轨迹
    bool m_isActualShowTrajectory = false; // 实际是否显示运动轨迹
    public int m_maxLinePointCount = 100; // 线条点个数

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
        // 初始化线条渲染器
        initLineRenderer();
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
        // 显示运动轨迹
        if (m_isActualShowTrajectory) {
            showTrajectory();
        }
        // 检测状态
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
        Vector2 velocity;
        m_rigidbody2D.gravityScale = getNextState(out velocity);
        m_rigidbody2D.velocity = velocity;
        // 将body type设为Dynamic
        if (m_rigidbody2D.bodyType != RigidbodyType2D.Dynamic) {
            m_rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        }
        // 新增跳跃次数
        m_jumpCount += 1;
        // 更新火焰粒子
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

    // 获取下一个状态【重力、速度】
    float getNextState(out Vector2 velocity) {
        velocity = m_rigidbody2D.velocity; // 初始化速度
        int jumpCntLimit = Mathf.Max(m_jumpCntActualLimit, m_jumpCountLimit);
        int diffCnt = jumpCntLimit - m_jumpCount - 1;
        if (diffCnt < 0) {
            return m_rigidbody2D.gravityScale;
        }
        float xSpeed = m_direction == Direction.Left ? -m_actualSpeed : m_actualSpeed;
        velocity = new Vector2(xSpeed * Mathf.Pow(0.2f, diffCnt), m_actualSpeed * diffCnt / jumpCntLimit); // 速度
        return m_minGravity * diffCnt; // 重力
    }

    void OnCollisionEnter2D(Collision2D collision) {
        Collider2D collider = collision.collider;
        if (collider.tag == "Board") {
            Board board = collider.GetComponent<Board>();
            Direction direction = board.IsRight() ? Direction.Right : Direction.Left;
            if (direction != m_direction) {
                return;
            }
            // 检测是否在板块的左右边界
            Vector3 pos = Camera.main.WorldToScreenPoint(this.transform.position);
            RectTransform rt = collider.GetComponent<RectTransform>();
            Vector3 cpos = Camera.main.WorldToScreenPoint(rt.transform.position);
            if (pos.x > cpos.x - rt.rect.width / 2 && pos.x < cpos.x + rt.rect.width / 2) {
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
            if (reward.enabled) {
                SkillReward sr = reward.GetComponent<SkillReward>();
                if (sr != null) {
                    reward.PlayDeadAnim(GameManager.Instance.GetSkillTrans(), () => {
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
    }

    // 检测是否超出边界
    bool isOutboundary() {
        Vector2 pos = Camera.main.WorldToScreenPoint(this.transform.position);
        if (pos.x <= 0 || pos.x >= Screen.width || pos.y <= -m_offsetY || (!m_isJumping && pos.y >= Screen.height + m_offsetY)) {
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
        m_rigidbody2D.velocity = new Vector2(0, 0);
        m_rigidbody2D.gravityScale = 0;
        // 重置当前跳跃次数
        m_jumpCount = 0;
        // 重置实际跳跃次数限制
        m_jumpCntActualLimit = m_jumpCountLimit;
        // 重置实际速度
        m_actualSpeed = m_speed;
        // 重置实际是否显示运动轨迹
        m_isActualShowTrajectory = m_isShowTrajectory;
    }

    // 更新跳跃次数限制
    public void UpdateJumpCountLimit(int limit) {
        if (limit > 0) {
            m_jumpCntActualLimit = limit;
        }
    }

    // 加速
    public void Accelerate(float speed) {
        m_actualSpeed += speed;
    }

    // 显示/隐藏运动轨迹
    public void ShowTrajectory(bool isShow = true) {
        m_isActualShowTrajectory = isShow;
    }

    // 初始化线条渲染器
    LineRenderer initLineRenderer() {
        m_lineRender = this.GetComponent<LineRenderer>();
        if (m_lineRender == null) {
            m_lineRender = this.gameObject.AddComponent<LineRenderer>();
        }
        // 线段宽度
        m_lineRender.startWidth = 0.05f;
        m_lineRender.endWidth = 0.1f;
        return m_lineRender;
    }

    // 显示运动轨迹
    void showTrajectory() {
        Vector2 velocity;
        float gravityScale = getNextState(out velocity);
        if (velocity.x != 0) {
            Vector2 lb = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));
            Vector2 rt = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
            float t = (rt.x - lb.x) / Mathf.Abs(velocity.x) / m_maxLinePointCount;
            float gravity = (Physics.gravity * gravityScale).y;
            Vector3 pos = this.transform.position;
            List<Vector3> posList = new List<Vector3>();
            Vector3 prevPos = this.transform.position, nextPos = Vector3.zero;
            posList.Add(prevPos);
            bool isBreak = false;
            for (int i = 1; i <= m_maxLinePointCount; i++) {
                float x = pos.x + velocity.x * t * i;
                float y = pos.y + velocity.y * t * i + 0.5f * gravity * Mathf.Pow(t * i, 2);
                if (x <= lb.x || x >= rt.x || y <= lb.y) {
                    isBreak = true;
                }
                nextPos = new Vector3(x, y, pos.z);
                if (!isBreak) {
                    // 通过射线检测碰撞
                    RaycastHit2D hitinfo = Physics2D.Raycast(prevPos, nextPos - prevPos, Vector2.Distance(nextPos, prevPos), LayerMask.GetMask("Board"));
                    if (hitinfo.collider != null) {
                        // nextPos = new Vector3(hitinfo.point.x, hitinfo.point.y, pos.z);
                        // isBreak = true;
                        break;
                    }
                }
                posList.Add(nextPos);
                if (isBreak) {
                    break;
                }
            }
            // 更新LineRenderer
            m_lineRender.positionCount = posList.Count;
            m_lineRender.SetPositions(posList.ToArray());
        }
    }
}
