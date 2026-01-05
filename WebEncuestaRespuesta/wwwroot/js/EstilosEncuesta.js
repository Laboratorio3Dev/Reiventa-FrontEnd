window.loadSurveyStyles = () => {
    // Evita duplicar si ya están cargados
    if (document.getElementById("survey-styles-loaded")) return;

    const link1 = document.createElement("link");
    link1.rel = "stylesheet";
    link1.href = "css/str.css";

    const link2 = document.createElement("link");
    link2.rel = "stylesheet";
    link2.href = "css/str-base.css";

    // Marcador para evitar recargas duplicadas
    const marker = document.createElement("meta");
    marker.id = "survey-styles-loaded";

    document.head.appendChild(link1);
    document.head.appendChild(link2);
    document.head.appendChild(marker);
};


window.unloadSurveyStyles = () => {
    document.querySelectorAll('link[href^="css/str"]').forEach(l => l.remove());
    const marker = document.getElementById("survey-styles-loaded");
    if (marker) marker.remove();
};
(function () {
    function wireCharCounters(root) {
        const scope = root || document;

        scope.querySelectorAll("textarea.js-charcount").forEach((ta) => {
            // Evita duplicar eventos si Razor re-renderiza o abres modales
            if (ta.dataset.ccWired === "1") return;
            ta.dataset.ccWired = "1";

            const container = ta.closest(".text-area-container") || ta.parentElement;
            const counter = container ? container.querySelector(".charcount") : null;

            const maxAttr = ta.getAttribute("maxlength");
            const max = maxAttr ? parseInt(maxAttr, 10) : null;

            const update = () => {
                const len = ta.value.length;
                if (counter) counter.textContent = String(len).padStart(2, "0");
                // Si quieres mostrar /max dinámico, avísame y lo agrego
            };

            ta.addEventListener("input", update);
            ta.addEventListener("change", update);

            // Inicializa al cargar (por si viene con texto)
            update();
        });
    }

    // Carga inicial
    document.addEventListener("DOMContentLoaded", () => wireCharCounters());

    // Si usas Bootstrap modal o contenido dinámico, puedes llamar esto manualmente:
    window.wireCharCounters = () => wireCharCounters(document);
})();