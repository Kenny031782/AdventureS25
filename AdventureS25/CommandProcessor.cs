namespace AdventureS25;

public static class CommandProcessor
{
    public static Command Process()
    {
        string rawInput = GetInput();
        
        Command command = Parser.Parse(rawInput);

        Debugger.Write("Verb: [" + command.Verb + "]");
        Debugger.Write("Noun: [" + command.Noun + "]");
        
        bool isValid = CommandValidator.IsValid(command);
        command.IsValid = isValid;
        
        return command;
    }
    
    public static string GetInput()
    {
        Console.Write("---------> ");
        string input = Console.ReadLine();
        return input;
    }
}