using System.ComponentModel.DataAnnotations;

namespace PlayerRecordsApi.Models;



public enum gameState {
    Win,
    Lose,
    Draw,
    Summary,
}

public class Player
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public gameState State { get; set; }
    public int Points { get; set; }
    public DateTime WhatTime { get; set; } = DateTime.Now;
    public bool IsSecret { get; set; } = false;

    public Player() {}

    public Player(long id, string name, gameState state, int points, DateTime whatTime, bool secret)
    {
        Id = id;
        Name = name;
        State = state;
        Points = points;
        WhatTime = whatTime;
        IsSecret = secret;
    }

    public Player(Player copy)
    {
        Id = copy.Id;
        Name = copy.Name;
        State = copy.State;
        Points = copy.Points;
        WhatTime = copy.WhatTime;
        IsSecret = copy.IsSecret;
    }

    public Player(PublicPlayer copy)
    {
        Id = copy.Id;
        Name = copy.Name;
        State = copy.State;
        Points = copy.Points;
        WhatTime = copy.WhatTime;
    }
}

public class PublicPlayer
{
    public long Id { get; set; }
    [Required(AllowEmptyStrings = false, ErrorMessage = "Name is empty")]
    public string Name { get; set; } = string.Empty;
    [EnumDataType(typeof(gameState), ErrorMessage = "State need to fill with enum gameState")]
    public gameState State { get; set; }
    public int Points { get; set; }
    [DataType(DataType.DateTime, ErrorMessage = "WhatTime need to fill with DateTime")]
    public DateTime WhatTime { get; set; } = DateTime.Now;

    public PublicPlayer() {}

    public PublicPlayer(long id, string name, gameState state, int points)
    {
        Id = id;
        Name = name;
        State = state;
        Points = points;
    }

    public PublicPlayer(long id, string name, gameState state, int points, DateTime whatTime)
    {
        Id = id;
        Name = name;
        State = state;
        Points = points;
        WhatTime = whatTime;
    }

    public PublicPlayer(PublicPlayer copy)
    {
        Id = copy.Id;
        Name = copy.Name;
        State = copy.State;
        Points = copy.Points;
        WhatTime = copy.WhatTime;
    }

    public PublicPlayer(Player copy)
    {
        Id = copy.Id;
        Name = copy.Name;
        State = copy.State;
        Points = copy.Points;
        WhatTime = copy.WhatTime;
    }
}

