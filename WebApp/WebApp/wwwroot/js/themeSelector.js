export function onDisplayModeChanged(mode) {
    if (mode === 'dark' || (mode === 'system' && window.matchMedia('(prefers-color-scheme: dark)').matches)) {
        ////document.documentElement.id = 'dark';
        document.documentElement.style.setProperty("--background", "#222");
        document.documentElement.style.setProperty("--background-2", "#fff");
        document.documentElement.style.setProperty("--button-background", "#212529");
        document.documentElement.style.setProperty("--font-color", "#fff");
        document.documentElement.style.setProperty("--font-color-2", "#000");
        document.documentElement.style.setProperty("--highlight", "#393939");
        document.documentElement.style.setProperty("--highlight-2", "#444444");
        document.documentElement.style.setProperty("--link", "#3ca4ff");
        document.documentElement.style.setProperty("--load-circle-background", "#444444");
        document.documentElement.style.setProperty("--button-boxShadow", "#f7f7f7");

        document.documentElement.style.setProperty("--palette-1", "#e63946");
        document.documentElement.style.setProperty("--palette-2", "#f1faee");
        document.documentElement.style.setProperty("--palette-2-hover", "#cb333f");
        document.documentElement.style.setProperty("--palette-3", "#a8dadc");
        document.documentElement.style.setProperty("--palette-4", "#457b9d");
        document.documentElement.style.setProperty("--palette-5", "#1d3557");
        document.documentElement.style.setProperty("--form-box-shadow", "#3a3a3a");

        document.documentElement.style.setProperty("--font-color-1-hover", "#ccc");
        document.documentElement.style.setProperty("--font-color-2-hover", "#333");
    }
    else {
        //document.documentElement.id = 'light';
        document.documentElement.style.setProperty("--background", "#fff");
        document.documentElement.style.setProperty("--background-2", "#222");
        document.documentElement.style.setProperty("--button-background", "#f8f9fa");
        document.documentElement.style.setProperty("--font-color", "#000");
        document.documentElement.style.setProperty("--font-color-2", "#fff");
        document.documentElement.style.setProperty("--highlight", "#f7f7f7");
        document.documentElement.style.setProperty("--highlight-2", "#95a6a6");
        document.documentElement.style.setProperty("--link", "#0366d6");
        document.documentElement.style.setProperty("--load-circle-background", "#e0e0e0");
        document.documentElement.style.setProperty("--button-boxShadow", "#393939");

        document.documentElement.style.setProperty("--palette-1", "#e63946");
        document.documentElement.style.setProperty("--palette-2", "#f1faee");
        document.documentElement.style.setProperty("--palette-2-hover", "#cb333f");
        document.documentElement.style.setProperty("--palette-3", "#a8dadc");
        document.documentElement.style.setProperty("--palette-4", "#457b9d");
        document.documentElement.style.setProperty("--palette-5", "#1d3557");
        document.documentElement.style.setProperty("--form-box-shadow", "#d2d5e4");

        document.documentElement.style.setProperty("--font-color-1-hover", "#333");
        document.documentElement.style.setProperty("--font-color-2-hover", "#ccc");
        //document.documentElement.classList.remove('dark');
    }
}