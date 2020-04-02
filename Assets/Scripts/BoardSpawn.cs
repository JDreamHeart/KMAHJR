using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("GameScripts/BoardSpawn")]

public class BoardSpawn : MonoBehaviour
{
    public Transform m_boardPrefab;

    public bool m_isRight;

    float m_seconds = 3f;
    
    float m_velocity = 3f;

    List<Board> m_rcBoards = new List<Board>();
    
    List<Board> m_activeBoards = new List<Board>();

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnBoard());
    }

    // Update is called once per frame
    void Update()
    {
        
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
            m_activeBoards.Add(board);
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

    List<Vector3> GetLightningPosList() {
        float height = this.GetComponent<RectTransform>().rect.height;
        Vector3 pos = Camera.main.WorldToScreenPoint(this.transform.position);
        Vector3 startPos = Camera.main.ScreenToWorldPoint(new Vector3(pos.x, height, pos.z));
        Vector3 endPos = Camera.main.ScreenToWorldPoint(new Vector3(pos.x, 0, pos.z));
        if (m_isRight) {
            startPos = Camera.main.ScreenToWorldPoint(new Vector3(pos.x, 0, pos.z));
            endPos = Camera.main.ScreenToWorldPoint(new Vector3(pos.x, height, pos.z));
        }
        List<Vector3> posList = new List<Vector3>();
        Vector3 prePos = startPos;
        foreach (Board board in m_activeBoards) {
            if (m_isRight) {

            } else {

            }
        }
        return posList;
    }
}
