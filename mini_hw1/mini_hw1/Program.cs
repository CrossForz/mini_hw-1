using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

// Интерфейсы
interface IAlive
{
    int Food { get; }
}

interface IInventory
{
    int Number { get; }
}

// Базовые классы
abstract class Animal : IAlive, IInventory
{
    public string Name { get; set; }
    public int Food { get; set; }
    public int Number { get; set; }

    protected Animal(string name, int food, int number)
    {
        Name = name;
        Food = food;
        Number = number;
    }
}

abstract class Herbo : Animal
{
    public int Kindness { get; set; }
    protected Herbo(string name, int food, int number, int kindness) : base(name, food, number)
    {
        Kindness = kindness;
    }
}

abstract class Predator : Animal
{
    protected Predator(string name, int food, int number) : base(name, food, number) { }
}

class Monkey : Herbo
{
    public Monkey(int number, int kindness) : base("Monkey", 5, number, kindness) { }
}

class Rabbit : Herbo
{
    public Rabbit(int number, int kindness) : base("Rabbit", 3, number, kindness) { }
}

class Tiger : Predator
{
    public Tiger(int number) : base("Tiger", 10, number) { }
}

class Wolf : Predator
{
    public Wolf(int number) : base("Wolf", 7, number) { }
}

class Thing : IInventory
{
    public int Number { get; }
    public string Name { get; }
    public Thing(int number, string name)
    {
        Number = number;
        Name = name;
    }
}

class Zoo
{
    private readonly List<Animal> _animals = new();
    private readonly List<IInventory> _inventory = new();
    private readonly IVeterinaryClinic _clinic;

    public Zoo(IVeterinaryClinic clinic)
    {
        _clinic = clinic;
    }

    public void AddAnimal(Animal animal)
    {
        if (_clinic.CheckHealth(animal))
        {
            _animals.Add(animal);
            _inventory.Add(animal);
            Console.WriteLine($"{animal.Name} принят в зоопарк.");
        }
        else
        {
            Console.WriteLine($"{animal.Name} не принят в зоопарк из-за проблем со здоровьем.");
        }
    }

    public void AddThing(Thing thing)
    {
        _inventory.Add(thing);
    }

    public void ReportFood()
    {
        Console.WriteLine($"Всего еды требуется: {_animals.Sum(a => a.Food)} кг в день.");
    }

    public void ContactZooAnimals()
    {
        var friendlyAnimals = _animals.OfType<Herbo>().Where(a => a.Kindness > 5);
        Console.WriteLine("Животные для контактного зоопарка:");
        foreach (var animal in friendlyAnimals)
        {
            Console.WriteLine(animal.Name);
        }
    }
}

interface IVeterinaryClinic
{
    bool CheckHealth(Animal animal);
}

class VeterinaryClinic : IVeterinaryClinic
{
    public bool CheckHealth(Animal animal) => new Random().Next(2) == 1;
}

class Program
{
    static void Main()
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton<IVeterinaryClinic, VeterinaryClinic>()
            .AddSingleton<Zoo>()
            .BuildServiceProvider();

        var zoo = serviceProvider.GetService<Zoo>();
        zoo.AddAnimal(new Monkey(1, 6));
        zoo.AddAnimal(new Tiger(2));
        zoo.AddThing(new Thing(100, "Table"));

        zoo.ReportFood();
        zoo.ContactZooAnimals();
    }
}
