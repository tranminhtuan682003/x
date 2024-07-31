using Fusion;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;
    public List<NetworkTransform> rubys;
    public Transform centerPoint;
    public Vector3 cubeSize = new Vector3(1f, 1f, 1f);
    public List<NetworkTransform> targets;
    public NetworkTransform demon;
    private int score;
    public TextMeshProUGUI scoretext;
    public TextMeshProUGUI scoreboardText;
    public float moveDistance = 3f;
    public float moveDuration = 2f;

    public GameObject sceneLoad;
    public Slider loadSlider;



    private bool arried;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void Start()
    {
        score = 0;
        loadSlider.value = 0f;
        sceneLoad.SetActive(true);
    }

    private void Update()
    {
        ItemRotation();
        ReActiveRuby();
        LoadScene();
    }

    private void ItemRotation()
    {
        foreach (var ruby in rubys)
        {
            ruby.transform.Rotate(Vector3.up, 30f * Time.deltaTime);
        }
    }

    private void ReActiveRuby()
    {
        foreach (var ruby in rubys)
        {
            if (!ruby.gameObject.activeInHierarchy)
            {
                Debug.Log($"Reactivating {ruby.gameObject.name}");
                StartCoroutine(ActiveRuby(ruby.gameObject));
            }
        }
    }

    IEnumerator ActiveRuby(GameObject ruby)
    {
        yield return new WaitForSeconds(10f);
        Debug.Log($"Setting active: {ruby.name}");
        ruby.SetActive(true);
    }

    private void OnDrawGizmos()
    {
        if (centerPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(centerPoint.position, cubeSize);
        }
    }

    public void SetPositionTarget()
    {
        foreach (var item in targets)
        {
            Vector3 randomPosition = centerPoint.position + new Vector3(
                Random.Range(-cubeSize.x / 2f, cubeSize.x / 2f),
                cubeSize.y,
                Random.Range(-cubeSize.z / 2f, cubeSize.z / 2f)
            );
            item.transform.position = randomPosition;
        }
    }

    private IEnumerator DemonAppear()
    {
        float elapsedTime = 0;
        Vector3 startingPos = demon.transform.position;
        Vector3 targetPos = startingPos + Vector3.up * moveDistance;

        while (elapsedTime < moveDuration)
        {
            demon.transform.position = Vector3.Lerp(startingPos, targetPos, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        demon.transform.position = targetPos;
    }

    public void Score()
    {
        score += 100;
        scoretext.text = score.ToString();
    }

    private void LoadScene()
    {
        loadSlider.value += Random.Range(0.0005f, 0.015f);
        if (loadSlider.value >= 1)
        {
            sceneLoad.SetActive(false);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            if (!arried)
            {
                StartCoroutine(DemonAppear());
            }
            arried = true;
        }
    }
}