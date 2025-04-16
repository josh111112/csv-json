using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;



/*
learned from: 
https://joshclose.github.io/CsvHelper/getting-started/#reading-a-csv-file
*/
public class Plswork
{
    public static async Task Main(){
        using (var reader = new StreamReader("C:\\Users\\josha\\Documents\\GitHub\\csv-json\\PremierLeagueMatches.csv"))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Context.RegisterClassMap<DataMap>();
            csv.Read();
            csv.ReadHeader();
        
            var record = csv.GetRecords<Data>().ToList();
        
            // Find matches with high xG difference
            IEnumerable<Data> XGDifference = 
                from match in record
                where match.HomeXG > 1.5
                select match;
            
            // Find matches with high scoring games
            IEnumerable<Data> highScoringGames = 
                from match in record
                where (match.HomeScore + match.AwayScore) > 4
                select match;

            // Find matches where the underdog won (based on xG)
            IEnumerable<Data> underdogWins = 
                from match in record
                where match.HomeXG < match.AwayXG && match.HomeScore > match.AwayScore
                select match;


            // TODO
            // Read System.Text.Json documentation
            string filename = "C:\\Users\\josha\\Documents\\GitHub\\csv-json\\plswork.json";
            await using FileStream createStream = File.Create(filename);
            await JsonSerializer.SerializeAsync(createStream, XGDifference);
            await JsonSerializer.SerializeAsync(createStream, highScoringGames);
            await JsonSerializer.SerializeAsync(createStream, underdogWins);
        }
    }
}

public class Data
{
    [Name("Matchday")]
    public int? MatchDay { get; set; }
    [Name("Date")]
    public string? Date { get; set; }
    [Name("Time")]
    public string? Time { get; set; }
    [Name("Home Team")]
    public string? HomeTeam { get; set; }
    [Name("homeScore")]
    public int? HomeScore { get; set; }
    [Name("homeXG")]
    public double? HomeXG { get; set; }
    [Name("awayScore")]
    public int? AwayScore { get; set; }
    [Name("awayXG")]
    public double? AwayXG { get; set; }
    [Name("Away Team")]
    public string? AwayTeam { get; set; }
    [Name("Attendance")]
    public string? Attendance { get; set; }
    [Name("Referee")]
    public string? Referee { get; set; }
    [Name("Stadium")]
    public string? Stadium { get; set; }
    [Name("Result")]
    public string? Result { get; set; }
    [Name("*Additional Stats")]
    public string? AddiStats { get; set; }
}
// Matchday,Date,Time,Home Team,homeScore,homeXG,awayScore,awayXG,Away Team,Attendance,Referee,Stadium,Result, *Additional Stats
public class DataMap: ClassMap<Data>
{
    public DataMap()
    {
        Map(m => m.MatchDay).Name("Matchday");
        Map(m => m.Date).Name("Date");
        Map(m => m.Time).Name("Time");
        Map(m => m.HomeTeam).Name("Home Team");
        Map(m => m.HomeScore).Name("homeScore");
        Map(m => m.HomeXG).Name("homeXG");
        Map(m => m.AwayScore).Name("awayScore");
        Map(m => m.AwayXG).Name("awayXG");
        Map(m => m.AwayTeam).Name("Away Team");
        Map(m => m.Attendance).Name("Attendance");
        Map(m => m.Referee).Name("Referee");
        Map(m => m.Stadium).Name("Stadium");
        Map(m => m.Result).Name("Result");
        Map(m => m.AddiStats).Name("*Additional Stats");
    }
}
