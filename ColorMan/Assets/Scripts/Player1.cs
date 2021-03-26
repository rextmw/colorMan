using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System ;
using UnityEngine.UI;


public class Player1 : MonoBehaviour
{

   
    int[,] field;
    int Y=GO.Y, X=GO.X;
    public GameObject[,] mapGO;
    public string colorName;
    public Transform target;
    public float velocity = 0.01f;
    public Transform stepObj;
    public int colorN;
    Color colorPlayer;

    public List<Vector3> steps;

    GO go;
    public int skill;

    private void Awake()
    {
     
    }
    // Start is called before the first frame update
    void Start()
    {
        
        go = GameObject.Find("grid").GetComponent<GO>();
        if (colorN == 0)
        {
            colorPlayer = Color.blue;
            colorName = "blue";
            
        } else if (colorN == 1)
        {
            colorPlayer = Color.yellow;
            colorName = "yellow";
        }  else if (colorN == 2)
        {
            colorPlayer = Color.green;
            colorName = "green";
        } 
        skill = PlayerPrefs.GetInt(colorN.ToString() + "Skill", 0);
        velocity += skill * .01f;
       
        transform.GetComponent<Renderer>().material.color = colorPlayer;
        transform.GetChild(0).GetComponent<Renderer>().material.color = colorPlayer;

        target = transform.GetChild(0);
        target.parent = null;
        field = GO.field;
      
    }

   
    // Update is called once per frame
    void Update()
    {
        
    }
 
    public void play()
    {
            int[,] field1 = findPath();        
    }


    public int[,] findPath()
    {
        bool add = true;
        int[,] cMap = new int[Y, X];
        int x, y, step = 0;

        for (y = 0; y < Y; y++)
        {
            for (x = 0; x < X; x++)
            {

                if (field[y, x] == 2 | field[y,x]==1)
                    cMap[y, x] = -2; 
                else cMap[y, x] = -1; 
            }
        }
     
        //начинаем отсчет с старта так будет удобней востанавливать путь
        cMap[(int)transform.position.z, (int)transform.position.x] = 0;
   
        int n = 0, l= 0;
   
        while (add == true)
        {
            add = false;
           
            for (x = 0; x < Y-1; x++)
            {
                for (y = 0; y < X-1; y++)
                {
                    if (cMap[x, y] == step)
                    { 
                        // если соседняя клетка не стена, и если она еще не помечена
                        // то помечаем ее значением шага + 1
                        if (y - 1 >= 0 && cMap[x, y - 1] != -2 && cMap[x, y - 1] == -1)
                        {
                            cMap[x, y - 1] = step + 1;
                         
                        }

                        if (x - 1 >= 0 && cMap[x - 1, y] != -2 && cMap[x - 1, y] == -1)
                        {
                            cMap[x - 1, y] = step + 1;
                   
                        }
                        if (y + 1 >= 0 && cMap[x, y + 1] != -2 && cMap[x, y + 1] == -1)
                        {
                            cMap[x, y + 1] = step + 1;
                 
                        }
                       
                        if (x + 1 >= 0 && cMap[x + 1, y] != -2 && cMap[x + 1, y] == -1)
                        {
                            cMap[x + 1, y] = step + 1;
                      
                        }
                        n = x;
                        l = y;
                    }
                }
            }
            step++;
            add = true;
          
       
            if (cMap[(int)target.position.z, (int)target.position.x]>0) //решение найдено
                add = false;
            if (step > GO.X * GO.Y) //решение не найдено, если шагов больше чем клеток
                add = false;
        }

        printMatrix(cMap);
        StartCoroutine(mov(cMap));
        return cMap;
    }

    IEnumerator mov(int[,] cMap)
    {
       
        int[] neighbors = new int[8]; //значение весов соседних клеток
                                      // будем хранить в векторе координаты клетки в которую нужно переместиться
        Vector3 moveTO = new Vector3(-1, 0, 10);
        Vector3 currentPosition = new Vector3(target.position.z, 0, target.position.x);

        bool endPanth = false;
        int o = cMap[(int)currentPosition.x, (int)currentPosition.z] - 1;
        while (!endPanth)
        {
            neighbors[0] = cMap[(int)currentPosition.x + 1, (int)currentPosition.z + 1];
            neighbors[1] = cMap[(int)currentPosition.x, (int)currentPosition.z + 1];
            neighbors[2] = cMap[(int)currentPosition.x - 1, (int)currentPosition.z + 1];
            neighbors[3] = cMap[(int)currentPosition.x - 1, (int)currentPosition.z];
            neighbors[4] = cMap[(int)currentPosition.x - 1, (int)currentPosition.z - 1];
            neighbors[5] = cMap[(int)currentPosition.x, (int)currentPosition.z - 1];
            neighbors[6] = cMap[(int)currentPosition.x + 1, (int)currentPosition.z - 1];
            neighbors[7] = cMap[(int)currentPosition.x + 1, (int)currentPosition.z];
            for (int i = 0; i < 8; i++)
            {

                if (neighbors[i] < 0)
                    // если клетка является непроходимой, делаем так, чтобы на нее юнит точно не попал            
                    neighbors[i] = 99999;
            }
            Array.Sort(neighbors); //первый элемент массива будет вес клетки куда нужно двигаться

            Vector3 vect = new Vector3(1, 0, 1);
            for (int x = (int)currentPosition.x - 1; x <= (int)currentPosition.x + 1; x++)
            {
                for (int y = (int)currentPosition.z + 1; y >= (int)currentPosition.z - 1; y--)
                {

                    if (cMap[x, y] == neighbors[0])
                    {                     
                        vect = new Vector3(y, 3, x);  
                    }
                }
            }

     
     
            yield return new WaitForSeconds(0.1f);
            if (vect.x == transform.position.x && vect.z == transform.position.z)
            {
                endPanth = true;
            }
            Transform trail = Instantiate(stepObj, vect, Quaternion.identity);
            trail.GetComponent<Renderer>().material.color = colorPlayer;
            steps.Add(vect);
            currentPosition = new Vector3(vect.z, 0, vect.x);
        
        }
    
        StartCoroutine(moveTo());
    }
    public static void printMatrix(int[,] map)
    {
        string arr = "";
        for (int i = 0; i < GO.Y; i++)
        {
            for (int j = 0; j < GO.X; j++)
            {

                arr += map[i, j] + ",";
            }
            arr += "\n";
        }
        print(arr);
    }

    IEnumerator moveTo()
    {
        steps.Reverse();
        steps.Add(target.position);
        yield return new WaitForSeconds(1);
        for (int i = 0; i< steps.Count; i++)
        {
           
            Vector3 posTarget = steps[i];
            while (Vector3.Distance(transform.position, posTarget) > 0.1f)
            {

                transform.position = Vector3.MoveTowards(transform.position, posTarget, velocity);
                yield return new WaitForSeconds(.01f);

            }
          
        }

        gameOver();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == colorName)
        {
            Destroy(other.gameObject);
            go.UpdateCoinsCounter(1);
        }
       
    }

    void gameOver()
    {
        go.Finish();
    }
}
