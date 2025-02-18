using System.Collections.Generic;

public class Dice
{
    List<IAnimal> animalsInDice;

    public List<IAnimal> AnimalsInDice { get => animalsInDice; set => animalsInDice = value; }

    public Dice()
    {
        AnimalsInDice = new List<IAnimal>();
    }

    /// <summary>
    /// I draw, then return one animal from this dice.
    /// </summary>
    /// <returns>random animal from this dice</returns>
    public IAnimal RollDice()
    {
        System.Random rd = new System.Random();
        return AnimalsInDice[rd.Next(AnimalsInDice.Count)];
    }
}
