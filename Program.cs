using DSharpPlus;
using DSharpPlus.SlashCommands;
using Microsoft.Data.Sqlite;

namespace rr_bot;

internal class Program
{
    static async Task Main(string[] args)
    {
        await using (var connection = new SqliteConnection("Data Source=database.db"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText =
                @"CREATE TABLE IF NOT EXISTS `reactionRoles` ( `message` INTEGER NOT NULL, `emoji` TEXT NOT NULL, `role` INTEGER NOT NULL );";
            command.ExecuteNonQuery();
        }

        String token = File.ReadAllText("token.conf").Trim();

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
    }
}