using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public GameObject traffic;
    public GameObject track;
    public GameObject player;
    public GameObject prefabRaceObstacle;
    public GameObject prefabTrack;
    public GameObject explosionPrefab;
    public GameObject imgArrowCrim;
    GameObject criminal;
    //public RoadArchitect.Road architectRoad;

    public GameObject[] canvasGamePlay;
    public GameObject[] canvasGameOver;
    public GameObject[] prefabsTraffic;
    public GameObject[] prefabsCriminalCar;

    static public List<GameObject> listTraffic = new List<GameObject>();

    public TextMeshProUGUI infoTxt;
    public TextMeshProUGUI infoTxt2;

    public AudioClip clipGame;
    private AudioSource audioClip;

    private int level = 1;
    private int allPath = 0;

    public float leftPos = -2f;
    public float rightPos = 2f;
    public float leftBord = -4f;
    public float rightBord = 4f;
    public float sizeTrack = 1000f;
    public float sizeHorizont = 120f;
    public float traffickDensity = 50f;
    public float startTimeSound = 3.0f;
    public float finishTimeSound = 4.0f;

    private bool isAddedTraffic;
    private bool isAddedTrack;
    private bool isAddedDistance;
    private bool isStopGame;
    private bool isNewTrack;
    bool isAddedCriminal;
    static public bool IsLose { get; set; }

    private List<int> listLevel = new List<int> { 0, 100, 220, 350, 500, 670, 860, 1070, 1300, 1550 };
    private List<int> listSpeed = new List<int> { 10, 12, 15, 18, 20, 25, 30, 35, 40, 50 };

    void Start()
    {
        isNewTrack = true;

        audioClip = gameObject.AddComponent<AudioSource>();
        audioClip.clip = clipGame;
        //audioClip.Play();
        //setInfoSpeedText(level);
    }

    void debugTest()
    {
        //float dist = architectRoad.RoadWidth();
        //Debug.Log("Width" + dist);
    }

    void Update()
    {
        if (isStopGame) return;

        int pos = (int)player.transform.position.z;

        if (pos % 10 == 0) {
            if (!isAddedDistance) {
                if (pos != 0) allPath += 10;
                infoTxt2.text = "Distanse:\n" + allPath;
                isAddedDistance = true;

                //if(level < listLevel.Count && allPath >= listLevel[level]) {
                //    setSpeed(level++);
                //}
            }
        }
        else isAddedDistance = false;

        if ((pos % (int)traffickDensity) == 0) {
            if (!isAddedTraffic) {
                //addTraffic(player.transform.position.z + sizeHorizont);
                isAddedTraffic = true;
            }
        }
        else
            isAddedTraffic = false;

        if(isNewTrack) {
            if (pos % 30 == 0 && pos != 0) {
                if (!isAddedTrack) {
                    //addTrack((int)player.transform.position.z + sizeHorizont);
                    isAddedTrack = true;
                }
            }
            else
                isAddedTrack = false;
        }

        if (pos >= sizeTrack - 100)
        {
            //updataTrack();
            //Debug.Log("Updata position " + pos + " " + sizeTrack);
        }

        if(allPath >= 50 && !isAddedCriminal) {
            //addCriminalCar(player.transform.position.z + 10);
        }

        if (criminal && (criminal.transform.position.z - player.transform.position.z) > 100)
        {
            Debug.Log("Happened something. Position: " + criminal.transform.position.z);
            criminal.transform.position = new Vector3(criminal.transform.position.x, criminal.transform.position.y, player.transform.position.z + 50);
        }

        //if (!IsLose && !audioClip.isPlaying) audioClip.Play();

        if (audioClip.isPlaying && audioClip.time >= finishTimeSound)
        {
            audioClip.time = startTimeSound;
            audioClip.Play();
        }

        if (IsLose) {
            for(int i = 0; i < canvasGamePlay.Length; i++) {
                canvasGamePlay[i].SetActive(false);
            }

            for (int i = 0; i < canvasGameOver.Length; i++) {
                canvasGameOver[i].SetActive(true);
            }

            isStopGame = true;
            isNewTrack = true;

            //audioClip.Stop();
        }
        else if (isStopGame) {
            isStopGame = false;
            //audioClip.Play();
        }
    }

    private void addTraffic(float posOnTrack)
    {
        //Checking free place
        foreach (var car in listTraffic) {
            if (Mathf.Abs(posOnTrack - car.transform.position.z) < traffickDensity)
                return;
        }

        //Adding traffic
        int pos = Random.Range(0, 2);
        float xPos = pos == 0 ? leftPos : rightPos;

        GameObject sample = prefabsTraffic[Random.Range(0, prefabsTraffic.Length)];

        GameObject newObj = Instantiate(sample, new Vector3(xPos, 1, posOnTrack), Quaternion.identity, traffic.transform);
        newObj.AddComponent<BoxCollider>();
        newObj.GetComponent<Rigidbody>().mass = 10000;
        newObj.AddComponent<TrafficMoving>();
        listTraffic.Add(newObj);

        //Removing pass traffic
        for (int i = 0; i < listTraffic.Count; i++) {
            if ((player.transform.position.z - listTraffic[i].transform.position.z) > 10) {
                GameObject objRemove = listTraffic[i--];
                listTraffic.Remove(objRemove);
                Destroy(objRemove);
            }
        }
    }

    private void addTrack(float posOnTrack) 
    {
        Instantiate(prefabTrack, new Vector3(-4f, -1.5f, posOnTrack), Quaternion.identity, track.transform);
    }

    void addCriminalCar(float posOnTrack)
    {
        //Checking free place
        //foreach (var car in listTraffic) {
        //    if (Mathf.Abs(posOnTrack - car.transform.position.z) < 5)
        //        return;
        //}

        //Adding criminal
        int pos = Random.Range(0, 2);
        float xPos = pos == 0 ? leftPos : rightPos;

        GameObject sample = prefabsCriminalCar[level - 1];

        GameObject newObj = Instantiate(sample, new Vector3(xPos, 1, posOnTrack), Quaternion.identity);
        newObj.AddComponent<BoxCollider>();
        newObj.GetComponent<Rigidbody>().mass = 100000;
        newObj.AddComponent<CriminalMoving>();
        criminal = newObj;
        CriminalMoving.explosionPrefab = explosionPrefab;
        ArrowTargetMoving.target = criminal.transform;
        newObj.tag = "Target";
        //imgArrowCrim.SetActive(true);

        isAddedCriminal = true;

        Debug.Log("Added criminal");
    }

    void updataTrack()
    {
        float posPlayer = player.transform.position.z;
        player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, 0);

        foreach (var car in listTraffic)
        {
            float distance = car.transform.position.z - posPlayer;
            car.transform.position = new Vector3(car.transform.position.x, car.transform.position.y, distance);
        }

        if(criminal)
        {
            float distance = criminal.transform.position.z - posPlayer;
            criminal.transform.position = new Vector3(criminal.transform.position.x, criminal.transform.position.y, distance);
        }

        isNewTrack = false;
    }

    private void setInfoSpeedText(float speed)
    {
        //infoTxt.text = "Level: " + lev + "\n" + "Speed: " + sp + " km/h";
        infoTxt.text = speed.ToString();
    }

    private void setSpeed(int lev)
    {
        //if (lev >= listLevel.Count) return;

        //PlayerMovement.speed = listSpeed[lev];
        //speed = (int)PlayerMovement.speed;
        //setInfoText(lev, speed);
    }
}
