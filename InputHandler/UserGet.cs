namespace InputHandler;
static class UserGet
{
    public static DateOnly GetDateOnly(string prompt)
    {
        while (true)
        {
            Prompt(prompt);
            DateOnly output;
            if (DateOnly.TryParse(Console.ReadLine(), out output))
            {
                return output;
            }
            Console.WriteLine("Skriv ett datum.");
        }
    }
    /// <summary>
    /// Prompts user for an int, and tries to parse it until user
    /// supplies valid value
    /// </summary>
    /// <param name="prompt">Prompt message to display</param>
    /// <returns>Valid integer</returns>
    public static int GetInt(string prompt)
    {
        while (true)
        {
            Prompt(prompt);
            int output;
            if (int.TryParse(Console.ReadLine(), out output))
            {
                return output;
            }
            Console.WriteLine("skriv en int.");
        }
    }
    /// <summary>
    /// Prompts user for a char, checks that the input is a digit or letter
    /// before returning it.
    /// </summary>
    /// <param name="prompt">Prompt message to display</param>
    /// <returns>Digit or letter char</returns>
    public static char GetChar(string prompt)
    {
        Prompt(prompt);
        char output;
        do
        {
            output = char.ToUpper(Console.ReadKey(true).KeyChar);
        }
        while (!char.IsLetterOrDigit(output));

        return output;

    }
    /// <summary>
    /// Prompts the user for a char, that must be one of the chars 
    /// supplied. Will ask until valid choice is made.
    /// </summary>
    /// <param name="prompt"></param>
    /// <param name="acceptedChars">Prompt message to display</param>
    /// <returns>Char of the supplied set</returns>
    public static char GetCharOfSet(string prompt, char[] acceptedChars)
    {
        while (true)
        {
            char output;
            output = GetChar(prompt);
            Console.Write(output + "\n");
            if (Contains(output, acceptedChars))
                return output;
            else
                Console.WriteLine("Felaktig input");
        }

    }
    public static bool Contains(char c, char[] chars)
    {
        for (int i = 0; i < chars.Length; i++)
        {
            if (c == chars[i])
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// Prompts the user for a string
    /// </summary>
    /// <param name="prompt">Prompt message to display</param>
    /// <returns>A string</returns>
    public static string GetString(string prompt)
    {
        Prompt(prompt);
        return Console.ReadLine()!;
    }
    
    //The format of the prompt
    private static void Prompt(string prompt) =>
        Console.Write($"{prompt}: ");

}