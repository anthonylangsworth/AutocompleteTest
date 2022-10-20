# AutocompleteTest

Minimal code to demonstrate a potential issue regarding autocomplete in Discord.Net. These instructions assume basic understanding of Discord, C# and the Discord.Net library.

**To reproduce the issue:**
1. Create a new Discord application at https://discord.com/developers/applications and convert it to a bot.
2. Get the API key and place it in the AUTOCOMPLETETEST_DISCORD_APIKEY environment variable.
3. Download the source code, compile it and run it.
4. Join the bot to a Discord guild using `https://discordapp.com/oauth2/authorize?client_id=CLIENT_ID&scope=bot&permissions=0`, replacing `CLIENT_ID` with your Discord bot's client ID.
5. Wait a moment for the commands to be registered.
6. Use the `/hello name` command to get an ephemeral message "Hello name".

**Expected behaviour:** The `EchoAutocompleteHandler` autocomplete command fires, suggesting the names of Donald Duck's nephews (https://en.wikipedia.org/wiki/Duck_family_(Disney)#Huey,_Dewey_and_Louie_Duck).

**Actual behaviour:** No autocomplete command is found in `DiscordClient_AutocompleteExecuted`. The autocompletion is registered but the Interaction framework cannot find the command. A sample console output:

```
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
```

**What have I tried?**
1. I have followed the documentation at https://discordnet.dev/guides/int_framework/autocompletion.html as best I am able.
2. I tried the alternate syntax at https://discordnet.dev/guides/int_framework/intro.html#autocomplete-commands, also to no avail.
3. Different variations, such as registring the commands globally instead of per guild.
4. I am using Visual Studio 2022 with .Net 6.0. This error occurs running locally and in a container.
