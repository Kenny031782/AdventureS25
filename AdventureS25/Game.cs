using System.ComponentModel.DataAnnotations;
using System.Net.Mime;

namespace AdventureS25;

public static class Game
{
    private static bool isPlaying = true;
    
    public static void PlayGame()
    {
        Initialize();
        
        Console.WriteLine(" ----- Type 'help' for the list of commands.");

        Console.WriteLine(Player.GetLocationDescription());
        
        while (isPlaying == true)
        {
            Command command = CommandProcessor.Process();
            
            if (command.IsValid)
            {
                if (command.Verb == "exit")
                {
                    Console.WriteLine("Game Over!");
                    isPlaying = false;
                }
                else
                {
                    CommandHandler.Handle(command);
                    if (Player.IsGameOver())
                        isPlaying = false;
                }
            }
        }
    }

    private static void Initialize()
    {
        Conditions.Initialize();
        States.Initialize();
        Map.Initialize();
        Items.Initialize();
        Player.Initialize();
    }

    public static void EndGame()
    {
        
        Console.Write("You killed kenny you bastard!");
        isPlaying = false;
    }
}