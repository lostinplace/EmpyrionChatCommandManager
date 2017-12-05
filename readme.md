# Empyrion Chat Command Manager

## FAQ

### Q:  What is this?

In the process of building a mod, I found that I needed a simple UI that allowed user interaction through the chat system.  This library makes it easier to create a simple chat-based UI.

### Q: How do I use it?

At a high level, this librarys assumes that any entries that are preceeded by a backslash ("\") are attempted chat commands.  You decide what commands you want to exist, describe them using regex, and then handle those commands.

For example, let's look at a simple command that allows us to execute telnet commands frm the chat window.  Its invocation is simple, `rcc {args}` and when we receive it, we just want to execute the result using `Request_ConsoleCommand(PString)`.

We start by describing the invocation using regex.  in this case:

```regex
rcc (.*)
```

This will match any commands that look like, "\rcc arguments".

Next, we write a command handler.  The command handler takes two arguments, a `ChatInfo` that comes from the API describing basic information about the request, and a `PString` command, which is the string representation of the arguments received

```csharp
private static void execConsoleCommand(ChatInfo data, PString command)
{
    GameAPI.Game_Request(CmdId.Request_ConsoleCommand, 0, command);
    GameAPI.Console_Write($"text: {command.pstr}");
}
```

All that this command handler does is pass the arguments allong to the API as Console Commands.

Once we've defined all of our commands, we integrate it into the mod by instantiating a new chatcommand manager with a dictionary of invocation descriptions and command handlers.  We then add a call to the command manager's `HandleMessage` method inside our `Game_Event` method:

```csharp
public class DebugMod : ModInterface
{
    static ModGameAPI GameAPI;
    static ChatCommandManager chatCommandManager;

    public void Game_Start(ModGameAPI dediAPI)
    {
        DebugMod.GameAPI = dediAPI;
        var chatCommandActions = new Dictionary<String, ChatCommandHandler>
        {
            { @"rcc (.*)", execConsoleCommand  }
        };
        chatCommandManager = new ChatCommandManager(chatCommandActions);
    }
    
    public void Game_Event(CmdId eventId, ushort seqNr, object data)
    {
        chatCommandManager.HandleMessage(eventId, data);
    }
}
```

In-game, a player can bring up a chat window by pressing ".", and type, `TIME 0` and the time will change, even on multiplayer servers

### Q:  How do I use this in my project?

That's up to you, but I use the MSBuild.ILMerge Task available at https://www.nuget.org/packages/MSBuild.ILMerge.Task/

To use that, just follow the instructions from the package above to install it. Then import the dll found in the bin/Release folder of this repo, as a project reference to your mod project and make sure that copy local is set to true in the reference properties. You can always build from source as well.

### Q: How do you feel about pull requests?

My fondest dreams are of pull requests. If you want to contribute, awesome. Just submit the PR in an issue so I can track changes.
