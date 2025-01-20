# Advent of Code 2024
Solutions to [Advent of Code 2024](https://adventofcode.com/2024)

I thought I wouldn't have any time for Advent of Code this year, but I still wanted to get a few in at the end.

As usual, I'll probably keep solving these over time, so I'll track which solutions I've done, in the chart below.

## Progress

|      |1|2|3|4|5|6|7|8|9|10|11|12|13|14|15|16|17|18|19|20|21|22|23|24|25|
|------|-|-|-|-|-|-|-|-|-|--|--|--|--|--|--|--|--|--|--|--|--|--|--|--|--|
|**C#**|🌟|🌟|🌟|🌟|🌟|🌟|🌟|🌟|🌟|🌟|🌟|🌟|⭐|  |  |  |  |  |  |  |  |  |  |  |  |

⭐ - First star completed\
🌟 - Both stars completed

## Results
I've added `output.txt` as a reference for my answers (for convenient checking while refactoring) and runtimes of my solutions.

C# - [output.txt](csharp/output.txt)

![Average runtimes](RuntimesChart.png)

### Updating results
To generate new output txt file
```
dotnet run --project .\AdventOfCode2024\AdventOfCode2024.csproj  --configuration=Release > output.txt
```

To update chart
1. Generate new runtimes:
```
dotnet run --project .\AdventOfCode2024\AdventOfCode2024.csproj --configuration=AverageRuntimes
```
2. Copy the last lines (after the "------") into the Excel file
3. Save chart to image as "RuntimesChart.png"

I know! It's a manual process. That bugs me too. I'm open to suggestions, but I do want to keep it flexible for multiple languages and edge cases.

## Environment
* Visual Studio 2022
* [.NET 8](https://dotnet.microsoft.com/download/dotnet/8.0) / C# 12

## Links
* [Advent of Code](https://adventofcode.com)
* [2023 Solutions](../2023/)