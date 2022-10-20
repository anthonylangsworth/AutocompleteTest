using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Reflection;

using DiscordSocketClient discordClient = new(new DiscordSocketConfig()
{
    GatewayIntents = GatewayIntents.None
});
using InteractionService interactionService = new(discordClient);
discordClient.Log += DiscordClient_Log;
interactionService.Log += DiscordClient_Log;
discordClient.SlashCommandExecuted += DiscordClient_SlashCommandExecutedAsync;
discordClient.AutocompleteExecuted += DiscordClient_AutocompleteExecuted;
discordClient.GuildAvailable += DiscordClient_GuildAvailable;
await interactionService.AddModulesAsync(Assembly.GetExecutingAssembly(), null);
await discordClient.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("AUTOCOMPLETETEST_DISCORD_APIKEY"));
await discordClient.StartAsync();
await Task.Delay(-1);

async Task DiscordClient_Log(LogMessage arg) => await Console.Out.WriteLineAsync(arg.ToString());

async Task DiscordClient_GuildAvailable(SocketGuild guild) => await interactionService.RegisterCommandsToGuildAsync(guild.Id);

async Task DiscordClient_SlashCommandExecutedAsync(SocketSlashCommand socketSlashCommand)
{
    IResult result = await interactionService.ExecuteCommandAsync(
        new SocketInteractionContext(discordClient, socketSlashCommand), null);
    Console.WriteLine($"Command result: {result}");
}

async Task DiscordClient_AutocompleteExecuted(SocketAutocompleteInteraction arg)
{
    IResult result = interactionService.SearchAutocompleteCommand(arg);
    if (result.IsSuccess
        && result is SearchResult<AutocompleteCommandInfo> autocompleteResult
        && autocompleteResult.Command != null)
    {
        result = await autocompleteResult.Command.ExecuteAsync(new SocketInteractionContext(discordClient, arg), null);
    }
    Console.WriteLine($"Autocomplete result: {result}");
}

namespace ModulesAndHandlers
{
    public class CommandModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("hello", "Say hello")]
        public async Task Hello(
            [
                Summary("name", "Say hello to this person"),
                Autocomplete(typeof(EchoAutocompleteHandler))
            ]
            string name) => await Context.Interaction.RespondAsync($"Hello {name}", ephemeral: true);
    }

    public class EchoAutocompleteHandler : AutocompleteHandler
    {
        public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context,
            IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
            => Task.FromResult(AutocompletionResult.FromSuccess(new[] { "Huey", "Dewey", "Louie" }.Select(s => new AutocompleteResult(s, s))));
    }
}

/*
 * Sample result:
 * 
22:51:36 Discord     Discord.Net v3.8.1 (API v10)
22:51:37 Gateway     Connecting
22:51:38 Gateway     Connected
Autocomplete result: UnknownCommand: No Discord.Interactions.AutocompleteCommandInfo found for hello name
Autocomplete result: UnknownCommand: No Discord.Interactions.AutocompleteCommandInfo found for hello name
Command result: Success
22:51:49 Gateway     Ready
Autocomplete result: UnknownCommand: No Discord.Interactions.AutocompleteCommandInfo found for hello name
Autocomplete result: UnknownCommand: No Discord.Interactions.AutocompleteCommandInfo found for hello name
Autocomplete result: UnknownCommand: No Discord.Interactions.AutocompleteCommandInfo found for hello name
Autocomplete result: UnknownCommand: No Discord.Interactions.AutocompleteCommandInfo found for hello name
Command result: Success
*/