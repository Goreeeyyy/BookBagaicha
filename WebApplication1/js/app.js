document.addEventListener('DOMContentLoaded', () => {
    // Cart functionality
    let cart = JSON.parse(localStorage.getItem('cart')) || [];
    updateCartCount();

    // Add to cart buttons
    document.querySelectorAll('.add-to-cart').forEach(btn => {
        btn.addEventListener('click', addToCart);
    });

    // Subscription form
    document.getElementById('btnSubscribe').addEventListener('click', handleSubscribe);

    // Image upload functionality
    document.querySelectorAll('.image-upload').forEach(input => {
        input.addEventListener('change', handleImageUpload);
    });

    // Carousel navigation
    document.getElementById('prevBtn').addEventListener('click', () => {
        document.querySelector('.carousel-track').scrollBy({ left: -140, behavior: 'smooth' });
    });

    document.getElementById('nextBtn').addEventListener('click', () => {
        document.querySelector('.carousel-track').scrollBy({ left: 140, behavior: 'smooth' });
    });

    function addToCart(e) {
        const card = e.target.closest('.card');
        const title = card.querySelector('.card-title').textContent;
        const price = card.querySelector('.text-primary').textContent;

        cart.push({ title, price });
        localStorage.setItem('cart', JSON.stringify(cart));
        updateCartCount();
    }

    function updateCartCount() {
        document.getElementById('cartCount').textContent = cart.length;
    }

    function handleSubscribe(e) {
        e.preventDefault();
        const email = document.getElementById('subscribeEmail').value;
        if (validateEmail(email)) {
            localStorage.setItem('subscribedEmail', email);
            alert('Thank you for subscribing!');
        } else {
            alert('Please enter a valid email address');
        }
    }

    function validateEmail(email) {
        return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
    }

    function handleImageUpload(e) {
        const file = e.target.files[0];
        if (!file) return;

        const reader = new FileReader();
        reader.onload = function (e) {
            const img = document.createElement('img');
            img.src = e.target.result;
            img.alt = "Uploaded image";
            const label = e.target.parentElement;
            label.innerHTML = '';
            label.appendChild(img);
        };
        reader.readAsDataURL(file);
    }
});