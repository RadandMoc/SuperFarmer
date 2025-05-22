using Assets.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	private static GameManager instance;
	List<Player> playerList;
	private byte actualPlayerId = 1;
	Dice[] dices = new Dice[2];
	string whatChangedRollDices;
	Save saveGame;

	public static GameManager Instance
	{
		get
		{
			if (instance == null)
				instance = new GameManager();
			return instance;
		}
	}

	private GameManager()
	{
		PlayerList = new List<Player>();
		dices[0] = new Dice();
		dices[1] = new Dice();
		Rabbit r = new Rabbit();
		Sheep s = new Sheep();
		Pig p = new Pig();

		for (int i = 0; i < 6; i++)
		{
			dices[0].AnimalsInDice.Add(r);
			dices[1].AnimalsInDice.Add(r);
		}
		for (int i = 0; i < 3; i++)
			dices[0].AnimalsInDice.Add(s);
		for (int i = 0; i < 2; i++)
			dices[1].AnimalsInDice.Add(s);

		dices[0].AnimalsInDice.Add(p);
		for (int i = 0; i < 2; i++)
			dices[1].AnimalsInDice.Add(p);

		dices[0].AnimalsInDice.Add(new Cow());
		dices[1].AnimalsInDice.Add(new Horse());
		dices[0].AnimalsInDice.Add(new Wolf());
		dices[1].AnimalsInDice.Add(new Fox());

		WhatChangedRollDices = "";
	}

	public List<Player> PlayerList { get => playerList; private set => playerList = value; }
	public byte ActualPlayerId { get => actualPlayerId; private set => actualPlayerId = value; }
	public string WhatChangedRollDices { get => whatChangedRollDices; set => whatChangedRollDices = value; }
	public Save SaveGame { get => saveGame; set => saveGame = value; }

	// Start is called before the first frame update
	void Start()
	{
		if (instance != null && instance != this)
			Destroy(this.gameObject);
		else
			instance = this;
	}

	// Update is called once per frame
	void Update()
	{

	}

	/// <summary>
	/// Set next player as actual player
	/// </summary>
	public void NextPlayer()
	{
		for (int i = 0; i < PlayerList.Count; i++)
		{
			ActualPlayerId++;
			ActualPlayerId %= (byte)PlayerList.Count;
			if (PlayerList[ActualPlayerId].TurnOfWin == null)
				return;
		}
	}

	/// <summary>
	/// Function restart game manager to status before the game.
	/// </summary>
	public void PlayAgain()
	{
		actualPlayerId = 1;
		playerList.Clear();
	}

	/// <summary>
	/// All backend logic what's happend when player throw dices.
	/// </summary>
	public void ThrowDices()
	{
		IAnimal firstResult = dices[0].RollDice();
		IAnimal secondResult = dices[1].RollDice();
		int numOfAnimals = 0;
		WhatChangedRollDices = $"Dice 1: {firstResult.GetName()}, Dice 2: {secondResult.GetName()}";
		if (firstResult == secondResult)
		{
			if (PlayerList[ActualPlayerId].NumOfAnimals.TryGetValue(firstResult, out numOfAnimals))
			{
				int addidtionalAnimals = Math.Min((int)((numOfAnimals + 2) / 2), GameManager.Instance.playerList[0].NumOfAnimals[firstResult]);
				PlayerList[ActualPlayerId].NumOfAnimals[firstResult] = numOfAnimals + addidtionalAnimals;
				WhatChangedRollDices += $"\nAdded {addidtionalAnimals} {firstResult.GetName()}";
				if (addidtionalAnimals > 1)
					WhatChangedRollDices += "s";
				GameManager.Instance.playerList[0].NumOfAnimals[firstResult] -= addidtionalAnimals;
			}
			else
			{
				if (GameManager.Instance.playerList[0].NumOfAnimals[firstResult] > 0)
				{
					PlayerList[ActualPlayerId].NumOfAnimals.Add(firstResult, 1);
					WhatChangedRollDices += $"\nAdded 1 {firstResult.GetName()}";
					GameManager.Instance.playerList[0].NumOfAnimals[firstResult] -= 1;
				}
				else
					WhatChangedRollDices += $"\nBank don't have this type of animal";
			}
		}
		else
		{
			if (firstResult is Wolf || secondResult is Fox)
			{
				if (firstResult is Wolf && secondResult is Fox)
				{
					WolfEat();
					FoxEat();
				}
				else if (firstResult is Wolf)
				{
					if (WolfEat())
					{
						if (secondResult is Rabbit || secondResult is Horse)
							WhatChangedRollDices += SingleReproduce(ActualPlayerId, secondResult);
					}
					else
						WhatChangedRollDices += SingleReproduce(ActualPlayerId, secondResult);
				}
				else //if (secondResult is Fox)
				{
					if (FoxEat())
					{
						if (firstResult is not Rabbit)
							WhatChangedRollDices += SingleReproduce(ActualPlayerId, firstResult);
					}
					else
						WhatChangedRollDices += SingleReproduce(ActualPlayerId, firstResult);
				}
			}
			else
			{
				string fisrtRes, secondRes;
				fisrtRes = SingleReproduce(ActualPlayerId, firstResult);
				secondRes = SingleReproduce(ActualPlayerId, secondResult);
				WhatChangedRollDices += fisrtRes + secondRes;
				if (fisrtRes.Length + secondRes.Length == 0)
					WhatChangedRollDices += "\nNothing has changed";
			}
		}
	}

	/// <summary>
	/// Function increase number of animals in player farm. <br/>Scheme of increase is in instrucion of game. <br/>It decrease number of animals from Bank.
	/// </summary>
	/// <param name="PlayerId"></param>
	/// <param name="diceResult"></param>
	/// <returns>text with information what has changed with \n in the beginning of the text.</returns>
	private string SingleReproduce(int PlayerId, IAnimal diceResult)
	{
		string result = "";
		if (PlayerList[ActualPlayerId].NumOfAnimals.TryGetValue(diceResult, out int numOfAnimals))
		{
			int addidtionalAnimals = Math.Min((int)((numOfAnimals + 1) / 2), GameManager.Instance.playerList[0].NumOfAnimals[diceResult]);
			PlayerList[ActualPlayerId].NumOfAnimals[diceResult] = numOfAnimals + addidtionalAnimals;
			result += $"\nAdded {addidtionalAnimals} {diceResult.GetName()}";
			if (addidtionalAnimals > 1)
				WhatChangedRollDices += "s";
			GameManager.Instance.playerList[0].NumOfAnimals[diceResult] -= addidtionalAnimals;
		}
		return result;
	}

	/// <summary>
	/// Function checking how many small dogs player had. <br/>If he hadn't any small dog, function delete players animals and add to the bank. <br/>If player had small dog, fun delete only one small dog and add in bank.
	/// </summary>
	/// <returns>true if player hadn't small dog<br/>false if player had small dog</returns>
	private bool FoxEat()
	{
		int numOfDogs = 0;
		if (!PlayerList[actualPlayerId].NumOfAnimals.TryGetValue(new SmallDog(), out numOfDogs)) // player don't have any small dog
		{
			int numOfAnimals = 0;
			if (PlayerList[actualPlayerId].NumOfAnimals.TryGetValue(new Rabbit(), out numOfAnimals) && numOfAnimals > 1)
			{
				PlayerList[actualPlayerId].NumOfAnimals[new Rabbit()] = 1;
				WhatChangedRollDices += $"\nYou lost {numOfAnimals - 1} rabbit";
				if (numOfAnimals > 3)
					WhatChangedRollDices += "s";
				PlayerList[0].NumOfAnimals[new Rabbit()] += numOfAnimals - 1;
			}
			else
				WhatChangedRollDices += $"\nThe fox did nothing to the player's farm.";
			return true;
		}
		else // disapear small dog
		{
			if (numOfDogs > 1)
				PlayerList[actualPlayerId].NumOfAnimals[new SmallDog()] = numOfDogs - 1;
			else
				PlayerList[actualPlayerId].NumOfAnimals.Remove(new SmallDog());
			PlayerList[0].NumOfAnimals[new SmallDog()] += 1;
			WhatChangedRollDices += "\nYou lost your small dog.";
			return false;
		}
	}

	/// <summary>
	/// Function checking how many big dogs player had. <br/>If he hadn't any big dog, function delete players animals and add to the bank. <br/>If player had big dog, fun delete only one big dog and add in bank.
	/// </summary>
	/// <returns>true if player hadn't big dog.<br/>false if player had big dog</returns>
	private bool WolfEat()
	{
		int numOfDogs = 0;
		if (!PlayerList[actualPlayerId].NumOfAnimals.TryGetValue(new BigDog(), out numOfDogs)) // player don't have any big dog
		{
			int numOfAnimals = 0;
			bool somethingChanged = false;
			if (PlayerList[actualPlayerId].NumOfAnimals.TryGetValue(new Sheep(), out numOfAnimals))
			{
				PlayerList[actualPlayerId].NumOfAnimals.Remove(new Sheep());
				WhatChangedRollDices += $"\nYou lost {numOfAnimals} sheep";
				if (numOfAnimals > 1)
					WhatChangedRollDices += "s";
				PlayerList[0].NumOfAnimals[new Sheep()] += numOfAnimals;
				numOfAnimals = 0;
				somethingChanged = true;
			}
			if (PlayerList[actualPlayerId].NumOfAnimals.TryGetValue(new Cow(), out numOfAnimals))
			{
				PlayerList[actualPlayerId].NumOfAnimals.Remove(new Cow());
				WhatChangedRollDices += $"\nYou lost {numOfAnimals} cow";
				if (numOfAnimals > 1)
					WhatChangedRollDices += "s";
				PlayerList[0].NumOfAnimals[new Cow()] += numOfAnimals;
				numOfAnimals = 0;
				somethingChanged = true;
			}
			if (PlayerList[actualPlayerId].NumOfAnimals.TryGetValue(new Pig(), out numOfAnimals))
			{
				PlayerList[actualPlayerId].NumOfAnimals.Remove(new Pig());
				WhatChangedRollDices += $"\nYou lost {numOfAnimals} pig";
				if (numOfAnimals > 1)
					WhatChangedRollDices += "s";
				PlayerList[0].NumOfAnimals[new Pig()] += numOfAnimals;
				somethingChanged = true;
			}
			if (!somethingChanged)
				WhatChangedRollDices += $"\nThe wolf did nothing to the player's farm.";
			return true;
		}
		else // disapear big dog
		{
			if (numOfDogs > 1)
				PlayerList[actualPlayerId].NumOfAnimals[new BigDog()] = numOfDogs - 1;
			else
				PlayerList[actualPlayerId].NumOfAnimals.Remove(new BigDog());
			PlayerList[0].NumOfAnimals[new BigDog()] += 1;
			WhatChangedRollDices += "\nYou lost your big dog.";
			return false;
		}
	}

	/// <summary>
	/// Save actual game status to binary file.
	/// </summary>
	/// <param name="wasTrade">information, actual player traded in this turn?</param>
	/// <param name="name">file name how save will have the name</param>
	public void SaveActualGame(bool wasTrade, string name)
	{
		SaveGame = new Save(PlayerList, ActualPlayerId, wasTrade);
		SaveGame.SaveBinary(name);
	}

	/// <summary>
	/// Logic load game
	/// </summary>
	/// <param name="name">file name</param>
	/// <returns><b>true</b> - if load was succesfully<br/><b>false</b> - if was error when app tried read file</returns>
	public bool LoadGame(string name)
	{
		Save zapis = Save.ReadBinary(name);
		if (zapis == null)
			return false;
		SaveGame = zapis;
		UnzipSave();
		return true;
	}

	/// <summary>
	/// Change save to game manager instance
	/// </summary>
	void UnzipSave()
	{
		foreach (var item in SaveGame.PlayerList)
		{
			playerList.Add(item);
			Player.PlayersWon = 0;
			if (item.TurnOfWin != null)
				Player.PlayersWon++;
		}
		ActualPlayerId = SaveGame.ActualPlayerId;
	}
}
