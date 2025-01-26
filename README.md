# Packages for komit modules

- Important!
Always set the following in values in the <PropertyGroup> of new projects
    <Version>1.0.0</Version>
    <Authors>Komit dev team</Authors>
    <Company>Komit a.m.b.a.</Company>
    <GeneratePackageOnBuild>True</GeneratePackageOnBuild>
Packages will only be published when incrementing the project package version
The projects in this solution should only reference each other via nuget packages
