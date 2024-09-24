using System.Diagnostics;
using System.Text.Json;
using Discord;
using Discord.WebSocket;

var token = Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN");
if (string.IsNullOrEmpty(token))
{
    Console.Error.WriteLine("DISCORD_BOT_TOKEN env variable is not set.");
    return;
}

string? guidId;
if (args.Length >= 1)
{
    guidId = args[0];
}
else
{
    guidId = Console.ReadLine();
}

if (string.IsNullOrWhiteSpace(guidId))
{
    Console.Error.WriteLine("Guild ID is required.");
    return;
}

var options = new DiscordSocketConfig
{
    GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers
};

var client = new DiscordSocketClient(options);

client.Log += (log) =>
{
    Debug.WriteLine(log.Message);
    return Task.CompletedTask;
};

client.Ready += async () =>
{
    var guild = client.GetGuild(ulong.Parse(guidId));
    var users = new List<UserDetails>();

    foreach (var user in await guild.GetUsersAsync().FlattenAsync())
    {
        users.Add(new UserDetails(user.Id, user.Username, user.DisplayName, user.JoinedAt));
    }

    var json = JsonSerializer.Serialize(users);
    Console.WriteLine(json);
};

await client.LoginAsync(TokenType.Bot, token);
await client.StartAsync();

await Task.Delay(-1);

record UserDetails(ulong Id, string Username, string DisplayName, DateTimeOffset? JoinTime);
