namespace AdventureS25;

public static class Player
{
    public static Location CurrentLocation;
    public static List<Item> Inventory;

    public static void Initialize()
    {
        Inventory = new List<Item>();
        CurrentLocation = Map.StartLocation;
    }

    public static void Move(Command command)
    {
        if (CurrentLocation.CanMoveInDirection(command))
        {
            CurrentLocation = CurrentLocation.GetLocationInDirection(command);
            Console.WriteLine(CurrentLocation.GetDescription());
        }
        else
        {
            Console.WriteLine("You can't move " + command.Noun + ".");
        }
    }

    public static string GetLocationDescription()
    {
        return CurrentLocation.GetDescription();
    }

    public static void Take(Command command)
    {
        // figure out which item to take: turn the noun into an item
        Item item = Items.GetItemByName(command.Noun);

        if (item == null)
        {
            Console.WriteLine("I don't know what " + command.Noun + " is.");
        }
        else if (!CurrentLocation.HasItem(item))
        {
            Console.WriteLine("There is no " + command.Noun + " here.");
        }
        else if (!item.IsTakeable)
        {
            Console.WriteLine("The " + command.Noun + " can't be taked.");
        }
        else if (command.Noun == "axe")
        {
            Console.WriteLine("You steal the axe from the man.");
            Inventory.Add(item);
        }
        else
        {
            Inventory.Add(item);
            CurrentLocation.RemoveItem(item);
            item.Pickup();
            Console.WriteLine("You take the " + command.Noun + ".");
        }
    }

    public static void ShowInventory()
    {
        if (Inventory.Count == 0)
        {
            Console.WriteLine("You are empty-handed.");
        }
        else
        {
            Console.WriteLine("You are carrying:");
            foreach (Item item in Inventory)
            {
                string article = SemanticTools.CreateArticle(item.Name);
                Console.WriteLine(" " + article + " " + item.Name);
            }
        }
    }

    public static void Look()
    {
        Console.WriteLine(CurrentLocation.GetDescription());
    }

    public static void Drop(Command command)
    {       
        Item item = Items.GetItemByName(command.Noun);

        if (item == null)
        {
            string article = SemanticTools.CreateArticle(command.Noun);
            Console.WriteLine("I don't know what " + article + " " + command.Noun + " is.");
        }
        else if (!Inventory.Contains(item))
        {
            Console.WriteLine("You're not carrying the " + command.Noun + ".");
        }
        else
        {
            Inventory.Remove(item);
            CurrentLocation.AddItem(item);
            Console.WriteLine("You drop the " + command.Noun + ".");
        }

    }

    public static void Drink(Command command)
    {
        if (command.Noun == "beer")
        {
            Console.WriteLine("** drinking beer");
            Conditions.ChangeCondition(ConditionTypes.IsDrunk, true);
            RemoveItemFromInventory("beer");
            AddItemToInventory("beer-bottle");
        }

        if (command.Noun == "redpotion")
        {
            Console.WriteLine("** drinking red potion");
            // add a strength effect when you drink the potion (use ConditionActions? maybe)
        }

        if (command.Noun == "greenpotion")
        {
            Console.WriteLine("** drinking green potion");
            AddItemToInventory("greenpotion");
        }
        
    }

    public static void AddItemToInventory(string itemName)
    {
        Item item = Items.GetItemByName(itemName);

        if (item == null)
        {
            return;
        }
        
        Inventory.Add(item);
    }

    public static void RemoveItemFromInventory(string itemName)
    {
        Item item = Items.GetItemByName(itemName);
        if (item == null)
        {
            return;
        }
        Inventory.Remove(item);
    }

    public static void MoveToLocation(string locationName)
    {
        // look up the location object based on the name
        Location newLocation = Map.GetLocationByName(locationName);
        
        // if there's no location with that name
        if (newLocation == null)
        {
            Console.WriteLine("Trying to move to unknown location: " + locationName + ".");
            return;
        }
            
        // set our current location to the new location
        CurrentLocation = newLocation;
        
        // print out a description of the location
        Look();
    }

    public static void Pull(Command command)
    {
        if (command.Noun == "lever")
        {
            Console.WriteLine("** pulling lever");
            Conditions.ChangeCondition(ConditionTypes.IsCreatedConnection, true);
        }
    }

    public static void Open(Command command)
    {
        if (command.Noun == "chest")
        {
            Console.WriteLine("you are too weak, come back later with a tool.");
            
        }
    }

    public static bool HasItem(string itemName)
    {
        return Inventory.Contains(Items.GetItemByName(itemName));
    }

    public static void Break(Command command)
    {
        if (command.Noun == "chest")
        {
            if (!HasItem("axe"))
            {
                Console.WriteLine("You don't have the right tool");
            }
            else if (CurrentLocation.Name != "East Cave Wall")
            {
                Console.WriteLine("There's no chest here.");
            }
            else
            {
                // break the chest
                Console.WriteLine("You break the chest open with the frail axe.");
                Map.AddItem("wooden key", "East Cave Wall");
                Map.RemoveItem("chest", "East Cave Wall");
                Console.WriteLine("There is a muddy, slimy, sticky key inside the broken chest.");
            }
        }
    }
    
    

    public static bool IsGameOver()
    {
        if (CurrentLocation.Name == "Storage")
        {
            Console.WriteLine("The door was booby trapped with a timed bomb, killing you instantly after 5 seconds.");
            return true;
        }
        if (CurrentLocation.Name == "Throne Room")
        {
            Console.WriteLine("The ghost of the fallen king haunts you to death, 'He strangles you'.");
            return true;
        }
        if (CurrentLocation.Name == "Pool")
        {
            Console.WriteLine("You slip and hit your head because you were running on the wet tiles when you shouldn't have.");
            return true;
        }
        // if in lake
        if (CurrentLocation.Name == "Lake")
        {
            if (HasItem("raft"))
            {
                return false;
            }
            Console.WriteLine("The water comes alive and drowns you to death.");
            return true;
        }

        if (HasItem("greenpotion"))
        {
            Console.WriteLine("You chose wrong. You have actually drank poison instead of watermelon mocktail.");
            return true;
        }
        
        // if has item in inventory
        
        
        return false;
    }

    public static void Climb(Command command)
    {
        if (CurrentLocation.Name is not ("East Cave" or "East Cave Wall"))
        {
            Console.WriteLine("There is no wall here. Try somewhere else.");
        }
        if (command.Noun == "up")
        {
            Console.WriteLine("You have climbed up the wall.");
            MoveToLocation("East Cave Wall");
        }
        if (command.Noun == "down")
        {
                Console.WriteLine("You have climbed down the wall.");
                MoveToLocation("East Cave");
        }
    }

    public static void Swim(Command command)
    {
        MoveToLocation("Lake");
    }

    public static void Cut(Command command)
    {
        if (command.Noun is ("tree" or "trees"))
        {
            Console.WriteLine("You have cut the Brazil Nut tree.");
            Console.WriteLine("Now you have all this wood, probably enough to build a raft.");
            AddItemToInventory("wood");
        }
    }

    public static void Build(Command command)
    {
        if (HasItem("wood"))
        {
            Console.WriteLine("You have built a raft out of wood.");
            AddItemToInventory("raft");
        }
    }

    public static void Use(Command command)
    {
        if (HasItem("raft"))
        {
            if (command.Noun == "raft")
            {
                Console.WriteLine("You sail across the lake safe and sound.");
                MoveToLocation("Lake House");
            }
        }
        if (command.Noun == "woodenkey")
        {
            Console.WriteLine("** unlocking door");
            Conditions.ChangeCondition(ConditionTypes.IsUnlockedHouse, true);
            Console.WriteLine("You have unlocked the door. You can go south to enter the house.");
        }
    }

    public static void Help(Command command)
    {
        Console.WriteLine("Command List: \n go \n take \n look \n inventory \n pull \n climb 'up/down' \n break \n cut \n build \n swim \n use");
    }
}