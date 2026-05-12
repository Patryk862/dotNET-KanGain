function odswiezGrafik() {
    const data = document.getElementById('dataPicker').value;
    const lokId = document.getElementById('lokalizacjaSelect').value;
    
    // Przeładowanie strony z nowymi parametrami w URL
    window.location.href = `?data=${data}&lokalizacjaId=${lokId}`;
}