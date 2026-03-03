// tema com persistência
(function () {
    const k = 'st-theme', el = document.getElementById('theme');
    if (!el) return;
    const saved = localStorage.getItem(k) || 'dark';
    document.documentElement.setAttribute('data-bs-theme', saved);
    el.checked = saved === 'light';
    el.addEventListener('change', () => {
        const v = el.checked ? 'light' : 'dark';
        document.documentElement.setAttribute('data-bs-theme', v);
        localStorage.setItem(k, v);
    });
})();

// cards de gênero → preenche filtro e envia mantendo seção "genres"
document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.genre-card').forEach(btn => {
        btn.addEventListener('click', e => {
            e.preventDefault();
            const form = document.getElementById('catalogForm');
            if (!form) return;
            form.action = form.action.split('#')[0]; // sem hash
            form.querySelector('input[name="GeneroId"]').value = btn.dataset.genre;
            form.querySelector('input[name="Page"]').value = 1;
            form.querySelector('input[name="section"]').value = 'genres';
            form.submit();
        });
    });
});
// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
