using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [SerializeField] public List<Sprite> sprites = new List<Sprite>();

    [SerializeField] public List<Sprite> gridBackGoundSprites = new List<Sprite>();

    [SerializeField] public GameObject germPrefab;

    [SerializeField] public GameObject blockPrefab;

    [SerializeField] public int gridDimension = 8;

    [SerializeField] public float distance = 1.0f;

    private GameObject[,] Grid;

    public Text scoreText;

    private int germAmount;

    public GameObject ui;

    private int _score;
    
    public int Score
    {
        get => this._score;
        set
        {
            this._score = value;
            scoreText.text = "SCORE: \n" + this._score.ToString();
        }
    }
    
    private void Awake()
    {
        Instance = this;
 //       Score = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        Grid = new GameObject[gridDimension, gridDimension];
//        gameOverMenu.SetActive(false);
        IniGrid();
        germAmount = gridDimension * gridDimension;
        ShowTheRestOfGerms();
    }

    private void IniGrid()
    {
        Vector3 positionOffset = transform.position - new Vector3(gridDimension * distance / 2.0f, gridDimension * distance / 2.0f, 0);

        for(int x = 0; x < gridDimension; x++)
        {
            for(int y =0; y<gridDimension; y++)
            {
                GameObject newGerm = Instantiate(this.germPrefab);

                List<Sprite> possibleSprites = new List<Sprite>(sprites);

                Sprite left1 = GetSprite(x - 1, y);
                Sprite left2 = GetSprite(x - 2, y);
                if (left2 != null && left1 == left2)
                {
                    possibleSprites.Remove(left1);
                }

                Sprite down1 = GetSprite(x, y - 1);
                Sprite down2 = GetSprite(x, y - 2);
                if (down2 != null && down1 == down2)
                {
                    possibleSprites.Remove(down1);
                }

                SpriteRenderer sr = newGerm.GetComponent<SpriteRenderer>();
                sr.sprite = possibleSprites[Random.Range(0, possibleSprites.Count)];

                Germ germ = newGerm.AddComponent<Germ>();
                germ.Position = new Vector2Int(x, y);

                newGerm.transform.parent = transform;
                newGerm.transform.position = positionOffset + new Vector3(x * this.distance, y * this.distance, 0);

                Grid[x, y] = newGerm;


                //create grid background

                GameObject BlockBackGroud = Instantiate(this.blockPrefab);
                SpriteRenderer blockSR = BlockBackGroud.GetComponent<SpriteRenderer>();
                BlockBackGroud.transform.position = positionOffset + new Vector3(x * this.distance, y * this.distance, 0);
                if ((x + y) % 2 != 0)
                {
                    blockSR.color = Color.gray;
                }
            }
        }
    }

    private Sprite GetSprite(int x, int y)
    {
        if (x < 0 || x >= gridDimension
            || y < 0 || y >= gridDimension)
            return null;
        GameObject germ = Grid[x, y];
        SpriteRenderer sr = germ.GetComponent<SpriteRenderer>();
        return sr.sprite;
    }

    public void SwapGerms(Vector2Int germ1Position, Vector2Int germ2Position)
    {
        GameObject germ1 = Grid[germ1Position.x, germ1Position.y];
        SpriteRenderer renderer1 = germ1.GetComponent<SpriteRenderer>();

        GameObject germ2 = Grid[germ2Position.x, germ2Position.y];
        SpriteRenderer renderer2 = germ2.GetComponent<SpriteRenderer>();

        Sprite temp = renderer1.sprite;
        renderer1.sprite = renderer2.sprite;
        renderer2.sprite = temp;

        SoundManager.Instance.PlaySound(SoundType.TypeMove);

        do
        {
            FillHoles();
        } while (CheckMatches());

        ShowTheRestOfGerms();
        if (germAmount <= 0)
        {
            ui.GetComponent<UIManager>().OnPauseClick();
        }
    }

    private SpriteRenderer GetSpriteRenderer(int x, int y)
    {
        if (x < 0 || x >= gridDimension || y < 0 || y >= gridDimension)
        {
            return null;
        }

        GameObject germ = Grid[x, y];
        SpriteRenderer sr = germ.GetComponent<SpriteRenderer>();
        return sr;
    }

    private bool CheckMatches()
    {
        HashSet<SpriteRenderer> matchedGerm = new HashSet<SpriteRenderer>();

        for(int x=0; x<gridDimension; x++)
        {
            for(int y=0; y<gridDimension; y++)
            {

                SpriteRenderer currentSR = GetSpriteRenderer(x, y);

                if (currentSR.sprite == null)
                    continue;

                //Findcolumnmatches
                List<SpriteRenderer> horizontalMatches = FindColumnMatchForGerm(x, y, currentSR.sprite);
                if(horizontalMatches.Count >= 2)
                {
                    matchedGerm.UnionWith(horizontalMatches);
                    matchedGerm.Add(currentSR);
                }

                //FindRowMatches
                List<SpriteRenderer> verticalMatches = FindRowMatchForGerm(x, y, currentSR.sprite);
                if (verticalMatches.Count >= 2)
                {
                    matchedGerm.UnionWith(verticalMatches);
                    matchedGerm.Add(currentSR);
                }
            }
        }

        foreach (SpriteRenderer sr in matchedGerm)
        {
            sr.sprite = null;
        }
        Score += matchedGerm.Count;
        germAmount -= matchedGerm.Count;
        
        return matchedGerm.Count > 0;
    }

    private List<SpriteRenderer> FindColumnMatchForGerm(int x, int y, Sprite sprite)
    {
        List<SpriteRenderer> result = new List<SpriteRenderer>();
        for(int i = x+1; i<gridDimension; i++)
        {
            SpriteRenderer nextColumn = GetSpriteRenderer(i, y);
            if(nextColumn.sprite != sprite)
            {
                break;
            }
            result.Add(nextColumn);
        }
        return result;
    }

    private List<SpriteRenderer> FindRowMatchForGerm(int x, int y, Sprite sprite)
    {
        List<SpriteRenderer> result = new List<SpriteRenderer>();
        for (int i = y + 1; i < gridDimension; i++)
        {
            SpriteRenderer nextRow = GetSpriteRenderer(x, i);
            if (nextRow.sprite != sprite)
            {
                break;
            }
            result.Add(nextRow);
        }
        return result;
    }

    private void FillHoles()
    {
        for (int x = 0; x < gridDimension; x++)
        {
            for (int y =0; y<gridDimension; y++)
            {
                while(GetSpriteRenderer(x, y).sprite == null && CheckFill(x,y))
                {
                    SpriteRenderer current = GetSpriteRenderer(x, y);
                    SpriteRenderer next = current;
                    for (int row = y; row < gridDimension - 1; row++)
                    {
                        next = GetSpriteRenderer(x, row + 1);
                        current.sprite = next.sprite;
                        current = next;
                    }
                    next.sprite = null;
                }
                
            }
        }
    }

    private bool CheckFill(int x, int y)
    {
        bool result = false;
        for(int i = y+1; i < gridDimension; i++)
        {
            if (GetSpriteRenderer(x, i).sprite != null)
            {
                result = true;
                break;
            }
        }
        return result;
    }

    public void PauseGame()
    {
        SoundManager.Instance.PlaySound(SoundType.TypeGameOver);

        for(int x = 0; x < gridDimension; x++)
        {
            for(int y=0; y < gridDimension; y++)
            {
                Grid[x, y].GetComponent<Collider2D>().enabled = false;
            }
        }
    }

    private void ShowTheRestOfGerms()
    {
        Debug.Log("Germs rest amount: " + germAmount);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
