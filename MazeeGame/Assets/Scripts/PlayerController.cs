using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections; // 用于 Coroutine（协程）
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;  // 移动速度
    public int maxHealth = 3; // 最大血量
    private int currentHealth; // 当前血量
    private Rigidbody rb;  // 刚体组件
    public TMP_Text healthText; // UI 文字（在 Inspector 中拖动绑定）
    public GameObject gameOverPanel; // 游戏结束 UI 面板
    public Button restartButton; // TMP Button 仍然是 UnityEngine.UI.Button
    public GameObject victoryPanel;
    public Button victoryRestartButton;

    private Renderer playerRenderer;
    private Color currentColor;
    private bool gameWon = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();  // 获取刚体组件
        currentHealth = maxHealth;
        UpdateHealthUI();

        gameOverPanel.SetActive(false); // 默认隐藏游戏结束 UI
        victoryPanel.SetActive(false);

        playerRenderer = GetComponent<Renderer>(); // 获取 Renderer
        currentColor = playerRenderer.material.color; // 记录初始颜色

        // 绑定 TMP 按钮的点击事件
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        if (victoryRestartButton != null)
        {
            victoryRestartButton.onClick.AddListener(RestartGame);
        }
    }

    void Update()
    {
        // 获取键盘输入（WASD / 方向键）
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // // 计算移动方向
        // Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        // // 施加力，让球滚动
        // rb.AddForce(movement * speed);

        // 计算移动向量
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical) * speed * Time.deltaTime;

        // 直接移动
        transform.Translate(movement, Space.World);
        if (!gameWon && transform.position.x > 6 && transform.position.z > 1.2)
        {
            Victory();
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall")) // 确保墙的 Tag 设置为 "Wall"
        {
            Renderer wallRenderer = collision.gameObject.GetComponent<Renderer>();

            if (wallRenderer != null && wallRenderer.material.color == currentColor)
            {
                Physics.IgnoreCollision(collision.collider, GetComponent<Collider>(), true);
            }
            else
            {
                TakeDamage();
                // StartCoroutine(RotateWallSmoothly(collision.gameObject)); // 碰撞后让墙平滑移动
            }

        }
        else if (collision.gameObject.CompareTag("MovingWall"))
        {
            StartCoroutine(MoveWallSmoothly(collision.gameObject));
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pill")) // 碰到药丸
        {
            CollectPill(other.gameObject);
        }
    }

    void CollectPill(GameObject pill)
    {
        Renderer pillRenderer = pill.GetComponent<Renderer>();
        if (pillRenderer != null)
        {
            currentColor = pillRenderer.material.color; // 变成药丸的颜色
            playerRenderer.material.color = currentColor;
        }

        Destroy(pill); // 收集后销毁药丸
    }

    // 扣血逻辑
    void TakeDamage()
    {
        currentHealth--; // 扣 1 滴血
        UpdateHealthUI(); // 更新 UI
        Debug.Log("Player took damage! Current Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    IEnumerator RotateWallSmoothly(GameObject wall)
    {
        float duration = 0.5f; // 旋转持续时间
        float elapsedTime = 0;

        // 生成一个随机角度（90° / 180° / 270°，或者完全随机）
        float randomRotationAngle = Random.Range(90f, 270f); // 可以调整范围
        Quaternion startRotation = wall.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, wall.transform.eulerAngles.y + randomRotationAngle, 0);

        while (elapsedTime < duration)
        {
            wall.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        wall.transform.rotation = targetRotation; // 确保最终旋转角度正确
    }

    // 更新 UI 显示
    void UpdateHealthUI()
    {
        healthText.text = "Health: " + currentHealth;

    }

    void GameOver()
    {
        Debug.Log("Game Over!");
        gameOverPanel.SetActive(true); // 显示游戏结束 UI
        Time.timeScale = 0; // 暂停游戏
    }

    void Victory() // 🎉 胜利处理
    {
        gameWon = true; // 避免 Victory 多次触发
        Debug.Log("🎉 Victory!");
        victoryPanel.SetActive(true); // 显示胜利 UI
        Time.timeScale = 0; // 暂停游戏
    }

    // 重启游戏
    void RestartGame()
    {
        Time.timeScale = 1; // 恢复游戏速度
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // 重新加载当前场景
    }

    private Dictionary<GameObject, bool> wallMoveStates = new Dictionary<GameObject, bool>();

    IEnumerator MoveWallSmoothly(GameObject wall)
    {
        float moveDistance = 2.1f; // 墙移动的距离
        float moveSpeed = 2f; // 移动速度
        float duration = moveDistance / moveSpeed; // 计算移动时间

        Vector3 targetPosition;
        Vector3 originalPosition = wall.transform.position;

        // 获取墙的当前移动状态（如果墙不在字典里，默认向上移动）
        bool moveUp = !wallMoveStates.ContainsKey(wall) || wallMoveStates[wall];

        // 计算目标位置
        targetPosition = moveUp
            ? originalPosition + new Vector3(0, 0, moveDistance)  // 向上移动
            : originalPosition - new Vector3(0, 0, moveDistance); // 向下移动

        // 记录新状态
        wallMoveStates[wall] = !moveUp;

        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            wall.transform.position = Vector3.Lerp(originalPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        wall.transform.position = targetPosition; // 确保最终位置正确
    }

}
