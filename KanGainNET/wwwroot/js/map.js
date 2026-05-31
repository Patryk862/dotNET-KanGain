// Deklarujemy mapę globalnie, aby inne funkcje miały do niej dostęp
let map;

document.addEventListener("DOMContentLoaded", function () {
    const mapElement = document.getElementById('map');

    // Zabezpieczenie: uruchamiamy skrypt tylko wtedy, gdy na stronie faktycznie jest mapa
    if (mapElement) {
        // Inicjalizacja mapy
        map = L.map('map', { zoomControl: false }).setView([52.2, 19.0], 6);

        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
        }).addTo(map);

        L.control.zoom({ position: 'bottomright' }).addTo(map);

        // Odczytanie danych z ukrytego elementu HTML
        const dataContainer = document.getElementById('klubyData');

        if (dataContainer) {
            try {
                // Konwersja tekstu na tablicę obiektów JS
                const kluby = JSON.parse(dataContainer.textContent);

                // Nakładanie markerów na mapę
                kluby.forEach(k => {
                    L.marker([parseFloat(k.lat), parseFloat(k.lng)])
                        .addTo(map)
                        .bindPopup(`<b style="color:#ff4500">${k.nazwa}</b><br><span style="color:black">${k.adres}</span>`);
                });
            } catch (error) {
                console.error("Błąd podczas ładowania danych o klubach na mapie:", error);
            }
        }

        setTimeout(() => { map.invalidateSize(); }, 400);
    }
});

// Zdefiniowanie funkcji globalnie (w obiekcie window), 
// aby atrybut 'onclick' wewnątrz Twojego HTML-a mógł ją wywołać
window.flyToLocation = function (lat, lng) {
    if (map) {
        map.flyTo([lat, lng], 15, { duration: 1.5 });
    }
};