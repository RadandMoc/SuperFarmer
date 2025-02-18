using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region fields
    [SerializeField]
    GameObject firstViewButtons;
    [SerializeField]
    private GameObject selectPlayers;
    [SerializeField]
    List<TextMeshProUGUI> NNamesInpt;
    [SerializeField]
    List<UnityEngine.UI.Image> wantThisPlayerBtn;
    [SerializeField]
    GameObject gameWindow;
    [SerializeField]
    List<GameObject> playersInfoBtns;
    [SerializeField]
    List<TextMeshProUGUI> playersBtnsText;
    [SerializeField]
    TextMeshProUGUI actualPlayerInfoTxt;
    private byte actualActivePlayerInfo = 5; // 0 - bank, 1-4 - players, 5 - anyone
    [SerializeField]
    GameObject playerInfoBox;
    [SerializeField]
    TextMeshProUGUI playerInfoContent;
    [SerializeField]
    GameObject dicesResultBox;
    [SerializeField]
    TextMeshProUGUI dicesResultText;
    [SerializeField]
    GameObject tradeBtn;
    [SerializeField]
    TMP_Dropdown trader;
    [SerializeField]
    GameObject traderBox;
    [SerializeField]
    TextMeshProUGUI playerAnimals;
    [SerializeField]
    TextMeshProUGUI traderAnimals;
    [SerializeField]
    GameObject errorBox;
    [SerializeField]
    List<TextMeshProUGUI> finalTradeAnimals;
    [SerializeField]
    TextMeshProUGUI errorMessageText;
    [SerializeField]
    List<GameObject> tradeInputsObjects;
    [SerializeField]
    GameObject winnerBox;
    [SerializeField]
    TextMeshProUGUI valueAnimalsBeforeTrade;
    [SerializeField]
    TextMeshProUGUI valueAnimalsAfterTrade;
    [SerializeField]
    GameObject loadGameWindow;
    [SerializeField]
    GameObject saveDoesntExist;
    [SerializeField]
    GameObject saveMenu;
    #endregion fields

    // Start is called before the first frame update
    void Start()
    {
        //Screen.SetResolution(800,500,true); // teoretycznie ustawia full screena i minimalne rozmiary, ale nie ufam temu.
        selectPlayers.gameObject.SetActive(false);
        gameWindow.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// This func close app (game).
    /// </summary>
    public void Exit()
    {
        Application.Quit();
        //Debug.Log("Zamykam Apke");
    }

    /// <summary>
    /// Change window view. <i><b>Close</b></i> window which player see first. <i><b>Open</b></i> window with inouts to write players nicknames.
    /// </summary>
    public void SelectPlayers()
    {
        firstViewButtons.gameObject.SetActive(false);
        selectPlayers.gameObject.SetActive(true);
    }

    /// <summary>
    /// Change window view. <i><b>Close</b></i> window with nickname inputs. <i><b>Open</b></i> window which player see first.
    /// </summary>
    public void BackToMainMenu()
    {
        selectPlayers.gameObject.SetActive(false);
        firstViewButtons.gameObject.SetActive(true);
    }

    /// <summary>
    /// <i><b>Close</b></i> window to entry nicknames. <i><b>Open</b></i> window to play game. <br/>This function add players to game manager from window with inputs.
    /// </summary>
    public void StartGame()
    {
        selectPlayers.gameObject.SetActive(false); // Hide window to writting nicknames
        UnityEngine.Color green = new UnityEngine.Color(0, 1, 0.09206605f, 1);
        List<string> names = new List<string>();
        for (int i = 0; i < 4; i++) // Creating list with human players.
        {
            if (ColorsEqual(wantThisPlayerBtn[i].color, green))
            {
                TextMeshProUGUI cosiek = NNamesInpt[i];
                string assist = NNamesInpt[i].text;
                if (assist.Length == 1 || string.IsNullOrEmpty(assist) || string.IsNullOrWhiteSpace(assist))
                    names.Add($"Player {names.Count + 1}");
                else
                    names.Add(assist);
            }
        }
        GameManager.Instance.PlayerList.Add(new Player(names.Count, "Bank")); // Adding all players to game manager
        for (int i = 0; i < names.Count; i++)
        {
            GameManager.Instance.PlayerList.Add(new Player(0, names[i]));
        }
        //foreach(Player player in GameManager.Instance.PlayerList)
        //{
        //    Debug.Log(player.ToString());
        //}
        for (int i = 0; i < GameManager.Instance.PlayerList.Count; i++) // Turning on buttons to see information about players animals
        {
            playersInfoBtns[i].SetActive(true);
            playersBtnsText[i].text = GameManager.Instance.PlayerList[i].NickName;
        }
        actualPlayerInfoTxt.text = $"Actual player: {GameManager.Instance.PlayerList[GameManager.Instance.ActualPlayerId].NickName}"; // Inormation about actual player turn
        gameWindow.SetActive(true);
        List<string> allPlayersNames = new List<string>();
        foreach (Player player in GameManager.Instance.PlayerList) // Add names to trade menu (to choose trader by player)
        {
            allPlayersNames.Add(player.ToString());
        }
        trader.AddOptions(allPlayersNames);
    }

    /// <summary>
    /// Function set information about player in special objects. Later show trade menu.
    /// </summary>
    public void ShowTraderMenu()
    {
        string content = "Your animals:\n"; //I wanted two enters after header. That's not mistake        
        byte i = 0;
        int numOfAnimals;
        int valueInRabbits = 0;
        IAnimal animalObj;
        foreach (var item in tradeInputsObjects)
        {
            animalObj = GetAnimal(i);
            if (GameManager.Instance.PlayerList[GameManager.Instance.ActualPlayerId].NumOfAnimals.TryGetValue(animalObj, out numOfAnimals))
            {
                item.GetComponent<TMP_InputField>().text = $"{numOfAnimals}"; // Default values in input fields.
                valueInRabbits += numOfAnimals * animalObj.GetValueInRabbits(); // Value in rabbits before trade
                content += $"\n{animalObj.GetName()}: {numOfAnimals}"; // Information about player animals before trade
            }
            else
                item.GetComponent<TMP_InputField>().text = $"0"; // Default values in input fields.
            i++;
            //Debug.Log($"Treœæ komórki {i}: {item.GetComponent<TMP_InputField>().text}");
            //Debug.Log($"Iloœæ znaków komórki {i}: {item.GetComponent<TMP_InputField>().text.Count()}");
        }
        playerAnimals.text = content;
        ShowTraderAnimals();
        valueAnimalsBeforeTrade.text = $"/{valueInRabbits}";
        traderBox.SetActive(true);
    }

    /// <summary>
    /// This function updates the value of the inputs. It converts the animals the player wants into values in rabbits.
    /// </summary>
    public void UpdateActualValueInRabbits()
    {
        byte i = 0;
        int valueInRabbits = 0;
        int numOfAnimals;
        IAnimal animalObj;
        foreach (var item in finalTradeAnimals)
        {
            animalObj = GetAnimal(i);
            if(item.text.Count()>3)
            {
                valueAnimalsAfterTrade.text = "Error";
                return;
            }
            else if(item.text.Count() == 3)
            {
                if (char.IsDigit(item.text[0]) && char.IsDigit(item.text[1]))
                    numOfAnimals = ((int)item.text[0] - (int)'0') * 10 + (int)item.text[1] - (int)'0';
                else
                {
                    valueAnimalsAfterTrade.text = "Error";
                    return;
                }
            }
            else if (item.text.Count() == 2)
            {
                if (char.IsDigit(item.text[0]))
                    numOfAnimals = (int)item.text[0] - (int)'0';
                else
                {
                    valueAnimalsAfterTrade.text = "Error";
                    return;
                }
            }
            else
                numOfAnimals = 0;
            valueInRabbits += numOfAnimals * animalObj.GetValueInRabbits(); // Value in rabbits after trade
            i++;
        }
        valueAnimalsAfterTrade.text = $"{valueInRabbits}";
    }

    /// <summary>
    /// This func hide trade window
    /// </summary>
    public void CancelTrade()
    {
        traderBox.SetActive(false);
    }

    /// <summary>
    /// This func records the animals that the trader owns.
    /// </summary>
    public void ShowTraderAnimals()
    {
        int selectedTrader = trader.value;
        string content = "";
        bool isFirst = true;
        foreach (var item in GameManager.Instance.PlayerList[selectedTrader].NumOfAnimals)
        {
            if (!isFirst)
                content += "\n";
            else
                isFirst = false;
            content += $"{item.Key.GetName()}: {item.Value}";
        }
        traderAnimals.text = content;
    }

    /// <summary>
    /// Func change clicked buttons color.
    /// </summary>
    /// <param name="img">clicked button (image)</param>
    public void SetOnOffPlayer(UnityEngine.UI.Image img)
    {
        UnityEngine.Color red = new UnityEngine.Color(1, 0.06112972f, 0, 1);
        UnityEngine.Color green = new UnityEngine.Color(0, 1, 0.09206605f, 1);
        if (ColorsEqual(img.color, red))
            img.color = green;
        else
            img.color = red;
    }

    /// <summary>
    /// This function show info about players animals, or hide this information if actually its showed.
    /// </summary>
    /// <param name="playerId">player which information player want see</param>
    public void ShowHideInfoAboutPlayer(int playerId)
    {
        if (playerId == actualActivePlayerInfo)
            HideInfoAboutPlayer();
        else
        {
            string content = $"{GameManager.Instance.PlayerList[playerId].NickName} have:";
            foreach (var item in GameManager.Instance.PlayerList[playerId].NumOfAnimals)
            {
                content += "\n";
                content += $"{item.Key.GetName()}: {item.Value}";
            }
            playerInfoContent.text = content;
            actualActivePlayerInfo = (byte)playerId;
            playerInfoBox.SetActive(true);
        }
    }

    /// <summary>
    /// Function realize all trade for the UI. (Hide trade window and button to show trade window)
    /// </summary>
    void RealizeTradeUI()
    {
        tradeBtn.SetActive(false);
        traderBox.SetActive(false);
    }

    /// <summary>
    /// All logic (and UI tasks) to check and make transaction (trade) between players.
    /// </summary>
    public void TryTrade()
    {
        Dictionary<IAnimal, int> finalAnimals = new Dictionary<IAnimal, int>();
        byte i = 0;

        // Fill dictionary with values from UI. If it is not possible show error.
        foreach (var item in finalTradeAnimals)
        {
            if (item.text.Count() > 3)
            {
                ErrorMessage(ErrorType.LetterInValue);
                return;
            }
            else if (item.text.Count() > 1)
            {
                int val = 0;
                if (item.text.Count() == 3)
                {
                    if (char.IsDigit(item.text[0]) && char.IsDigit(item.text[1]))
                        val = ((int)item.text[0] - (int)'0') * 10 + (int)item.text[1] - (int)'0';
                    else
                    {
                        ErrorMessage(ErrorType.LetterInValue);
                        return;
                    }
                }
                else if (item.text.Count() == 2)
                {
                    if (char.IsDigit(item.text[0]))
                        val = (int)item.text[0] - (int)'0';
                    else
                    {
                        ErrorMessage(ErrorType.LetterInValue);
                        return;
                    }
                }
                if (val > 0)
                    finalAnimals.Add(GetAnimal(i), val);
            }
            i++;
        }

        if (!GameManager.Instance.PlayerList[GameManager.Instance.ActualPlayerId].IsTradeValueEqual(finalAnimals))
        {
            ErrorMessage(ErrorType.ValueNotEqual);
            return;
        }
        if(GameManager.Instance.ActualPlayerId == trader.value)
        {
            ErrorMessage(ErrorType.TheSamePlayer);
            return;
        }
        Dictionary<IAnimal, int> traderDics;
        if (!GameManager.Instance.PlayerList[trader.value].TryGetDicsAfterTrade(GameManager.Instance.PlayerList[GameManager.Instance.ActualPlayerId], finalAnimals, out traderDics)) //(!GameManager.Instance.PlayerList[trader.value].PlayerHaveEnoughAnimals(finalAnimals)) <= wrong =========== wrong =========== wrong =========
        {
            ErrorMessage(ErrorType.TraderDontHaveAnimal);
            return;
        }
        else // Trade is possible
        {
            if (trader.value == 0 && traderDics.Count() != 7) // Bank always have every type of animals in own dictionary. (For easy get values)
            {
                i = 0;
                int bin;
                IAnimal animal;
                while (i < 7 && traderDics.Count() != 7)
                {
                    animal = GetAnimal(i);
                    if (!traderDics.TryGetValue(animal, out bin))
                        traderDics.Add(animal, 0);
                    i++;
                }
            }
            GameManager.Instance.PlayerList[trader.value].NumOfAnimals = traderDics;
            GameManager.Instance.PlayerList[GameManager.Instance.ActualPlayerId].NumOfAnimals = finalAnimals;
            RealizeTradeUI();
        }

        // Checking whether a player has won
        if (GameManager.Instance.PlayerList[GameManager.Instance.ActualPlayerId].IsWin())
            winnerBox.SetActive(true);
    }

    /// <summary>
    /// Show error box. Fun for UI when player try trade.
    /// </summary>
    /// <param name="why">enum with type why trade is not possible</param>
    void ErrorMessage(ErrorType why)
    {
        switch (why)
        {
            case ErrorType.LetterInValue:
                errorMessageText.text = "The values entered are incorrect.\nCheck that you have not entered a letter and that all windows have up to 2 characters.";
                errorBox.SetActive(true);
                break;
            case ErrorType.TraderDontHaveAnimal:
                errorMessageText.text = "The values entered are incorrect.\nCheck that the person you are trying to trade with has enough animals.";
                errorBox.SetActive(true);
                break;
            case ErrorType.ValueNotEqual:
                errorMessageText.text = "The values entered are incorrect.\nCheck that the value of your animals is the same before and after the trade.";
                errorBox.SetActive(true);
                break;
            case ErrorType.TheSamePlayer:
                errorMessageText.text = "The values entered are incorrect.\nYou cannot trade with yourself.";
                errorBox.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// Fun for hide error box
    /// </summary>
    public void HideErrorBox()
    {
        errorBox.SetActive(false);
    }

    /// <summary>
    /// Get animal by id.
    /// </summary>
    /// <param name="id">from 0 up to 6</param>
    /// <returns></returns>
    IAnimal GetAnimal(byte id)
    {
        if (id == 0)
            return new Rabbit();
        else if (id == 1)
            return new Sheep();
        else if (id == 2)
            return new Pig();
        else if (id == 3)
            return new Cow();
        else if (id == 4)
            return new Horse();
        else if (id == 5)
            return new SmallDog();
        else if (id == 6)
            return new BigDog();
        else
            return null;
    }

    /// <summary>
    /// Func to hide box with information about player
    /// </summary>
    public void HideInfoAboutPlayer()
    {
        playerInfoBox.SetActive(false);
        actualActivePlayerInfo = 5;
    }

    /// <summary>
    /// Function for all UI to restart UI before game and set window to write nicknames.
    /// </summary>
    public void PlayAgain()
    {
        tradeBtn.SetActive(true);
        gameWindow.SetActive(false);
        foreach (var item in playersInfoBtns)
        {
            item.gameObject.SetActive(false);
        }
        winnerBox.SetActive(false);
        trader.ClearOptions();
        selectPlayers.SetActive(true);
    }

    /// <summary>
    /// UI logic for throwing dices
    /// </summary>
    public void ThrowDices()
    {
        if (GameManager.Instance.PlayerList[GameManager.Instance.ActualPlayerId].IsWin())
        { // window for winner
            winnerBox.SetActive(true);
        }
        else
        { // Show information what was changed.
            dicesResultText.text = GameManager.Instance.WhatChangedRollDices;
            dicesResultBox.SetActive(true);
        }
    }

    /// <summary>
    /// Func change content of label with information who play. Show button for trading and hide result of dices.
    /// </summary>
    public void NextTurnForUI()
    {
        dicesResultBox.SetActive(false);
        actualPlayerInfoTxt.text = $"Actual player: {GameManager.Instance.PlayerList[GameManager.Instance.ActualPlayerId].NickName}";
        //Debug.Log(actualPlayerInfoTxt.text[1]);
        tradeBtn.SetActive(true);
    }

    /// <summary>
    /// Fun compares two colors and checks that they are equal to each other to 3 decimal places.
    /// </summary>
    /// <param name="color1">first color</param>
    /// <param name="color2">second color</param>
    /// <returns><b>true</b> - if colors are equal<br/><b>false</b> - if colors are different</returns>
    bool ColorsEqual(UnityEngine.Color color1, UnityEngine.Color color2)
    {
        return Mathf.Round(color1.r * 1000) == Mathf.Round(color2.r * 1000) && Mathf.Round(color1.g * 1000) == Mathf.Round(color2.g * 1000) && Mathf.Round(color1.b * 1000) == Mathf.Round(color2.b * 1000);
    }

    /// <summary>
    /// Save actual game
    /// </summary>
    /// <param name="saveId">index of save</param>
    public void SaveGame(int saveId)
    {
        GameManager.Instance.SaveActualGame(!tradeBtn.activeSelf, $"Save{saveId}");
        HideOptionsToSave();
    }

    /// <summary>
    /// Hide menu to save game
    /// </summary>
    public void HideOptionsToSave()
    {
        saveMenu.SetActive(false);
    }

    /// <summary>
    /// Show menu to save game
    /// </summary>
    public void ShowOptionsToSave()
    {
        saveMenu.SetActive(true);
    }

    /// <summary>
    /// Show menu with saves to load.
    /// </summary>
    public void ShowSavedGames()
    {
        firstViewButtons.SetActive(false);
        loadGameWindow.SetActive(true);
    }

    /// <summary>
    /// Back from menu with saves to main menu
    /// </summary>
    public void BackFromLoadScreen()
    {
        firstViewButtons.SetActive(true);
        loadGameWindow.SetActive(false);
    }

    /// <summary>
    /// Hide communicate about "save doesn't exist"
    /// </summary>
    public void HideCommunicate()
    {
        saveDoesntExist.SetActive(false);
    }

    /// <summary>
    /// Load and start game. If it is not possible, show communicate
    /// </summary>
    /// <param name="saveId"></param>
    public void LoadGame(int saveId)
    {
        if(GameManager.Instance.LoadGame($"Save{saveId}"))
        {
            for (int i = 0; i < GameManager.Instance.PlayerList.Count; i++) // Turning on buttons to see information about players animals
            {
                playersInfoBtns[i].SetActive(true);
                playersBtnsText[i].text = GameManager.Instance.PlayerList[i].NickName;
            }
            actualPlayerInfoTxt.text = $"Actual player: {GameManager.Instance.PlayerList[GameManager.Instance.ActualPlayerId].NickName}"; // Inormation about actual player turn
            tradeBtn.SetActive(!GameManager.Instance.SaveGame.WasTrade);
            gameWindow.SetActive(true);
            loadGameWindow.SetActive(false);
            List<string> allPlayersNames = new List<string>();
            foreach (Player player in GameManager.Instance.PlayerList) // Add names to trade menu (to choose trader by player)
            {
                allPlayersNames.Add(player.ToString());
            }
            trader.AddOptions(allPlayersNames);
        }
        else
        {
            saveDoesntExist.SetActive(true);
        }
    }
}

enum ErrorType
{
    LetterInValue,
    ValueNotEqual,
    TraderDontHaveAnimal,
    TheSamePlayer
}