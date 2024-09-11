using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;

public class Board : MonoBehaviour
{
    private static readonly KeyCode[] SUPPORTED_KEYS = new KeyCode[]
    {
        KeyCode.A,KeyCode.B,KeyCode.C,KeyCode.D,KeyCode.E,KeyCode.F,
        KeyCode.G,KeyCode.H,KeyCode.I,KeyCode.J,KeyCode.K,KeyCode.L,
        KeyCode.M,KeyCode.N,KeyCode.O,KeyCode.P,KeyCode.Q,KeyCode.R,
        KeyCode.S,KeyCode.T,KeyCode.U,KeyCode.V,KeyCode.W,KeyCode.X,  
        KeyCode.Y,KeyCode.Z
    };

    private Row[] rows;

    private string[] solutions;
    private string[] validWords;
    private string word;

    private int rowIndex;
    private int columnIndex;

    [Header("States")]
    public Tile.State emptyState;
    public Tile.State occupiedState;
    public Tile.State correctState;
    public Tile.State wrongSpotState;
    public Tile.State incorrectState;

    [Header("UI")]
    public GameObject invalidWordText;



    private void Awake()
    {
        rows = GetComponentsInChildren<Row>();  
    }

    private void Start()
    {
        LoadData();
        SetRandomWord();

    }

    private void LoadData()
    {
        TextAsset textFile = Resources.Load("official_wordle_all") as TextAsset;
        validWords = textFile.text.Split('\n');

        textFile = Resources.Load("official_wordle_common") as TextAsset;
        solutions = textFile.text.Split('\n');

    }
       
    private void SetRandomWord()
    {
        word = solutions[Random.Range(0, solutions.Length)];
        word = word.ToLower().Trim();
        word = "games";
    }
    private void Update()
    {
        Row currentRow = rows[rowIndex];

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            columnIndex = Mathf.Max(columnIndex -1,0);
            

            currentRow.tiles[columnIndex].setLetter('\0');
            currentRow.tiles[columnIndex].setState(emptyState);

            invalidWordText.gameObject.SetActive(false);
        }

        else if (columnIndex >= rows[rowIndex].tiles.Length)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SubmitRow(currentRow);
            }
        }
        else
        {


            for (int i = 0; i < SUPPORTED_KEYS.Length; i++)
            {
                if (Input.GetKeyDown(SUPPORTED_KEYS[i]))
                {
                    currentRow.tiles[columnIndex].setLetter((char)SUPPORTED_KEYS[i]);
                    currentRow.tiles[columnIndex].setState(occupiedState);
                    columnIndex++;
                    break;
                }
            }

        }
    }

    private void SubmitRow(Row row)
    {
        if (!IsValidWord(row.word))
        {
            invalidWordText.gameObject.SetActive(true);
            return;
        }
        string reaminig = word;

        for (int i = 0; i < row.tiles.Length; i++)
        {
            Tile tile = row.tiles[i];

            if (tile.letter == word[i])
            {
                tile.setState(correctState);
                reaminig = reaminig.Remove(i, 1);
                reaminig = reaminig.Insert(i," ");
                
            }
            else if (!word.Contains(tile.letter))
            {
                tile.setState(incorrectState);
              
            }
        }

        for (int i = 0; i < row.tiles.Length; i++)
        {
            Tile tile = row.tiles[i];

            if(tile.state != correctState && tile.state != incorrectState)
            {
                if (reaminig.Contains(tile.letter))
                {
                    tile.setState(wrongSpotState);

                    int index = reaminig.IndexOf(tile.letter);
                    reaminig = reaminig.Remove(i, 1);
                    reaminig = reaminig.Insert(i, " ");
                }
                else
                {
                    tile.setState(incorrectState);
                }
            }


        }

        rowIndex++;
        columnIndex = 0;

        if(rowIndex >= rows.Length)
        {
            enabled = false; 

        }
    }
    private bool IsValidWord(string word)
    {
        for (int i = 0; i < validWords.Length; i++)
        {
            if (validWords[i] == word)
            {
                return true;
            }
        }
        return false;
    }
}
