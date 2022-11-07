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
discordClient.InteractionCreated += DiscordClient_InteractionCreated;
discordClient.GuildAvailable += DiscordClient_GuildAvailable;
await interactionService.AddModulesAsync(Assembly.GetExecutingAssembly(), null);
await discordClient.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("AUTOCOMPLETETEST_DISCORD_APIKEY"));
await discordClient.StartAsync();
await Task.Delay(-1);

async Task DiscordClient_Log(LogMessage arg) => await Console.Out.WriteLineAsync(arg.ToString());

async Task DiscordClient_GuildAvailable(SocketGuild guild) => await interactionService.RegisterCommandsToGuildAsync(guild.Id);

async Task DiscordClient_InteractionCreated(SocketInteraction interaction)
{
    try
    {
        // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules.
        var context = new SocketInteractionContext(discordClient, interaction);

        // Execute the incoming command.
        var result = await interactionService.ExecuteCommandAsync(context, null);

        if (!result.IsSuccess)
            switch (result.Error)
            {
                case InteractionCommandError.UnmetPrecondition:
                    // implement
                    break;
                default:
                    break;
            }
    }
    catch
    {
        // If Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
        // response, or at least let the user know that something went wrong during the command execution.
        if (interaction.Type is InteractionType.ApplicationCommand)
            await interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
    }
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