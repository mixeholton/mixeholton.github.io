Get MudBlazor Package in your Nuget Package or with following command:
dotnet add package MudBlazor

Add Imports:
@using MudBlazor

USING THE SYSTEM
To use the Components you should insert this in your App.razor:
<script src="_content/Komit.Base.Ui.Components/script-loader.js"></script>


Then in the Program.cs you should insert this:

using Komit.Base.Ui.Components;

builder.Services.AddBaseUiComponents();