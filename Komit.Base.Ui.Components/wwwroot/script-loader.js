


loadedScripts = [];
loadedLinks = [];
window.loadScript = (scriptPath) => loadedScripts[scriptPath]
    // If loaded return empty promise
    ? new this.Promise((resolve, reject) => resolve())
    : new Promise(function (resolve, reject) {
        loadedScripts[scriptPath] = true;
        var script = document.createElement("script");
        script.src = scriptPath;
        script.type = "text/javascript";
        script.onload = () => resolve(scriptPath);
        script.onerror = () => reject(scriptPath);
        document["body"].appendChild(script);
    });
window.loadCss = (scriptPath) => loadedLinks[scriptPath]
    // If loaded return empty promise
    ? new this.Promise((resolve, reject) => resolve())
    : new Promise(function (resolve, reject) {
        loadedLinks[scriptPath] = true;
        var link = document.createElement("link");
        link.href = scriptPath;
        link.rel = 'stylesheet';
        link.type = "text/css";
        link.onload = () => resolve(scriptPath);
        link.onerror = () => reject(scriptPath);

        //document.querySelector('accounting-app').shadowRoot.appendChild(link);
        document["head"].appendChild(link);
    });

// Load MudBlazor JavaScript
window.loadScript("_content/MudBlazor/MudBlazor.min.js").catch(err => console.error("Failed to load MudBlazor script:", err));

// Load MudBlazor CSS
window.loadCss("_content/MudBlazor/MudBlazor.min.css").catch(err => console.error("Failed to load MudBlazor CSS:", err));