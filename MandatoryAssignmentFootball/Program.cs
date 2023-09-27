using footballAssignment.models;
using footballAssignment.Helpers;

class FootballsApplication
{

    static void Main()
    {
        Football();
        FootballTop();
        FootballBottom();
    }

    static async void Football()
    {
        //string outputFilePath = Directory.GetCurrentDirectory() + "\\CSV_files\\results.CSV"; // The path to the output "Results" CSV file this will write out data to our bin file rather than the actual CSV file.
        string inputDirectoryPath = Directory.GetCurrentDirectory() + "\\CSV_files\\CSV_rounds\\"; // Replace with your directory path
        var outputFilePath = Directories.GetSourceDirectory() + "\\CSV_files\\results.csv"; //this function takes the helper function, and directs to our source file, and places information in our CSV file


        // Ensure the input directory exists
        if (!Directory.Exists(inputDirectoryPath))
        {
            throw new Exception($"Input directory '{inputDirectoryPath}' not found.");
        }

        // Initialize the output file (create or clear it)
        File.WriteAllText(outputFilePath, "");

        // Dictionary to store team names and their total points
        Dictionary<string, int> teamGoals = new Dictionary<string, int>();
        Dictionary<string, int> teamPoints = new Dictionary<string, int>();
        Dictionary<string, int> goalsAgainst = new Dictionary<string, int>();
        Dictionary<string, int> matchesPlayed = new Dictionary<string, int>();
        Dictionary<string, int> matchesWon = new Dictionary<string, int>();
        Dictionary<string, int> matchesLost = new Dictionary<string, int>();
        Dictionary<string, int> matchesTied = new Dictionary<string, int>();

        var roundsProcessed = 0;

        for (int roundNumber = 1; roundNumber <= 22; roundNumber++)
        {
            string roundFileName = $"round-{roundNumber:00}.CSV"; // Formatted file name
            string roundFilePath = Path.Combine(inputDirectoryPath, roundFileName);

            if (File.Exists(roundFilePath))
            {
                //Console.WriteLine($"Processing {roundFileName}...");
                roundsProcessed++;
                foreach (var round in CSV_to_GameResult(roundFilePath))
                {
                    // Ensure that the homeTeam exists in the dictionary
                    if (teamGoals.ContainsKey(round.homeTeam) == false && teamPoints.ContainsKey(round.homeTeam) == false &&
                        goalsAgainst.ContainsKey(round.homeTeam) == false && matchesPlayed.ContainsKey(round.homeTeam) == false &&
                        matchesWon.ContainsKey(round.homeTeam) == false && matchesLost.ContainsKey(round.homeTeam) == false &&
                        matchesTied.ContainsKey(round.homeTeam) == false)
                    {
                        teamGoals[round.homeTeam] = 0;
                        teamPoints[round.homeTeam] = 0;
                        goalsAgainst[round.homeTeam] = 0;
                        matchesPlayed[round.homeTeam] = 0;
                        matchesWon[round.homeTeam] = 0;
                        matchesLost[round.homeTeam] = 0;
                        matchesTied[round.homeTeam] = 0;

                    }

                    // Ensure that the awayTeam exists in the dictionary
                    if (teamGoals.ContainsKey(round.awayTeam) == false && teamPoints.ContainsKey(round.awayTeam) == false &&
                        goalsAgainst.ContainsKey(round.awayTeam) == false && matchesPlayed.ContainsKey(round.awayTeam) == false && matchesWon.ContainsKey(round.awayTeam) == false
                        && matchesLost.ContainsKey(round.awayTeam) == false && matchesTied.ContainsKey(round.awayTeam) == false)
                    {
                        teamGoals[round.awayTeam] = 0;
                        teamPoints[round.awayTeam] = 0;
                        goalsAgainst[round.awayTeam] = 0;
                        matchesPlayed[round.awayTeam] = 0;
                        matchesWon[round.awayTeam] = 0;
                        matchesLost[round.awayTeam] = 0;
                        matchesTied[round.awayTeam] = 0;

                    }

                    teamGoals[round.homeTeam] += round.homeScore;
                    teamGoals[round.awayTeam] += round.awayScore;

                    teamPoints[round.homeTeam] += round.homePointsForMatch;
                    teamPoints[round.awayTeam] += round.awayPointsForMatch;

                    goalsAgainst[round.homeTeam] += round.awayScore;
                    goalsAgainst[round.awayTeam] += round.homeScore;

                    matchesPlayed[round.homeTeam]++;
                    matchesPlayed[round.awayTeam]++;

                    //should be moved out as a seperate method, to avoid writing same code multiple times.
                    if (round.homePointsForMatch == 3)
                    {
                        matchesWon[round.homeTeam]++;
                        matchesLost[round.awayTeam]++;
                    }
                    else if (round.awayPointsForMatch == 3)
                    {
                        matchesWon[round.awayTeam]++;
                        matchesLost[round.homeTeam]++;
                    }
                    else
                    {
                        matchesTied[round.homeTeam]++;
                        matchesTied[round.awayTeam]++;
                    }
                }
            }
            else
            {
                throw new Exception($"{roundFileName} not found.");
            }
        }

        // Write the total points for each team to the "Results.csv" file
        Console.WriteLine("All teams:");
        WriteTotalsToOutput(teamPoints, teamGoals, goalsAgainst, matchesPlayed, matchesWon, matchesLost, matchesTied, outputFilePath);

        Console.WriteLine("");

        Console.WriteLine($"Processed {roundsProcessed} matches");
        Console.WriteLine("");

        Console.WriteLine("Top six:");
        ReadFirstSixLines();

        Console.WriteLine("");

        Console.WriteLine("Bottom six:");
        LastLines();
    }

    static GameResult[] CSV_to_GameResult(string file)
    {
        return File.ReadLines(file)
            .Skip(1)
            .Select(line =>
            {
                string[] columns = TrimElementsInArray(line.Split(','));

                return new GameResult()
                {
                    homeTeam = columns[0],
                    awayTeam = columns[1],
                    homeScore = int.Parse(columns[2]),
                    awayScore = int.Parse(columns[3]),
                    homePointsForMatch = int.Parse(columns[4]),
                    awayPointsForMatch = int.Parse(columns[5])
                };
            })
            .ToArray();
    }

    static string[] TrimElementsInArray(string[] array)
    {
        return array.Select(line => line.Trim()).ToArray();
    }

    static void WriteTotalsToOutput(
        Dictionary<string, int> teamPoints,
        Dictionary<string, int> teamGoals,
        Dictionary<string, int> goalsAgainst,
        Dictionary<string, int> matchesPlayed,
        Dictionary<string, int> matchesWon,
        Dictionary<string, int> matchesLost,
        Dictionary<string, int> matchesTied,
        string outputFilePath)
    {
        try
        {
            // Append a header for the total points and goals section
            File.AppendAllText(outputFilePath, "Total Points, Goal Difference, Goals Scored, Goals Against:, Matches Played:, Matches Won:, Matches Lost: and Matches Tied" + Environment.NewLine);

            // Create a list of KeyValuePair objects for sorting
            var sortedTeams = teamPoints
                .Select(kvp => new KeyValuePair<string, int>(kvp.Key, kvp.Value))
                .OrderByDescending(kvp => kvp.Value)  // Sort by points in descending order
                .ThenByDescending(kvp => teamGoals[kvp.Key] - goalsAgainst.GetValueOrDefault(kvp.Key, 0))  // Then sort by goal difference in descending order
                .ThenByDescending(kvp => teamGoals[kvp.Key])  // Then sort by goals for in descending order
                .ThenBy(kvp => goalsAgainst.GetValueOrDefault(kvp.Key, 0))  // Then sort by goals against in ascending order
                .ThenBy(kvp => kvp.Key)  // Finally, sort alphabetically by team name
                .ToList();

            // Append the total points, goals, goal difference, and goals against for each team
            foreach (var kvp in sortedTeams)
            {
                int goalsFor = teamGoals[kvp.Key];
                int goalsAgainstValue = goalsAgainst.GetValueOrDefault(kvp.Key, 0);
                int goalDifference = goalsFor - goalsAgainstValue;

                string line = $"Team: {kvp.Key}, Total Points: {kvp.Value}, Goal Difference: {goalDifference}, Goals Scored: {goalsFor}, Goals Against: {goalsAgainstValue}, Matches Played: {matchesPlayed[kvp.Key]}, Matches Won: {matchesWon[kvp.Key]}, Matches Lost: {matchesLost[kvp.Key]}, Matches Tied: {matchesTied[kvp.Key]}";
                File.AppendAllText(outputFilePath, line + Environment.NewLine);

                Console.WriteLine(line);
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while writing total points and goals to {outputFilePath}: {ex.Message}");
        }
    }

    static void ReadFirstLine()
    {
        // Define the path to your CSV file
        string resultFilePath = Directory.GetCurrentDirectory() + "\\CSV_files\\resultsPromotion.CSV";


        // Read the first 5 lines from the CSV file
        ReadFirstLine(resultFilePath);
    }

    static void ReadFirstLine(string resultFilePath)
    {
        try
        {
            using (StreamReader reader = new StreamReader(resultFilePath))
            {

                int linesToRead = 2;
                int linesRead = 0;

                while (linesRead < linesToRead)
                {
                    string line = reader.ReadLine();

                    if (line == null)
                        break;

                    // Skip the header line (first line)
                    if (linesRead == 0)
                    {
                        linesRead++;
                        continue;
                    }

                    Console.WriteLine(line);
                    linesRead++;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }

    static void ReadFirstSixLines()
    {
        // Define the path to your CSV file
        string resultFilePath = Directory.GetCurrentDirectory() + "\\CSV_files\\results.CSV";


        // Read the first 5 lines from the CSV file
        ReadFirst6Lines(resultFilePath);
    }

    static void ReadFirst6Lines(string resultFilePath)
    {
        try
        {
            using (StreamReader reader = new StreamReader(resultFilePath))
            {

                int linesToRead = 7;
                int linesRead = 0;

                while (linesRead < linesToRead)
                {
                    string line = reader.ReadLine();

                    if (line == null)
                        break;

                    // Skip the header line (first line)
                    if (linesRead == 0)
                    {
                        linesRead++;
                        continue;
                    }

                    Console.WriteLine(line);
                    linesRead++;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }

static void LastLines()
    {
        string resultFilePath = Directory.GetCurrentDirectory() + "\\CSV_files\\results.CSV";
        // Read the last 6 lines from the CSV file
        List<string> last5Lines = ReadLastNLines(resultFilePath, 6);

        // Process the CSV data as needed
        foreach (string line in last5Lines)
        {
            string[] fields = line.Split(','); // Assuming CSV is comma-separated

            // Process the fields as needed
            foreach (string field in fields)
            {
                Console.Write(field + " ");
            }

            Console.WriteLine(); // Add a new line between CSV rows
        }
    }

 static List<string> ReadLastNLines(string resultFilePath, int n)
    {
        List<string> lastNLines    = new List<string>();

        using (StreamReader reader = new StreamReader(resultFilePath))
        {
            string line;
            Queue<string> linesQueue = new Queue<string>(n);

            // Read and enqueue lines until we have n lines or reach the end of the file
            while ((line = reader.ReadLine()) != null)
            {
                linesQueue.Enqueue(line);
                if (linesQueue.Count > n)
                {
                    linesQueue.Dequeue();
                }
            }

            // Add the last n lines to the result list
            lastNLines.AddRange(linesQueue);
        }

        return lastNLines;
    }
 
 static void LastLine()
    {
        string resultFilePath = Directory.GetCurrentDirectory() + "\\CSV_files\\resultsDemotion.CSV";
        // Read the last 5 lines from the CSV file
        List<string> last5Lines = ReadLastNLines(resultFilePath, 1);

        // Process the CSV data as needed
        foreach (string line in last5Lines)
        {
            string[] fields = line.Split(','); // Assuming CSV is comma-separated

            // Process the fields as needed
            foreach (string field in fields)
            {
                Console.Write(field + " ");
            }

            Console.WriteLine(); // Add a new line between CSV rows
        }
    }

    static void FootballTop()
    {
        string outputFilePath = Directory.GetCurrentDirectory() + "\\CSV_files\\resultsPromotion.CSV"; // The path to the output "ResultsPromotion" CSV file
        string inputDirectoryPath = Directory.GetCurrentDirectory() + "\\CSV_files\\CSV_rounds\\"; // Replace with your directory path


        // Ensure the input directory exists
        if (!Directory.Exists(inputDirectoryPath))
        {
            throw new Exception($"Input directory '{inputDirectoryPath}' not found.");
        }

        // Initialize the output file (create or clear it)
        File.WriteAllText(outputFilePath, "");

        // Dictionary to store team names and their total points
        Dictionary<string, int> teamGoals = new Dictionary<string, int>();
        Dictionary<string, int> teamPoints = new Dictionary<string, int>();
        Dictionary<string, int> goalsAgainst = new Dictionary<string, int>();
        Dictionary<string, int> matchesPlayed = new Dictionary<string, int>();
        Dictionary<string, int> matchesWon = new Dictionary<string, int>();
        Dictionary<string, int> matchesLost = new Dictionary<string, int>();
        Dictionary<string, int> matchesTied = new Dictionary<string, int>();

        var roundsProcessed = 0;

        for (int roundNumber = 23; roundNumber <= 32; roundNumber++)
        {
            string roundFileName = $"round-{roundNumber:00}.CSV"; // Formatted file name
            string roundFilePath = Path.Combine(inputDirectoryPath, roundFileName);

            if (File.Exists(roundFilePath))
            {
                //Console.WriteLine($"Processing {roundFileName}...");
                roundsProcessed++;
                foreach (var round in CSV_to_GameResult(roundFilePath))
                {

                    // Ensure that the homeTeam exists in the dictionary
                    if (teamGoals.ContainsKey(round.homeTeam) == false && teamPoints.ContainsKey(round.homeTeam) == false && goalsAgainst.ContainsKey(round.homeTeam) == false
                        && matchesPlayed.ContainsKey(round.homeTeam) == false &&
                        matchesWon.ContainsKey(round.homeTeam) == false && matchesLost.ContainsKey(round.homeTeam) == false &&
                        matchesTied.ContainsKey(round.homeTeam) == false)
                    {
                        teamGoals[round.homeTeam] = 0;
                        teamPoints[round.homeTeam] = 0;
                        goalsAgainst[round.homeTeam] = 0;
                        matchesPlayed[round.homeTeam] = 0;
                        matchesWon[round.homeTeam] = 0;
                        matchesLost[round.homeTeam] = 0;
                        matchesTied[round.homeTeam] = 0;
                    }

                    // Ensure that the awayTeam exists in the dictionary
                    if (teamGoals.ContainsKey(round.awayTeam) == false && teamPoints.ContainsKey(round.awayTeam) == false && goalsAgainst.ContainsKey(round.awayTeam) == false
                        && matchesPlayed.ContainsKey(round.awayTeam) == false && matchesPlayed.ContainsKey(round.awayTeam) == false && matchesWon.ContainsKey(round.awayTeam) == false
                        && matchesLost.ContainsKey(round.awayTeam) == false && matchesTied.ContainsKey(round.awayTeam) == false)
                    {
                        teamGoals[round.awayTeam] = 0;
                        teamPoints[round.awayTeam] = 0;
                        goalsAgainst[round.awayTeam] = 0;
                        matchesPlayed[round.awayTeam] = 0;
                        matchesWon[round.awayTeam] = 0;
                        matchesLost[round.awayTeam] = 0;
                        matchesTied[round.awayTeam] = 0;
                    }

                    teamGoals[round.homeTeam] += round.homeScore;
                    teamGoals[round.awayTeam] += round.awayScore;

                    teamPoints[round.homeTeam] += round.homePointsForMatch;
                    teamPoints[round.awayTeam] += round.awayPointsForMatch;

                    goalsAgainst[round.homeTeam] += round.awayScore;
                    goalsAgainst[round.awayTeam] += round.homeScore;

                    matchesPlayed[round.homeTeam]++;
                    matchesPlayed[round.awayTeam]++;

                    if (round.homePointsForMatch == 3)
                    {
                        matchesWon[round.homeTeam]++;
                        matchesLost[round.awayTeam]++;
                    }
                    else if (round.awayPointsForMatch == 3)
                    {
                        matchesWon[round.awayTeam]++;
                        matchesLost[round.homeTeam]++;
                    }
                    else
                    {
                        matchesTied[round.homeTeam]++;
                        matchesTied[round.awayTeam]++;
                    }
                }
            }
            else
            {
                throw new Exception($"{roundFileName} not found.");
            }
        }

        // Write the total points for each team to the "Results.csv" file
        Console.WriteLine("");
        Console.WriteLine("Top six teams rounds:");
        WriteTotalsToOutput(teamPoints, teamGoals, goalsAgainst, matchesPlayed, matchesWon, matchesLost, matchesTied, outputFilePath);

        Console.WriteLine("");

        Console.WriteLine($"Processed {roundsProcessed} matches");
        Console.WriteLine("");

        Console.WriteLine("Winner:");
        ReadFirstLine();

    }


static void FootballBottom()
    {
        string outputFilePath = Directory.GetCurrentDirectory() + "\\CSV_files\\resultsDemotion.CSV"; // The path to the output "Results" CSV file
        string inputDirectoryPath = Directory.GetCurrentDirectory() + "\\CSV_files\\CSV_rounds\\"; // Replace with your directory path


        // Ensure the input directory exists
        if (!Directory.Exists(inputDirectoryPath))
        {
            throw new Exception($"Input directory '{inputDirectoryPath}' not found.");
        }

        // Initialize the output file (create or clear it)
        File.WriteAllText(outputFilePath, "");

        // Dictionary to store team names and their total points
        Dictionary<string, int> teamGoals     = new Dictionary<string, int>();
        Dictionary<string, int> teamPoints    = new Dictionary<string, int>();
        Dictionary<string, int> goalsAgainst  = new Dictionary<string, int>();
        Dictionary<string, int> matchesPlayed = new Dictionary<string, int>();
        Dictionary<string, int> matchesWon    = new Dictionary<string, int>();
        Dictionary<string, int> matchesLost   = new Dictionary<string, int>();
        Dictionary<string, int> matchesTied   = new Dictionary<string, int>();

        var roundsProcessed  = 0;

        for (int roundNumber = 33; roundNumber <= 42; roundNumber++)
        {
            string roundFileName = $"round-{roundNumber:00}.CSV"; // Formatted file name
            string roundFilePath = Path.Combine(inputDirectoryPath, roundFileName);

            if (File.Exists(roundFilePath))
            {
                //Console.WriteLine($"Processing {roundFileName}...");
                roundsProcessed++;
                foreach (var round in CSV_to_GameResult(roundFilePath))
                {

                    // Ensure that the homeTeam exists in the dictionary
                    if (teamGoals.ContainsKey(round.homeTeam) == false && teamPoints.ContainsKey(round.homeTeam) == false && goalsAgainst.ContainsKey(round.homeTeam) == false
                        && matchesPlayed.ContainsKey(round.homeTeam) == false &&
                        matchesWon.ContainsKey(round.homeTeam) == false && matchesLost.ContainsKey(round.homeTeam) == false &&
                        matchesTied.ContainsKey(round.homeTeam) == false)
                    {
                        teamGoals[round.homeTeam]     = 0;
                        teamPoints[round.homeTeam]    = 0;
                        goalsAgainst[round.homeTeam]  = 0;
                        matchesPlayed[round.homeTeam] = 0;
                        matchesWon[round.homeTeam]    = 0;
                        matchesLost[round.homeTeam]   = 0;
                        matchesTied[round.homeTeam]   = 0;
                    }

                    // Ensure that the awayTeam exists in the dictionary
                    if (teamGoals.ContainsKey(round.awayTeam) == false && teamPoints.ContainsKey(round.awayTeam) == false && goalsAgainst.ContainsKey(round.awayTeam) == false
                        && matchesPlayed.ContainsKey(round.awayTeam) == false && matchesWon.ContainsKey(round.awayTeam) == false
                        && matchesLost.ContainsKey(round.awayTeam) == false && matchesTied.ContainsKey(round.awayTeam) == false)
                    {
                        teamGoals[round.awayTeam]     = 0;
                        teamPoints[round.awayTeam]    = 0;
                        goalsAgainst[round.awayTeam]  = 0;
                        matchesPlayed[round.awayTeam] = 0;
                        matchesWon[round.awayTeam]    = 0;
                        matchesLost[round.awayTeam]   = 0;
                        matchesTied[round.awayTeam]   = 0;
                    }

                    teamGoals[round.homeTeam]         += round.homeScore;
                    teamGoals[round.awayTeam]         += round.awayScore;

                    teamPoints[round.homeTeam]        += round.homePointsForMatch;
                    teamPoints[round.awayTeam]        += round.awayPointsForMatch;

                    goalsAgainst[round.homeTeam]      += round.awayScore;
                    goalsAgainst[round.awayTeam]      += round.homeScore;

                    matchesPlayed[round.homeTeam]     ++;
                    matchesPlayed[round.awayTeam]     ++;

                    if (round.homePointsForMatch      == 3)
                    {
                        matchesWon[round.homeTeam]    ++;
                        matchesLost[round.awayTeam]   ++;
                    }
                    else if (round.awayPointsForMatch == 3)
                    {
                        matchesWon[round.awayTeam]    ++;
                        matchesLost[round.homeTeam]   ++;
                    }
                    else
                    {
                        matchesTied[round.homeTeam]   ++;
                        matchesTied[round.awayTeam]   ++;
                    }
                }
            }
            else
            {
                throw new Exception($"{roundFileName} not found.");
            }
        }

        // Write the total points for each team to the "Results.csv" file
        Console.WriteLine("");
        Console.WriteLine("Bottom six teams rounds:");
        WriteTotalsToOutput(teamPoints, teamGoals, goalsAgainst, matchesPlayed, matchesWon, matchesLost, matchesTied, outputFilePath);

        Console.WriteLine("");

        Console.WriteLine($"Processed {roundsProcessed} matches");
        Console.WriteLine("");

        Console.WriteLine("Team to be demoted:");
        LastLine();
    }
}
