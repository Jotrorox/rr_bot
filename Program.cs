using DSharpPlus;
using DSharpPlus.SlashCommands;
using Microsoft.Data.Sqlite;
using rr_bot;

await using (var connection = new SqliteConnection("Data Source=database.db"))
{
    connection.Open();
    var command = connection.CreateCommand();
    command.CommandText =
        @"CREATE TABLE IF NOT EXISTS `reactionRoles` ( `message` INTEGER NOT NULL, `emoji` TEXT NOT NULL, `role` INTEGER NOT NULL );";
    command.ExecuteNonQuery();
}

String token = Environment.GetEnvironmentVariable("DISCORD_TOKEN");

// Setup Client
var clientConfig = new DiscordConfiguration
{
    Token = token,
    TokenType = TokenType.Bot,
    Intents = DiscordIntents.GuildMessageReactions |
                DiscordIntents.AllUnprivileged
};

var discordClient = new DiscordClient(clientConfig);

// Register Slash Commands
discordClient.UseSlashCommands().RegisterCommands<ReactionRolesSlashCommands>();

// Bind Event Handlers
discordClient.MessageReactionAdded += ReactionRoleHandler.ReactionEventHandler;

// Launch Client
await discordClient.ConnectAsync();
await Task.Delay(-1);
