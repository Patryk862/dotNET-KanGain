document.addEventListener("DOMContentLoaded", function () {
    const selectedList = document.getElementById('selected-list');
    const hiddenInput = document.getElementById('UkrytyOpisParametrow');

    if (!selectedList || !hiddenInput) return;

    let parametry = {};
    const parametryContainer = document.getElementById('zapisaneParametryData');
    if (parametryContainer) {
        try {
            parametry = JSON.parse(parametryContainer.textContent || "{}");
        } catch (e) {
            console.error("Błąd parsowania zapisanych parametrów", e);
        }
    }

    window.zapiszDoJson = function () {
        const słownik = {};
        selectedList.querySelectorAll('.pozycja-planu-item').forEach(item => {
            const id = item.getAttribute('data-cwiczenie-id');
            const s = item.querySelector('.input-serie').value;
            const p = item.querySelector('.input-przerwa').value;

            słownik[id] = {
                s: Math.max(1, parseInt(s) || 3),
                p: Math.max(15, parseInt(p) || 60)
            };
        });
        hiddenInput.value = JSON.stringify(słownik);
    };

    window.korygujWartosc = function (el, min) {
        if (el.value === '' || parseInt(el.value) < min) {
            el.value = min;
        }
        zapiszDoJson();
    };

    window.usunCwiczenie = function (btnElement, id) {
        btnElement.closest('.pozycja-planu-item').remove();
        zapiszDoJson();

        const addButton = document.querySelector(`.add-btn[data-id="${id}"]`);
        if (addButton) {
            addButton.classList.remove('disabled');
            addButton.classList.replace('btn-secondary', 'btn-outline-light');
            addButton.innerText = "Dodaj";
        }
    };

    window.dodajCwiczenieDoZestawu = function (id, name, serie = 3, przerwa = 60) {
        if (selectedList.querySelector(`[data-cwiczenie-id="${id}"]`)) return;

        const li = document.createElement('li');
        li.className = "list-group-item bg-transparent border-0 border-bottom border-secondary text-white py-2 px-0 d-flex justify-content-between align-items-center pozycja-planu-item";
        li.setAttribute('data-cwiczenie-id', id);

        li.innerHTML = `
            <div class="d-flex align-items-center flex-grow-1">
                <div class="fw-bold text-truncate pe-2" style="width: 220px; color: var(--gym-primary);">${name}:</div>
                <div class="d-flex align-items-center">
                    <div class="d-flex align-items-center me-4">
                        <input type="number"
                            oninput="zapiszDoJson()"
                            onchange="korygujWartosc(this, 1)"
                            onkeydown="if(['-', '+', 'e', 'E'].includes(event.key)) event.preventDefault();"
                            class="form-control form-control-sm bg-dark text-white border-secondary text-center input-serie"
                            value="${serie}" min="1" style="width: 50px; height: 28px;">
                        <span class="text-secondary small ms-1">serie</span>
                    </div>
                    <div class="d-flex align-items-center">
                        <input type="number"
                            oninput="zapiszDoJson()"
                            onchange="korygujWartosc(this, 15)"
                            onkeydown="if(['-', '+', 'e', 'E'].includes(event.key)) event.preventDefault();"
                            class="form-control form-control-sm bg-dark text-white border-secondary text-center input-przerwa"
                            value="${przerwa}" min="15" step="5" style="width: 60px; height: 28px;">
                        <span class="text-secondary small ms-1">s przerwy</span>
                    </div>
                </div>
            </div>
            <button type="button" class="btn-close btn-close-white ms-2" style="font-size: 0.8rem;" onclick="usunCwiczenie(this, '${id}')"></button>
            <input type="hidden" name="WybraneCwiczeniaIds" value="${id}" />
        `;
        selectedList.appendChild(li);
        zapiszDoJson();

        const addButton = document.querySelector(`.add-btn[data-id="${id}"]`);
        if (addButton) {
            addButton.classList.add('disabled');
            addButton.classList.replace('btn-outline-light', 'btn-secondary');
            addButton.innerText = "Dodane";
        }
    };

    document.querySelectorAll('.add-btn').forEach(btn => {
        btn.addEventListener('click', function () {
            dodajCwiczenieDoZestawu(this.dataset.id, this.dataset.name);
        });
    });

    const kreatorGlowny = document.getElementById('kreator-glowny');
    if (kreatorGlowny) {
        const rawIds = kreatorGlowny.getAttribute('data-zapisane-ids');
        if (rawIds && rawIds.trim() !== "") {
            rawIds.split(',').forEach(id => {
                const btn = document.querySelector(`.add-btn[data-id="${id}"]`);
                if (btn) {
                    const p = parametry[id] || {};
                    dodajCwiczenieDoZestawu(id, btn.dataset.name, p.s || 3, p.p || 60);
                }
            });
        }
    }
}); document.addEventListener("DOMContentLoaded", function () {
    const selectedList = document.getElementById('selected-list');
    const hiddenInput = document.getElementById('UkrytyOpisParametrow');

    if (!selectedList || !hiddenInput) return;

    let parametry = {};
    const parametryContainer = document.getElementById('zapisaneParametryData');
    if (parametryContainer) {
        try {
            parametry = JSON.parse(parametryContainer.textContent || "{}");
        } catch (e) {
            console.error("Błąd parsowania zapisanych parametrów", e);
        }
    }

    window.zapiszDoJson = function () {
        const słownik = {};
        selectedList.querySelectorAll('.pozycja-planu-item').forEach(item => {
            const id = item.getAttribute('data-cwiczenie-id');
            const s = item.querySelector('.input-serie').value;
            const p = item.querySelector('.input-przerwa').value;

            słownik[id] = {
                s: Math.max(1, parseInt(s) || 3),
                p: Math.max(15, parseInt(p) || 60)
            };
        });
        hiddenInput.value = JSON.stringify(słownik);
    };

    window.korygujWartosc = function (el, min) {
        if (el.value === '' || parseInt(el.value) < min) {
            el.value = min;
        }
        zapiszDoJson();
    };

    window.usunCwiczenie = function (btnElement, id) {
        btnElement.closest('.pozycja-planu-item').remove();
        zapiszDoJson();

        const addButton = document.querySelector(`.add-btn[data-id="${id}"]`);
        if (addButton) {
            addButton.classList.remove('disabled');
            addButton.classList.replace('btn-secondary', 'btn-outline-light');
            addButton.innerText = "Dodaj";
        }
    };

    window.dodajCwiczenieDoZestawu = function (id, name, serie = 3, przerwa = 60) {
        if (selectedList.querySelector(`[data-cwiczenie-id="${id}"]`)) return;

        const li = document.createElement('li');
        li.className = "list-group-item bg-transparent border-0 border-bottom border-secondary text-white py-2 px-0 d-flex justify-content-between align-items-center pozycja-planu-item";
        li.setAttribute('data-cwiczenie-id', id);

        li.innerHTML = `
            <div class="d-flex align-items-center flex-grow-1">
                <div class="fw-bold text-truncate pe-2" style="width: 220px; color: var(--gym-primary);">${name}:</div>
                <div class="d-flex align-items-center">
                    <div class="d-flex align-items-center me-4">
                        <input type="number"
                            oninput="zapiszDoJson()"
                            onchange="korygujWartosc(this, 1)"
                            onkeydown="if(['-', '+', 'e', 'E'].includes(event.key)) event.preventDefault();"
                            class="form-control form-control-sm bg-dark text-white border-secondary text-center input-serie"
                            value="${serie}" min="1" style="width: 50px; height: 28px;">
                        <span class="text-secondary small ms-1">serie</span>
                    </div>
                    <div class="d-flex align-items-center">
                        <input type="number"
                            oninput="zapiszDoJson()"
                            onchange="korygujWartosc(this, 15)"
                            onkeydown="if(['-', '+', 'e', 'E'].includes(event.key)) event.preventDefault();"
                            class="form-control form-control-sm bg-dark text-white border-secondary text-center input-przerwa"
                            value="${przerwa}" min="15" step="5" style="width: 60px; height: 28px;">
                        <span class="text-secondary small ms-1">s przerwy</span>
                    </div>
                </div>
            </div>
            <button type="button" class="btn-close btn-close-white ms-2" style="font-size: 0.8rem;" onclick="usunCwiczenie(this, '${id}')"></button>
            <input type="hidden" name="WybraneCwiczeniaIds" value="${id}" />
        `;
        selectedList.appendChild(li);
        zapiszDoJson();

        const addButton = document.querySelector(`.add-btn[data-id="${id}"]`);
        if (addButton) {
            addButton.classList.add('disabled');
            addButton.classList.replace('btn-outline-light', 'btn-secondary');
            addButton.innerText = "Dodane";
        }
    };

    document.querySelectorAll('.add-btn').forEach(btn => {
        btn.addEventListener('click', function () {
            dodajCwiczenieDoZestawu(this.dataset.id, this.dataset.name);
        });
    });

    const kreatorGlowny = document.getElementById('kreator-glowny');
    if (kreatorGlowny) {
        const rawIds = kreatorGlowny.getAttribute('data-zapisane-ids');
        if (rawIds && rawIds.trim() !== "") {
            rawIds.split(',').forEach(id => {
                const btn = document.querySelector(`.add-btn[data-id="${id}"]`);
                if (btn) {
                    const p = parametry[id] || {};
                    dodajCwiczenieDoZestawu(id, btn.dataset.name, p.s || 3, p.p || 60);
                }
            });
        }
    }
});