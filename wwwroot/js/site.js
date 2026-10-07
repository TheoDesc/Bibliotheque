// Aperçu de l'affiche choisie dans le formulaire
document.addEventListener("DOMContentLoaded", () => {
    const input = document.querySelector("[data-poster-input]");
    const preview = document.querySelector("[data-poster-preview]");
    if (!input || !preview) return;

    input.addEventListener("change", () => {
        const file = input.files && input.files[0];
        if (!file) return;
        preview.src = URL.createObjectURL(file);
        preview.classList.remove("d-none");
    });
});
