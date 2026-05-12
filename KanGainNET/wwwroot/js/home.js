document.addEventListener("DOMContentLoaded", () => {
    // --- LOGIKA KARUZELI CENNIKA ---
    const cards = document.querySelectorAll('.pricing-card');
    const totalCards = cards.length;
    let currentIndex = Math.floor(totalCards / 2) - 1; 

    function updateCarousel() {
        const prevIndex = (currentIndex - 1 + totalCards) % totalCards;
        const nextIndex = (currentIndex + 1) % totalCards;

        cards.forEach((card, index) => {
            card.classList.remove('active', 'prev', 'next');
            const btn = card.querySelector('.dynamic-btn');
            
            if(btn) {
                btn.classList.remove('btn-gym', 'btn-outline-light');

                if (index === currentIndex) {
                    card.classList.add('active');
                    btn.classList.add('btn-gym');
                } 
                else if (index === prevIndex) {
                    card.classList.add('prev');
                    btn.classList.add('btn-outline-light');
                } 
                else if (index === nextIndex) {
                    card.classList.add('next');
                    btn.classList.add('btn-outline-light');
                } else {
                    btn.classList.add('btn-outline-light');
                }
            }
        });
    }

    // Przypisujemy funkcję pod strzałki do globalnego obiektu window
    window.moveCarousel = function(step) {
        currentIndex = (currentIndex + step + totalCards) % totalCards;
        updateCarousel();
    }

    cards.forEach((card, index) => {
        card.addEventListener('click', () => {
            if (card.classList.contains('prev') || card.classList.contains('next')) {
                currentIndex = index;
                updateCarousel();
            }
        });
    });

    // --- EFEKT PISANIA NA MASZYNIE ---
    const words = ["granice", "swoje słabości", "oczekiwania", "własne limity"];
    let wordIndex = 0;
    let timer;

    function typingEffect() {
        const element = document.getElementById('dynamic-text');
        if(!element) return; // Zabezpieczenie jeśli element nie istnieje na stronie

        let word = words[wordIndex].split("");
        var loopTyping = function () {
            if (word.length > 0) {
                element.innerHTML += word.shift();
            } else {
                setTimeout(deletingEffect, 2500);
                return false;
            }
            timer = setTimeout(loopTyping, 100);
        };
        loopTyping();
    }

    function deletingEffect() {
        const element = document.getElementById('dynamic-text');
        let word = words[wordIndex].split("");
        
        var loopDeleting = function () {
            if (word.length > 0) {
                word.pop();
                element.innerHTML = word.join("");
            } else {
                wordIndex = (wordIndex + 1) % words.length;
                typingEffect();
                return false;
            }
            timer = setTimeout(loopDeleting, 50);
        };
        loopDeleting();
    }

    updateCarousel();
    typingEffect();
});