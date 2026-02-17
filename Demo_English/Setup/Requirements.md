# GitHub Copilot SDK - Setup Guide

## Requirements

### 1. Installed `.NET SDK` (8 or later) on your machine

<details>

Verify installation by running the following command in your terminal:

```
dotnet --list-sdks
```

Expected output example:

```
8.0.417 [C:\Program Files\dotnet\sdk]
9.0.305 [C:\Program Files\dotnet\sdk]
```
> **NOTE:** This demo is using .NET 8 (LTS) through Visual Studio 2022.

</details>

### 2. Updating `Windows PowerShell` (to 6 or later) on your machine

<details>

> PowerShell 6 or later is required by `GitHub Copilot CLI`.

Mosty likely, by default you have Windows PowerShell 5.1 installed on your Windows machine. To check the version, run the following command in your terminal:

```
$PSVersionTable.PSVersion
```

Expected output example:
```
Major  Minor  Build  Revision
-----  -----  -----  --------
5      1      26100  7462
```

Updating to the last version of PowerShell can be done by typing the following command in your terminal:

```
winget install --id Microsoft.PowerShell --source winget
```

Restart your terminal or open a new one and verify the version again:

```
$PSVersionTable.PSVersion
```

Expected output example:

```
Major  Minor  Patch  PreReleaseLabel BuildLabel
-----  -----  -----  --------------- ----------
7      5      4
```

---
Shorter command to check the version:

```
pwsh
```

Expected output example:

```
PowerShell 7.5.4
```

</details>

### 3. Installed `GitHub Copilot CLI` on your machine (instruction linked below)

<details>

Verify installation by running the following command in your terminal:

```
copilot --version
```

Expected output example:

```
0.0.395
Commit: 4b4fe6e
```

</details>

### 4. Active `GitHub Copilot subscription`

<details>

It can be a _free_ or _pro plan_ **GitHub Copilot subscription** activated on your private **GitHub account**.

</details>

### 5. Download NuGet package for `GitHub Copilot SDK`

<details>

> Find and add to your project: `GitHub.Copilot.SDK` from NuGet package manager.

</details>

---

## Resources

1. [**GitHub Docs:** Installing GitHub Copilot CLI](https://docs.github.com/en/copilot/how-tos/set-up/install-copilot-cli) - Guide on how to install **GitHub Copilot CLI**

2. [**GitHub Docs:** Using GitHub Copilot CLI](https://docs.github.com/en/copilot/how-tos/use-copilot-agents/use-copilot-cli) - Guide on how to use **GitHub Copilot CLI**

3. [**GitHub Copilot SDK:** Documentation](https://github.com/github/copilot-sdk) - Official documentation for **GitHub Copilot SDK**

	a. [Getting Started Guide](https://github.com/github/copilot-sdk/blob/main/docs/getting-started.md) - Instruction on how to set up and use the **GitHub Copilot SDK**
	
	b. [.NET (C#) Cookbook](https://github.com/github/copilot-sdk/blob/main/cookbook/dotnet/README.md) - Examples and recipes for using GitHub Copilot SDK with **.NET**

4. [**Awesome GitHub Copilot Customizations**](https://github.com/github/awesome-copilot/blob/main/instructions/copilot-sdk-csharp.instructions.md) - Community contributed customizations and examples for using **GitHub Copilot SDK**