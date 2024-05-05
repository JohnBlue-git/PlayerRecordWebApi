using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("PlayerRecordsApi.Tests")]

namespace PlayerRecordsApi.Models;

public class PlayerRecords
{
    private static ConcurrentDictionary<long, Player> Records;

    static PlayerRecords()
    {
        Records = new ConcurrentDictionary<long, Player>();

        RestoreRecords();
    }

    private static ConcurrentDictionary<long, Player> GetRecords()
    {
        return Records;
    }

    internal static void RestoreRecords()
    {
        // clear
        Records.Clear();

        // defualt values for testing
        Records.TryAdd(0, new Player(new PublicPlayer(0, "Noah", gameState.Win, 3, DateTime.Now)));
        Records.TryAdd(1, new Player(new PublicPlayer(1, "Liam", gameState.Lose, -1, DateTime.Now)));
        Records.TryAdd(2, new Player(new PublicPlayer(2, "Noah", gameState.Win, 3, DateTime.Now)));
        Records.TryAdd(3, new Player(new PublicPlayer(3, "William", gameState.Lose, -1, DateTime.Now)));
        Records.TryAdd(4, new Player(new PublicPlayer(4, "William", gameState.Draw, 1, DateTime.Now)));
        Records.TryAdd(5, new Player(new PublicPlayer(5, "Liam", gameState.Draw, 1, DateTime.Now)));

        // name: Noah Liam William Leo Theodore ...
    }

    public static async Task<bool> IsPlayerExistsAsync(long id)
    {
        bool check = false;
        await Task.Run(() => {
            check = Records.ContainsKey(id);
        });
        return check;
    }

    public static async Task<List<PublicPlayer>> GetPlayerAsync()
    {
        List<PublicPlayer> players = new List<PublicPlayer>();
        await Task.Run(() => {
            foreach(Player player in Records.Values)
            {
                if (player.IsSecret == true)
                {
                    continue;
                }

                players.Add(new PublicPlayer(player));
            }            
        });
        return players;
    }

    public static async Task<PublicPlayer?> GetPlayerAsync(long id)
    {
        Player? player = null;
        await Task.Run(() => {
            Records.TryGetValue(id, out player);
        });
        if (player == null || player.IsSecret == true)
        {
            return null;
        }
        else
        {
            return new PublicPlayer(player);
        }
    }

    public static async Task PostPlayerAsync(PublicPlayer player)
    {
        if (await IsPlayerExistsAsync(player.Id) == true)
        {
            return;
        }
        await Task.Run(() => {
            Records.TryAdd(player.Id, new Player(player));
        });
    }

    public static async Task PutPlayerAsync(PublicPlayer player)
    {
        if (await IsPlayerExistsAsync(player.Id) == false)
        {
            return;
        }
        await Task.Run(() => {
            Records[player.Id] = new Player(player);
        });
    }

    public static async Task DeletePlayerAsync(long id)
    {
        await Task.Run(() => {
            Records.TryRemove(id, out Player player);
        });
    }

    public static async Task MarkRecordSecretAsync(long id)
    {
        if (await IsPlayerExistsAsync(id) == false)
        {
            return;
        }
        await Task.Run(() => {
            Player player = Records[id];
            player.IsSecret = true;
        });
    }

    public static async Task<List<PublicPlayer>> GetSummaryAsync()
    {
        List<PublicPlayer> players = new List<PublicPlayer>();
        await Task.Run(() => {
            int id = 0;
            players.AddRange(
                Records.Values
                .GroupBy(x => new { x.Name })
                .Select(x =>
                {
                    Player player = new Player(x.First());
                    player.Id = id++;
                    player.State = gameState.Summary;
                    player.Points = x.Sum(xv => xv.Points);
                    player.WhatTime = new DateTime();
                    return new PublicPlayer(player);
                })
            );
        });
        return players;
    }

    public static async Task<int> GetPlayerPointsSummaryAsync(string name)
    {
        IEnumerable<Player> record = new List<Player>();
        await Task.Run(() => {
            record
                = Records.Values
                .Where(x => x.Name == name);
        });
        return record.Any() ? record.Sum(x => x.Points) : 0;
    }
}
