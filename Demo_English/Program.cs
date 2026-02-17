using Demo_English._1_Basics;
using Demo_English._2_Intermediate;

try
{
    await A_BasicCall.RequestAsync("gpt-4o", "What is the capital of France?");

    //await B_StreamedCall.RequestAsync("gpt-4o", "Explain the theory of relativity in simple terms.");

    //await C_SystemMessage.RequestAsync("claude-sonnet-4.5", "Why Singleton can be considered an antipattern?");

    //await D_UsingTools.RequestAsync("gpt-4o", "Convert 200 EUR to USD");
    //await D_UsingTools.RequestAsync("gpt-4o", "Convert 200 EUR to JPY");

    //await E_InteractiveAssistant.RequestAsync("gpt-4o");  // Pick it up from here and have a conversation with the Agent

    //await F_McpServerIntegration.RequestAsync("gpt-4o", 
    //    """
    //    List all files and folders in the current directory using the following structure as an example:

    //    [Folder1]
    //      - file.dll
    //      - file.csproj
    //    [Folder2]
    //      - file.cs
    //    """);

    //await G_MultiAgent.RequestAsync(
    //    """
    //    How can I prepare a production‑ready version of my GitHub Copilot SDK 
    //    that does not rely on the GitHub Copilot CLI for license authorization, 
    //    model‑deployment configuration, server startup, or the runtime interface? 
    //    I want the SDK to operate independently without relying on the local 
    //    environment or requiring synchronization with the CLI.
    //    """);
}
catch (Exception exception)
{
    Console.WriteLine(exception.Message);
}