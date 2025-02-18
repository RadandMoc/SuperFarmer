using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class Player 
{
    Dictionary<IAnimal, int> numOfAnimals;  //IAnimal should be an enum. Yes. I know. But I wanted to make something strange but more advanced.
    string nickName;

    public Dictionary<IAnimal, int> NumOfAnimals { get => numOfAnimals; set => numOfAnimals = value; }
    public string NickName { get => nickName; set => nickName = value; }

    private Player() 
    {
        NumOfAnimals = new Dictionary<IAnimal, int>();
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="numOfPlayers">0 if declaring player is human, number [2;4] if it's bank player (number mean how many players are in the game</param>
    /// <param name="Nick">player nickname</param>
    public Player(int numOfPlayers, string Nick):this()
    {
        if(numOfPlayers == 0)
            NumOfAnimals.Add(new Rabbit(), 1);
        else
        {
            NumOfAnimals.Add(new Rabbit(), 60-numOfPlayers);
            NumOfAnimals.Add(new Sheep(), 24);
            NumOfAnimals.Add(new Pig(), 20);
            NumOfAnimals.Add(new Cow(), 12);
            NumOfAnimals.Add(new Horse(), 6);
            NumOfAnimals.Add(new SmallDog(), 4);
            NumOfAnimals.Add(new BigDog(), 2);
        }
        NickName = Nick;
    }

    public override string ToString() => $"{NickName}";

    /// <summary>
    /// Check The function checks whether the player has won.
    /// </summary>
    /// <returns><b>true</b> - if the player has met the conditions<br/><b>false</b> - if player need something to met the conditions</returns>
    public bool IsWin()
    {
        int havingAnimals = 0;
        foreach(var item in NumOfAnimals)
        {
            if(item.Key.GetType() == typeof(Horse) || item.Key.GetType() == typeof(Cow) || item.Key.GetType() == typeof(Pig) || item.Key.GetType() == typeof(Sheep) || item.Key.GetType() == typeof(Rabbit))
                havingAnimals++;
        }
        if(havingAnimals==5)
            return true;
        else 
            return false;
    }

    /// <summary>
    /// The function checks whether the value of the player's animals before the exchange is equal to that after the exchange.
    /// </summary>
    /// <param name="finalAnimals">A dictionary with the animals the player wants to have after the exchange.</param>
    /// <returns><b>true</b> - if values are equal<br/><b>false</b> - if values aren't equal</returns>
    public bool IsTradeValueEqual(Dictionary<IAnimal, int> finalAnimals)
    {
        int Value1 = 0;
        foreach(var item in NumOfAnimals)
        {
            Value1 += item.Value * item.Key.GetValueInRabbits();
        }
        int Value2 = 0;
        foreach (var item in finalAnimals)
        {
            Value2 += item.Value * item.Key.GetValueInRabbits();
        }
        return Value1 == Value2;
    }

    /// <summary>
    /// The function combines the two dictionaries and returns one that has the grouped animal values.
    /// </summary>
    /// <param name="secondPlayer">Second player wich dictionary you want add</param>
    /// <returns>United dictionary</returns>
    public Dictionary<IAnimal, int> UnionPlayersAnimals(Player secondPlayer)
    {
        Dictionary<IAnimal, int> returner = new Dictionary<IAnimal, int>();
        int noOfAnimals = 0;
        foreach (var item in NumOfAnimals)
        {
            if (secondPlayer.NumOfAnimals.TryGetValue(item.Key, out noOfAnimals))
                noOfAnimals += item.Value;
            else
                noOfAnimals = item.Value;
            returner.Add(item.Key, noOfAnimals);
            noOfAnimals = 0;
        }
        foreach (var item in secondPlayer.NumOfAnimals)
        {
            if (!returner.ContainsKey(item.Key))
                returner.Add(item.Key, item.Value);
        }
        return returner;
    }

    /// <summary>
    /// Function try return dictionary which is result of transaction. If you will use this function, you will have 2 dictionaries after transaction: <b>wantedAnimalsDicForSecondPlayer</b> and <b>thisPlayerDic</b>.<br/>If this trade is not possible, function return false.
    /// </summary>
    /// <param name="secondPlayer">The other player with whom the transaction is taking place.</param>
    /// <param name="wantedAnimalsDicForSecondPlayer">A tidy dictionary for the second (other) player.</param>
    /// <param name="thisPlayerDic">The resulting dictionary of exchanges between players.</param>
    /// <returns><b>true</b> - if trade is possible<br/><b>false</b> - if players don't have enough animals to make deal</returns>
    public bool TryGetDicsAfterTrade(Player secondPlayer, Dictionary<IAnimal, int> wantedAnimalsDicForSecondPlayer, out Dictionary<IAnimal, int> thisPlayerDic)
    {
        Dictionary<IAnimal, int> unifiedDic = this.UnionPlayersAnimals(secondPlayer);
        thisPlayerDic = new Dictionary<IAnimal, int>();
        foreach (var item in wantedAnimalsDicForSecondPlayer)
        {
            if (!unifiedDic.ContainsKey(item.Key))
                return false;
            if(item.Value > unifiedDic[item.Key])
                return false;
            else if(item.Value < unifiedDic[item.Key])
            {
                thisPlayerDic.Add(item.Key, unifiedDic[item.Key] - item.Value);
            }
        }
        foreach(var item in unifiedDic.Where(p=>!wantedAnimalsDicForSecondPlayer.ContainsKey(p.Key)))
        {
            thisPlayerDic.Add(item.Key,item.Value);
        }
        return true;
    }
}
