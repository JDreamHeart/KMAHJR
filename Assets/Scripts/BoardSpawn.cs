using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("GameScripts/BoardSpawn")]

public class BoardSpawn : MonoBehaviour
{
    public Transform m_boardPrefab;

    public Transform m_lightningPrefab;

    public bool m_isRight;

    public float m_lightningOffsetX;

    float m_seconds = 3f;
    
    float m_velocity = 3f;

    List<Board> m_rcBoards = new List<Board>();
    
    List<Board> m_activeBoards = new List<Board>();

    List<LightningChain> m_lightningList = new List<LightningChain>();

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnBoard());
    }

    // Update is called once per frame
    void Update()
    {
        // 创建闪电链
        List<Vector3[]> posList = getLightningPosList();
        createLightningList(posList);
    }
    
    public bool IsRight() {
        return m_isRight;
    }

    public void UpdateSeconds(float seconds) {
        m_seconds = seconds;
    }

    public void UpdateVelocity(float velocity) {
        m_velocity = velocity;
    }

    IEnumerator SpawnBoard() {
        while (true) {
            Board board = getBoard(); // 获取块状
            m_activeBoards.Insert(0, board);
            board.SetBoardSpawn(this);
            Rigidbody2D rigidbody2D = board.GetComponent<Rigidbody2D>();
            if (m_isRight) {
                rigidbody2D.velocity = new Vector2(0, m_velocity);
            } else {
                rigidbody2D.velocity = new Vector2(0, -m_velocity);
            }
            // 等待一段时间
            yield return new WaitForSeconds(m_seconds);
        }
    }

    // 获取块状（使用缓存）
    Board getBoard() {
        if (m_rcBoards.Count > 0) {
            Board board = m_rcBoards[0];
            m_rcBoards.RemoveAt(0);
            board.gameObject.SetActive(true);
            board.GetComponent<Transform>().position = transform.position; // 更新位置
            return board;
        }
        Transform trans = Instantiate(m_boardPrefab, transform.position, Quaternion.identity, transform) as Transform; // 生成块状实例
        return trans.GetComponent<Board>();
    }

    // 回收块状
    public void RecoverBoard(Board board) {
        board.gameObject.SetActive(false);
        m_rcBoards.Add(board);
        m_activeBoards.Remove(board);
    }

    List<Vector3[]> getLightningPosList() {
        Vector3 pos = Camera.main.WorldToScreenPoint(this.transform.position);
        Vector3 startPos = Camera.main.ScreenToWorldPoint(new Vector3(pos.x + m_lightningOffsetX, Screen.height, pos.z));
        Vector3 endPos = Camera.main.ScreenToWorldPoint(new Vector3(pos.x + m_lightningOffsetX, 0, pos.z));
        if (m_isRight) {
            startPos = Camera.main.ScreenToWorldPoint(new Vector3(pos.x + m_lightningOffsetX, 0, pos.z));
            endPos = Camera.main.ScreenToWorldPoint(new Vector3(pos.x + m_lightningOffsetX, Screen.height, pos.z));
        }
        List<Vector3[]> posList = new List<Vector3[]>();
        Vector3 lastPos = startPos;
        // Debug.Log(string.Format("getLightningPosList: {0}, {1}, {2}", m_isRight, startPos, endPos));
        // 判断板块点
        foreach (Board board in m_activeBoards) {
            float height = board.GetComponent<RectTransform>().rect.height;
            Vector3 bpos = Camera.main.WorldToScreenPoint(board.GetComponent<Transform>().position);
            if (m_isRight) {
                Vector3 prevPos = Camera.main.ScreenToWorldPoint(new Vector3(bpos.x + m_lightningOffsetX, bpos.y - height/2, bpos.z));
                if (prevPos.y > lastPos.y) {
                    posList.Add(new Vector3[]{lastPos, prevPos});
                }
                Vector3 tempPos = Camera.main.ScreenToWorldPoint(new Vector3(bpos.x + m_lightningOffsetX, bpos.y + height/2, bpos.z));
                if (tempPos.y > lastPos.y) {
                    lastPos = tempPos;
                }
                // Debug.Log(string.Format("getLightningPosList board: {0}, {1}, {2}", m_isRight, prevPos, lastPos));
            } else {
                Vector3 prevPos = Camera.main.ScreenToWorldPoint(new Vector3(bpos.x + m_lightningOffsetX, bpos.y + height/2, bpos.z));
                if (prevPos.y < lastPos.y) {
                    posList.Add(new Vector3[]{lastPos, prevPos});
                }
                Vector3 tempPos = Camera.main.ScreenToWorldPoint(new Vector3(bpos.x + m_lightningOffsetX, bpos.y - height/2, bpos.z));
                if (tempPos.y < lastPos.y) {
                    lastPos = tempPos;
                }
                // Debug.Log(string.Format("getLightningPosList board: {0}, {1}, {2}", m_isRight, prevPos, lastPos));
            }
        }
        // 判断结束点
        if (m_isRight) {
            if (endPos.y > lastPos.y) {
                posList.Add(new Vector3[]{lastPos, endPos});
            }
        } else {
            if (endPos.y < lastPos.y) {
                posList.Add(new Vector3[]{lastPos, endPos});
            }
        }
        return posList;
    }

    LightningChain getLightningChain() {
        if (m_lightningList.Count > 0) {
            LightningChain lc = m_lightningList[0];
            m_lightningList.RemoveAt(0);
            lc.gameObject.SetActive(true);
            return lc;
        }
        Transform trans = Instantiate(m_lightningPrefab, transform.position, Quaternion.identity, transform) as Transform; // 生成闪电实例
        return trans.GetComponent<LightningChain>();
    }

    void createLightningList(List<Vector3[]> posList) {
        // 更新闪电链对象
        List<LightningChain> lcList = new List<LightningChain>();
        foreach (Vector3[] pos in posList) {
            LightningChain lc = getLightningChain();
            lc.UpdatePosList(pos[0], pos[1]);
            lcList.Insert(0, lc);
        }
        // 隐藏已有对象
        foreach (LightningChain lc in m_lightningList) {
            lc.gameObject.SetActive(false);
        }
        // 更新m_lightningList
        foreach (LightningChain lc in lcList) {
            m_lightningList.Insert(0, lc);
        }
    }

}
