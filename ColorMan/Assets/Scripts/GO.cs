using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GO : MonoBehaviour {

   
    public static int[,] field;
    public static int X = 12, Y = 10;
    Transform target;
   

    public GameObject block;
    public GameObject player;

    public Button buttonPlay;
    public Player1[] players;

    // будем кидать луч для расстановки целей 
    int layerMask = 0;
    RaycastHit hit;
    Ray ray = new Ray();

    public GameObject skillsPanel;
    public static int coins;
    public Text txtCoins;
    int outPlayer = 0;//игроки пришедшие к финишу
    bool move = false;
    // Start is called before the first frame update
    void Start()
    {
        //PlayerPrefs.DeleteAll();
        field = Levels.map[Application.loadedLevel];
        X = field.GetLength(1);
        Y = field.GetLength(0);
       
        UpdateCoinsCounter(0);
        coins = PlayerPrefs.GetInt("coins", 0);
        layerMask = 1 << 8;
      
        int n = 0;
        for (int y = 0; y < Y; y++)
        {
            for (int x = 0; x < X; x++)
            {
                if (field[y, x] == 2)
                {
                    Instantiate(block, new Vector3(x, transform.position.y + 1, y), Quaternion.identity);

                }
                else if (field[y, x] == 3)
                {
                    players[n] = Instantiate(player, new Vector3(x, transform.position.y + 1, y), Quaternion.identity).GetComponent<Player1>();
                    players[n].colorN = n;
                    players[n].name = "Player_" + n.ToString();
                    n++;
                }
            }
        }
    }

    //запускаем капсулы
    public void play()
    {
        buttonPlay.interactable = false;
        for (int i = 0; i < players.Length; i++)
        {
            move = true;
            players[i].play();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (move) return;
        //пускаем луч что бы расставить цели движения
        if (Input.GetMouseButton(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //выбираем нужную капсулу и таргет для нее
            if (target == null)
            {
                if (Physics.Raycast(ray, out hit, 100))
                {
                  
                    if (hit.transform.tag == "Player")
                        target = hit.transform.GetComponent<Player1>().target;

                }
            }
            else
            {
                //перемещаем выбранный таргет
                if (Physics.Raycast(ray, out hit, 100, layerMask))
                {
                    if (hit.transform.name == "grid")
                    {
                        Vector3 pos = new Vector3(Mathf.Round(hit.point.x), hit.point.y + .04f, Mathf.Round(hit.point.z));
                        
                        target.position = new Vector3(Mathf.Clamp(pos.x, 1, X-2), pos.y, Mathf.Clamp(pos.z, 1, Y-2));
                    }
                }
            }

        }

        //отпускаем мышь и обнуляем таргет
        if (Input.GetMouseButtonUp(0))
        {
            target = null;
            CheckTargets();
        }
    }

    //проверяем расстановку всех целей 
    void CheckTargets()
    {
      
        int n = 0;
        for (int i = 0; i < players.Length; i++)
        {
            float dist = Vector3.Distance(players[i].transform.position, players[i].target.position);
            if (dist > 1.1f)
            {
                n++;
            }
        }

        if (n >= players.Length)
        {
            if (!buttonPlay.IsInteractable())
            {
                buttonPlay.interactable = true;
            }
        }
        else
        {
            if (buttonPlay.IsInteractable())
            {
                buttonPlay.interactable = false;
            }
        }
    }

    //пересчитаем  доступные монеты и если надо активируем  скиллы
    public void UpdateCoinsCounter(int i)
    {
        coins = PlayerPrefs.GetInt("coins", 0);


        coins += i;
        if (coins < 0) coins = 0;

        PlayerPrefs.SetInt("coins", coins);

        txtCoins.text = coins.ToString();

        if (skillsPanel.activeSelf){

            bool boolShow = false;
            if (coins < 10)
            {
                boolShow = false;
            } else
            {
                boolShow = true;
            }

                for (int n = 0; n < 3; n++)
                {
                    skillsPanel.transform.GetChild(n).GetComponent<Button>().interactable = boolShow;
                    skillsPanel.transform.GetChild(n).GetChild(0).GetComponent<Text>().text = "ур. " + players[n].skill.ToString() + "+";
                }
            
        }
       
    }
    //повышаем уровень
    public void UpSkill(int i)
    {
        if (coins > 9)
        {
            int skill = PlayerPrefs.GetInt(i.ToString() + "Skill", 0);
            players[i].skill += 1;
            PlayerPrefs.SetInt(i.ToString() + "Skill", players[i].skill); 
            UpdateCoinsCounter(-10);
        }
    }

    //считаем количество дошедших до финиша и включаем панель повышений
    public void Finish()
    {
        outPlayer++;
        if (outPlayer > 2)
        {
            skillsPanel.SetActive(true);
            UpdateCoinsCounter(0);
        }
    }

    //загружаем следующий уровень, после последнего - первый
    public void loadNextLevel()
    {
        int i = Application.loadedLevel+1;
        
        if (i >= Levels.map.Length)
            i = 0;
        Application.LoadLevel(i);
    }

}
