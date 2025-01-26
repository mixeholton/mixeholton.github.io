Adds following to client:
- Simple token control
- Swagger for dev controller access

Remember to:
- Login, default via ../dev/access, Specified session login via ../dev/access/{IdString}
- Call await SessionHandler.Initialize(); OnNavigateAsync in Routes.razor on clients 
(Else login will be needed after each refresh)
- Set @attribute [Authorize] on _Imports.razor module ui component libraries
- AddAdditionalAssemblies for module ui component libraries on server