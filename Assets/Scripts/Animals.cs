using System;

[Serializable]
public class Rabbit : IAnimal
{
    public static string name = "Rabbit";
    
    public static int valueInRabbits = 1;

    public int GetValueInRabbits()
    {
        return valueInRabbits;
    }
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        Rabbit otherAnimal = (Rabbit)obj;

        return true;
    }

    public override int GetHashCode() => 1;

    public string GetName() => name;
}

[Serializable]
public class Sheep : IAnimal
{
    public static string name = "Sheep";

    public static int valueInRabbits = 6;

    public int GetValueInRabbits()
    {
        return valueInRabbits;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        Sheep otherAnimal = (Sheep)obj;

        return true;
    }

    public override int GetHashCode() => 2;

    public string GetName() => name;
}

[Serializable]
public class Pig : IAnimal
{
    public static string name = "Pig";

    public static int valueInRabbits = 12;

    public int GetValueInRabbits()
    {
        return valueInRabbits;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        Pig otherAnimal = (Pig)obj;

        return true;
    }

    public override int GetHashCode() => 3;

    public string GetName() => name;
}

[Serializable]
public class Cow : IAnimal
{
    public static string name = "Cow";

    public static int valueInRabbits = 36;

    public int GetValueInRabbits()
    {
        return valueInRabbits;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        Cow otherAnimal = (Cow)obj;

        return true;
    }

    public override int GetHashCode() => 4;

    public string GetName() => name;
}

[Serializable]
public class Horse : IAnimal
{
    public static string name = "Horse";

    public static int valueInRabbits = 72;

    public int GetValueInRabbits()
    {
        return valueInRabbits;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        Horse otherAnimal = (Horse)obj;

        return true;
    }

    public override int GetHashCode() => 5;

    public string GetName() => name;
}

[Serializable]
public class SmallDog : IAnimal
{
    public static string name = "Small Dog";

    public static int valueInRabbits = 6;

    public int GetValueInRabbits()
    {
        return valueInRabbits;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        SmallDog otherAnimal = (SmallDog)obj;

        return true;
    }

    public override int GetHashCode() => 6;

    public string GetName() => name;
}

[Serializable]
public class BigDog : IAnimal
{
    public static string name = "Big Dog";

    public static int valueInRabbits = 36;

    public int GetValueInRabbits()
    {
        return valueInRabbits;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        BigDog otherAnimal = (BigDog)obj;

        return true;
    }

    public override int GetHashCode() => 7;

    public string GetName() => name;
}

public class Wolf : IAnimal
{
    public static string name = "Wolf";

    public static int valueInRabbits = 0;

    public int GetValueInRabbits()
    {
        return valueInRabbits;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        Wolf otherAnimal = (Wolf)obj;

        return true;
    }

    public override int GetHashCode() => 8;

    public string GetName() => name;
}

public class Fox : IAnimal
{
    public static string name = "Fox";

    public static int valueInRabbits = 0;

    public int GetValueInRabbits()
    {
        return valueInRabbits;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        Fox otherAnimal = (Fox)obj;

        return true;
    }

    public override int GetHashCode() => 9;

    public string GetName() => name;
}